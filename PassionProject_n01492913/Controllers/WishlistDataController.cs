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
using System.Diagnostics;

namespace PassionProject_n01492913.Controllers
{
    public class WishlistDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/WishlistData/ListWishlists
        [HttpGet]
        public IEnumerable<WishlistDto> ListWishlists()
        {
            List<Wishlist> Wishlists = db.Wishlists.ToList();
            List<WishlistDto> WishlistDtos = new List<WishlistDto>();

            Wishlists.ForEach(a => WishlistDtos.Add(new WishlistDto(){
                WishlistID = a.WishlistID,
                WishlistName = a.WishlistName
            }));
            
            return WishlistDtos;
        }

        // GET: api/WishlistData/FindWishlist/5
        [HttpGet]
        [ResponseType(typeof(Wishlist))]
        public IHttpActionResult FindWishlist(int id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            return Ok(wishlist);
        }

        // POST: api/WishlistData/UpdateWishlist/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateWishlist(int id, Wishlist wishlist)
        {
            Debug.WriteLine("Reached Update Wishlist Method");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is Invalid");
                return BadRequest(ModelState);
            }

            if (id != wishlist.WishlistID)
            {
                Debug.WriteLine("ID Mismatch");
                Debug.WriteLine("GET parameter" + id);
                Debug.WriteLine("POST parameter" + wishlist.WishlistID);
                return BadRequest();
            }

            db.Entry(wishlist).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishlistExists(id))
                {
                    Debug.WriteLine("Wishlist Not Found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("None of the Conditions Triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WishlistData/AddWishlist
        [ResponseType(typeof(Wishlist))]
        [HttpPost]
        public IHttpActionResult AddWishlist(Wishlist wishlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Wishlists.Add(wishlist);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = wishlist.WishlistID }, wishlist);
        }

        // POST: api/WishlistData/DeleteWishlist/5
        [ResponseType(typeof(Wishlist))]
        [HttpPost]
        public IHttpActionResult DeleteWishlist(int id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            db.Wishlists.Remove(wishlist);
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

        private bool WishlistExists(int id)
        {
            return db.Wishlists.Count(e => e.WishlistID == id) > 0;
        }
    }
}