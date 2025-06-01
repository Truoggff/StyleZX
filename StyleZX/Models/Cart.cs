namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cart
    {
        [Display(Name = "Mã Giỏ Hàng")]
        public int CartId { get; set; }

        [Display(Name = "Người Dùng")]
        public int UserId { get; set; }

        [Display(Name = "ID Biến Thể ")]
        public int ProductVariantId { get; set; }

        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime? CreatedAt { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }
        public virtual User User { get; set; }
    }
}
