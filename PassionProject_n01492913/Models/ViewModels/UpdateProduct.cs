using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_n01492913.Models.ViewModels
{
    public class UpdateProduct
    {
        //this viewmodel stores info to present to /Product/Update/{id}

        //existing product information
        public ProductDto SelectedProduct { get; set; }

        //all categories for select
        public IEnumerable<CategoryDto> CategoryOptions { get; set; }
    }
}