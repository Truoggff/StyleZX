using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleZX.Models.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }

    }
}