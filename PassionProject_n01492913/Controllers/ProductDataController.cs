using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject_n01492913.Models;

namespace PassionProject_n01492913.Controllers
{
    public class ProductDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProductData/ListProducts
        [HttpGet]
        public IEnumerable<ProductDto> ListProducts()
        {
            List<Product> Products = db.Products.ToList();
            List<ProductDto> ProductDtos = new List<ProductDto>();

            Products.ForEach(a => ProductDtos.Add(new ProductDto()
            {
                ProductID = a.ProductID,
                ProductName = a.ProductName,
                ProductPrice = a.ProductPrice,
                CategoryName = a.Category.CategoryName
            }));

            return ProductDtos;
        }


        // GET: api/ProductData/FindProduct/5
        [ResponseType(typeof(Product))]
        [HttpGet]
        public IHttpActionResult FindProduct(int id)
        {
            Product Product = db.Products.Find(id);
            ProductDto ProductDto = new ProductDto()
            {
                ProductID = Product.ProductID,
                ProductName = Product.ProductName,
                ProductPrice = Product.ProductPrice,
                CategoryName = Product.Category.CategoryName
            };
            if (Product == null)
            {
                return NotFound();
            }

            return Ok(ProductDto);
        }

        // POST: api/ProductData/UpdateProduct/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductID)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProductData/AddProduct
        [ResponseType(typeof(Product))]
        [HttpPost]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        }

        // DELETE: api/ProductData/DeleteProduct/5
        [ResponseType(typeof(Product))]
        [HttpPost]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductID == id) > 0;
        }
    }
}