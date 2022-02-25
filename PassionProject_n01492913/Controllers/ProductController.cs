using PassionProject_n01492913.Models;
using PassionProject_n01492913.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PassionProject_n01492913.Controllers
{
    public class ProductController : Controller
    {
        private static readonly HttpClient client;
        JavaScriptSerializer jss = new JavaScriptSerializer();

        static ProductController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44309/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted: " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Product/List
        public ActionResult List()
        {
            //objective: communicate with product data api to retrieve a list of products
            //curl https://localhost:44309/api/productdata/listproducts

            string url = "productdata/listproducts";
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("Response Code: " + response.StatusCode);

            IEnumerable<ProductDto> products = response.Content.ReadAsAsync<IEnumerable<ProductDto>>().Result;
            Debug.WriteLine("Number of Products Received: " + products.Count());

            return View(products);
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            DetailsProduct ViewModel = new DetailsProduct();

            //existing product information
            //objective: communicate with product data api to retrieve one product
            //curl https://localhost:44309/api/productdata/findproduct/{id}

            string url = "productdata/findproduct/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ProductDto SelectedProduct = response.Content.ReadAsAsync<ProductDto>().Result;
            ViewModel.SelectedProduct = SelectedProduct;

            //all wishlists for select
            url = "wishlistdata/listwishlists";
            response = client.GetAsync(url).Result;
            IEnumerable<WishlistDto> WishlistOptions = response.Content.ReadAsAsync<IEnumerable<WishlistDto>>().Result;
            ViewModel.WishlistOptions = WishlistOptions;

            Debug.WriteLine("Response Code: " + response.StatusCode);
            Debug.WriteLine("Product Received: " + SelectedProduct.ProductName);

            return View(ViewModel);
        }

        //POST: Product/Associate/{productid}
        [HttpPost]
        public ActionResult Associate(int id, int WishlistID)
        {
            GetApplicationCookie();
            Debug.WriteLine("Attempting to associate product :" + id + " with wishlist " + WishlistID);

            //call our api to associate product with wishlist
            string url = "productdata/associateproductwithwishlist/" + id + "/" + WishlistID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //Get: Product/UnAssociate/{id}?WishlistID={wishlistID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int WishlistID)
        {
            GetApplicationCookie();
            Debug.WriteLine("Attempting to unassociate product: " + id + " with wishlist: " + WishlistID);

            //call our api to associate product with wishlist
            string url = "productdata/unassociateproductwithwishlist/" + id + "/" + WishlistID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            //return RedirectToAction("Details/" + id);
            return RedirectToAction("Details/" + WishlistID, "Wishlist");
        }

        // GET: Product/New
        public ActionResult New()
        {
            //retrieve a list of categories
            //GET api/categorydata/listcategories

            string url = "categorydata/listcategories";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CategoryDto> CategoryOptions = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;

            return View(CategoryOptions);
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(Product product)
        {
            GetApplicationCookie();
            Debug.WriteLine("Inputted Product Name: " + product.ProductName);

            //objective: add a new product into system using api
            //curl -d @product.json -H "Content-type:application/json" https://localhost:44309/api/productdata/addproduct
            string url = "productdata/addproduct";

            string jsonpayload = jss.Serialize(product);
            Debug.WriteLine("jsonpayload: " + jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateProduct ViewModel = new UpdateProduct();
            
            //existing product information
            string url = "productdata/findproduct/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ProductDto SelectedProduct = response.Content.ReadAsAsync<ProductDto>().Result;
            ViewModel.SelectedProduct = SelectedProduct;

            //include all categories to choose from when updating product
            url = "categorydata/listcategories/";
            response = client.GetAsync(url).Result;
            IEnumerable<CategoryDto> CategoryOptions = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;
            ViewModel.CategoryOptions = CategoryOptions;

            return View(ViewModel);
        }

        // POST: Product/Update/5
        [HttpPost]
        public ActionResult Update(int id, Product product, HttpPostedFileBase ProductPic)
        {
            GetApplicationCookie();
            string url = "productdata/updateproduct/" + id;
            string jsonpayload = jss.Serialize(product);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            //Update request successful, and we have image data
            if (response.IsSuccessStatusCode && ProductPic != null)
            {
                //Updating the product picture as a separate request
                Debug.WriteLine("Calling Update Image method.");
                //Send over image data for product
                url = "ProductData/UploadProductPic/" + id;
                Debug.WriteLine("Received Product Picture " + ProductPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(ProductPic.InputStream);
                requestcontent.Add(imagecontent, "ProductPic", ProductPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //No image upload, but update still successful
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Product/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "productdata/findproduct/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ProductDto SelectedProduct = response.Content.ReadAsAsync<ProductDto>().Result;
            return View(SelectedProduct);
        }


        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "productdata/deleteproduct/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
