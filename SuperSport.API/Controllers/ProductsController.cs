using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<Product> GetProducts() {
            return _shopContext.Products.ToArray();
        }
    }
}
