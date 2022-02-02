using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetProducts() {
            var products = await _shopContext.Products.ToArrayAsync();
            return Ok(products);
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
