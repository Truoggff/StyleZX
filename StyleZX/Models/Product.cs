namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
            ProductVariants = new HashSet<ProductVariant>();
        }

        [Display(Name = "Mã Sản Phẩm")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên Sản Phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô Tả")]
        public string Description { get; set; }

        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Mã Danh Mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Danh Mục")]
        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Display(Name = "Danh Sách Ảnh Sản Phẩm")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Display(Name = "Biến Thể Sản Phẩm")]
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
