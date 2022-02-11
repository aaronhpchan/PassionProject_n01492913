using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PassionProject_n01492913.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistID { get; set; }
        public string WishlistName { get; set; }

        //a wishlist can have many products
        public ICollection<Product> Products { get; set; }
    }

    public class WishlistDto
    {
        public int WishlistID { get; set; }
        public string WishlistName { get; set; }
    }
}