using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_n01492913.Models.ViewModels
{
    public class DetailsProduct
    {
        //this viewmodel stores info to present to /Product/Details/{id}

        //existing product information
        public ProductDto SelectedProduct { get; set; }

        //all wishlists for select
        public IEnumerable<WishlistDto> WishlistOptions { get; set; }
    }
}