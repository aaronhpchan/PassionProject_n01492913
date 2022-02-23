using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_n01492913.Models.ViewModels
{
    public class DetailsCategory
    {
        public CategoryDto SelectedCategory { get; set; }
        public IEnumerable<ProductDto> CategorizedProducts { get; set; }
    }
}