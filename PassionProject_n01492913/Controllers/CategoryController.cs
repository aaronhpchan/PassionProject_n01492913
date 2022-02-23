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
    public class CategoryController : Controller
    {
        private static readonly HttpClient client;
        JavaScriptSerializer jss = new JavaScriptSerializer();

        static CategoryController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44309/api/");
        }

        // GET: Category/List
        public ActionResult List()
        {
            //objective: communicate with category data api to retrieve a list of categories
            //curl https://localhost:44309/api/categorydata/listcategories

            string url = "categorydata/listcategories";
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("Response Code: " + response.StatusCode);

            IEnumerable<CategoryDto> categories = response.Content.ReadAsAsync<IEnumerable<CategoryDto>>().Result;
            Debug.WriteLine("Number of Category Received: " + categories.Count());

            return View(categories);
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with category data api to retrieve a list of categories
            //curl https://localhost:44309/api/categorydata/findcategory/{id}

            DetailsCategory ViewModel = new DetailsCategory();

            string url = "categorydata/findcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("Response Code: " + response.StatusCode);

            CategoryDto SelectedCategory = response.Content.ReadAsAsync<CategoryDto>().Result;
            //Debug.WriteLine("Category Received: " + SelectedCategory.CategoryName);

            ViewModel.SelectedCategory = SelectedCategory;

            //send request to gather info about products related to a particular category ID
            url = "productdata/listproductsforcategory/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ProductDto> CategorizedProducts = response.Content.ReadAsAsync<IEnumerable<ProductDto>>().Result;
            ViewModel.CategorizedProducts = CategorizedProducts;

            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Category/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category category)
        {
            Debug.WriteLine("Inputted Category Name: " + category.CategoryName);

            //objective: add a new category into system using api
            //curl -d @category.json -H "Content-type:application/json" https://localhost:44309/api/categorydata/addcategory
            string url = "categorydata/addcategory";

            string jsonpayload = jss.Serialize(category);

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

        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "categorydata/findcategory/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CategoryDto SelectedCategory = response.Content.ReadAsAsync<CategoryDto>().Result;
            return View(SelectedCategory);
        }

        // POST: Category/Update/5
        [HttpPost]
        public ActionResult Update(int id, Category category)
        {
            string url = "categorydata/updatecategory/" + id;
            string jsonpayload = jss.Serialize(category);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Category/Delete/5
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
