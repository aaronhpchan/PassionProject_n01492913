using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
                ProductHasPic = a.ProductHasPic,
                PicExtension = a.PicExtension,
                CategoryName = a.Category.CategoryName
            }));

            return ProductDtos;
        }

        /// <summary>
        /// Gathers info about products related to a particular category
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all products in the database that match to a particular category id
        /// </returns>
        /// <param name="id">Category ID</param>
        /// <example>
        /// GET: api/ProductData/ListProductsForCategory/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ProductDto))]
        public IHttpActionResult ListProductsForCategory(int id)
        {
            //all products that have wishlists which match with ID
            List<Product> Products = db.Products.Where(a => a.CategoryID == id).ToList();
            List<ProductDto> ProductDtos = new List<ProductDto>();

            Products.ForEach(a => ProductDtos.Add(new ProductDto()
            {
                ProductID = a.ProductID,
                ProductName = a.ProductName,
                ProductPrice = a.ProductPrice,
                ProductHasPic = a.ProductHasPic,
                PicExtension = a.PicExtension
            }));

            return Ok(ProductDtos);
        }

        /// <summary>
        /// Gathers info about products related to a particular wishlist
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all products in the database, including their associated categories that match to a particular wishlist id
        /// </returns>
        /// <param name="id">Wishlist ID</param>
        /// <example>
        /// GET: api/ProductData/ListProductsForWishlist/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ProductDto))]
        public IHttpActionResult ListProductsForWishlist(int id)
        {
            //all products that have wishlists which match with ID
            List<Product> Products = db.Products.Where(
                a => a.Wishlists.Any(
                    k => k.WishlistID == id
                )).ToList();
            List<ProductDto> ProductDtos = new List<ProductDto>();

            Products.ForEach(a => ProductDtos.Add(new ProductDto()
            {
                ProductID = a.ProductID,
                ProductName = a.ProductName,
                ProductPrice = a.ProductPrice,
                ProductHasPic = a.ProductHasPic,
                PicExtension = a.PicExtension,
                CategoryName = a.Category.CategoryName
            }));

            return Ok(ProductDtos);
        }

        /// <summary>
        /// Associates a particular wishlist with a particular product
        /// </summary>
        /// <param name="productid">The product ID primary key</param>
        /// <param name="wishlistid">The wishlist ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/ProductData/AssociateProductWithWishlist/9/1
        /// </example>
        [HttpPost]
        [Route("api/ProductData/AssociateProductWithWishlist/{productid}/{wishlistid}")]
        public IHttpActionResult AssociateProductWithWishlist(int productid, int wishlistid)
        {

            Product SelectedProduct = db.Products.Include(a => a.Wishlists).Where(a => a.ProductID == productid).FirstOrDefault();
            Wishlist SelectedWishlist = db.Wishlists.Find(wishlistid);

            if (SelectedProduct == null || SelectedWishlist == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input product id: " + productid);
            Debug.WriteLine("selected product name: " + SelectedProduct.ProductName);
            Debug.WriteLine("input wishlist id: " + wishlistid);
            Debug.WriteLine("selected wishlist name: " + SelectedWishlist.WishlistName);


            SelectedProduct.Wishlists.Add(SelectedWishlist);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular wishlist and a particular product
        /// </summary>
        /// <param name="productid">The product ID primary key</param>
        /// <param name="wishlistid">The wishlist ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/ProductData/UnAssociateProductWithWishlist/9/1
        /// </example>
        [HttpPost]
        [Route("api/ProductData/UnAssociateProductWithWishlist/{productid}/{wishlistid}")]
        public IHttpActionResult UnAssociateProductWithWishlist(int productid, int wishlistid)
        {

            Product SelectedProduct = db.Products.Include(a => a.Wishlists).Where(a => a.ProductID == productid).FirstOrDefault();
            Wishlist SelectedWishlist = db.Wishlists.Find(wishlistid);

            if (SelectedProduct == null || SelectedWishlist == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input product id: " + productid);
            Debug.WriteLine("selected product name: " + SelectedProduct.ProductName);
            Debug.WriteLine("input wishlist id: " + wishlistid);
            Debug.WriteLine("selected wishlist name: " + SelectedWishlist.WishlistName);


            SelectedProduct.Wishlists.Remove(SelectedWishlist);
            db.SaveChanges();

            return Ok();
        }

        // GET: api/ProductData/FindProduct/5
        [ResponseType(typeof(ProductDto))]
        [HttpGet]
        public IHttpActionResult FindProduct(int id)
        {
            Product Product = db.Products.Find(id);
            ProductDto ProductDto = new ProductDto()
            {
                ProductID = Product.ProductID,
                ProductName = Product.ProductName,
                ProductPrice = Product.ProductPrice,
                ProductHasPic = Product.ProductHasPic,
                PicExtension = Product.PicExtension,
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
            db.Entry(product).Property(a => a.ProductHasPic).IsModified = false;
            db.Entry(product).Property(a => a.PicExtension).IsModified = false;

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

        /// <summary>
        /// Receives product picture data, uploads it to the webserver and updates the product's HasPic option
        /// </summary>
        /// <param name="id">the product id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F productpic=@file.jpg "https://localhost:xx/api/productdata/uploadproductpic/2"
        /// POST: api/productData/UpdateProductPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        [HttpPost]
        public IHttpActionResult UploadProductPic(int id)
        {
            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var productPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (productPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(productPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/animals/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Products/"), fn);

                                //save the file
                                productPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the animal haspic and picextension fields in the database
                                Product SelectedProduct = db.Products.Find(id);
                                SelectedProduct.ProductHasPic = haspic;
                                SelectedProduct.PicExtension = extension;
                                db.Entry(SelectedProduct).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("product Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }
                }
                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();
            }
        }

        // POST: api/ProductData/AddProduct
        [ResponseType(typeof(Product))]
        [HttpPost]
        public IHttpActionResult AddProduct(Product product)
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

            if (product.ProductHasPic && product.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Products/" + id + "." + product.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists...preparing to delete");
                    System.IO.File.Delete(path);
                }
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok();
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