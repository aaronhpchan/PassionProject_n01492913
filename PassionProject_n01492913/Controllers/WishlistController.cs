using PassionProject_n01492913.Models;
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
    public class WishlistController : Controller
    {
        private static readonly HttpClient client;
        JavaScriptSerializer jss = new JavaScriptSerializer();

        static WishlistController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44309//api/wishlistdata/");
        }
        
        // GET: Wishlist/List
        public ActionResult List()
        {
            //objective: communicate with wishlist data api to retrieve a list of wishlists
            //curl https://localhost:44309//api/wishlistdata/listwishlists

            string url = "listwishlists";
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("Response Code: " + response.StatusCode);

            IEnumerable<WishlistDto> wishlists = response.Content.ReadAsAsync<IEnumerable<WishlistDto>>().Result;
            Debug.WriteLine("Number of Wishlist Received: " + wishlists.Count());

            return View(wishlists);
        }

        // GET: Wishlist/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with wishlist data api to retrieve one wishlist
            //curl https://localhost:44309//api/wishlistdata/findwishlist/{id}

            string url = "findwishlist/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("Response Code: " + response.StatusCode);

            WishlistDto selectedWishlist = response.Content.ReadAsAsync<WishlistDto>().Result;
            Debug.WriteLine("Wishlist Received: " + selectedWishlist.WishlistName);

            return View(selectedWishlist);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Wishlist/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Wishlist/Create
        [HttpPost]
        public ActionResult Create(Wishlist wishlist)
        {
            Debug.WriteLine("Inputted Wishlist Name: " + wishlist.WishlistName);

            //objective: add a new wishlist into system using api
            //curl -d @wishlist.json -H "Content-type:application/json" https://localhost:44309//api/wishlistdata/addwishlist
            string url = "addwishlist";

            string jsonpayload = jss.Serialize(wishlist);

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

            return RedirectToAction("List");
        }

        // GET: Wishlist/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Wishlist/Edit/5
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

        // GET: Wishlist/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Wishlist/Delete/5
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
