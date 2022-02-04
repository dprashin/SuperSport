﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperSport.API.Classes;
using SuperSport.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSport.API.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase {

        private readonly ShopContext _shopContext;
        public ProductsController(ShopContext shopContext) {
            _shopContext = shopContext;

            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters queryParameters) {
            IQueryable<Product> products = _shopContext.Products;

            if (queryParameters.MinPrice != null && queryParameters.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value &&
                         p.Price <= queryParameters.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchString))
            {
                products = products.Where(p => p.Sku.ToLower().Contains(queryParameters.SearchString.ToLower()) ||
                                               p.Name.ToLower().Contains(queryParameters.SearchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }

            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            products = products
                    .Skip(queryParameters.Size * (queryParameters.Page - 1))
                    .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) {
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product) {
            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute]int id, [FromBody]Product product) {
            if(id != product.Id)
            {
                return BadRequest();
            }

            _shopContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(_shopContext.Products.Find(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id) {
            var product = await _shopContext.Products.FindAsync(id);
            if(product == null){
                return NotFound();
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();

            //return product or NoContent
            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteMultipleProducts([FromQuery]int[] ids) {

            //store list of products found in the database
            var products = new List<Product>();

            foreach(var id in ids){
                var product = await _shopContext.Products.FindAsync(id);
                if(product == null)
                {
                    return NotFound();
                }
                products.Add(product);
            }

            _shopContext.Products.RemoveRange(products);
            await _shopContext.SaveChangesAsync();

            //return product or NoContent
            return Ok(products);
        }
    }
}
