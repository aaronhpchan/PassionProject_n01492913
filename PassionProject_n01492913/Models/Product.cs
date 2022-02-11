using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PassionProject_n01492913.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }

        //a product belongs to one category
        //a category can have many products
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }

        //a product can be in many wishlists
        public ICollection<Wishlist> Wishlists { get; set; }
    }

    public class ProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string CategoryName { get; set; }
    }
}