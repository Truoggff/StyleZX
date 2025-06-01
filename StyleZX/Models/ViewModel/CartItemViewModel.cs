using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleZX.Models.ViewModel
{
    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public List<string> AvailableColors { get; set; }
        public List<string> AvailableSizes { get; set; }
        public int ProductVariantId { get; set; }
    }
}