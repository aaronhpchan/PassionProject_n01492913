using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_n01492913.Models.ViewModels
{
    public class ProductsWishlist
    {
        public WishlistDto SelectedWishlist { get; set; }
        public IEnumerable<ProductDto> RelatedProducts { get; set; }
    }
}