namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductSize
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductSize()
        {
            ProductVariants = new HashSet<ProductVariant>();
        }

        [Key]
        [Display(Name = "Mã Kích Cỡ")]
        public int SizeId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên Kích Cỡ")]
        public string NameSize { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Display(Name = "Biến Thể Sản Phẩm")]
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
