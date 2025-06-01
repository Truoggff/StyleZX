using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StyleZX.Models.ViewModel
{
    public class CheckoutViewModel
    {
        [Display(Name = "Sản phẩm trong giỏ hàng")]
        public List<CartItemViewModel> CartItems { get; set; }

        [Display(Name = "Tổng Tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Họ Tên")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa Chỉ Giao Hàng")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Phương Thức Thanh Toán")]
        public string PaymentMethod { get; set; }
    }
}
