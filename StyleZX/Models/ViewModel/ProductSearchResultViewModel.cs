using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleZX.Models.ViewModel
{
    public class ProductSearchResultViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
    }

}