namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductImage
    {
        [Key]
        [Display(Name = "Mã Ảnh")]
        public int ImageId { get; set; }

        [Display(Name = "Mã Sản Phẩm")]
        public int? ProductId { get; set; }

        [StringLength(500)]
        [Display(Name = "Đường Dẫn Ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Ảnh Chính")]
        public bool? IsMain { get; set; }

        [StringLength(255)]
        [Display(Name = "Thay Thế")]
        public string AltText { get; set; }

        [Display(Name = "Sản Phẩm")]
        public virtual Product Product { get; set; }
    }
}
