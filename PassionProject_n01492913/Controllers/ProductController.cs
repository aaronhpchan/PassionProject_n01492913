using PassionProject_n01492913.Models;
using PassionProject_n01492913.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace PassionProject_n01492913.Controllers
{
    public class ProductController : Controller
    {
        private static readonly HttpClient client;

        static ProductController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44309/api/");
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

        // GET: Product/New
        public ActionResult New()
        {
            //retrieve a list of wishlists
            //GET api/wishlistdata/listwishlists

            string url = "wishlistdata/listwishlists";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<WishlistDto> wishlistOptions = response.Content.ReadAsAsync<IEnumerable<WishlistDto>>().Result;

            return View(wishlistOptions);
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
