using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetProducts([FromQuery] QueryParameters queryParameters) {
            IQueryable<Product> products = _shopContext.Products;

            products = products
                    .Skip(queryParameters.Size * (queryParameters.Page - 1))
                    .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) {
            var product = await _shopContext.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
