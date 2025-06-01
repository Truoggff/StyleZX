namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Display(Name = "Mã Đơn Hàng")]
        public int OrderId { get; set; }

        [Display(Name = "Mã Người Dùng")]
        public int? UserId { get; set; }

        [Display(Name = "Ngày Đặt Hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng Tiền")]
        public decimal TotalAmount { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa Chỉ Giao")]
        public string ShippingAddress { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng Thái")]
        public string Status { get; set; }

        [StringLength(50)]
        [Display(Name = "Phương Thức Thanh Toán")]
        public string PaymentMethod { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Display(Name = "Chi Tiết Đơn Hàng")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [Display(Name = "Người Dùng")]
        public virtual User User { get; set; }
    }
}
