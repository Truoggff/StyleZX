namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductColor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductColor()
        {
            ProductVariants = new HashSet<ProductVariant>();
        }

        [Key]
        [Display(Name = "Mã Màu")]
        public int ColorId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên Màu")]
        public string ColorName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Display(Name = "Biến Thể Sản Phẩm")]
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
