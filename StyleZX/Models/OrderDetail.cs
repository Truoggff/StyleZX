namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetail
    {
        [Display(Name = "Mã Chi Tiết Đơn Hàng")]
        public int OrderDetailId { get; set; }

        [Display(Name = "Mã Đơn Hàng")]
        public int OrderId { get; set; }

        [Display(Name = "Id Biến Thể")]
        public int ProductVariantId { get; set; }

        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn Hàng")]
        public virtual Order Order { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}
