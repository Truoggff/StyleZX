namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProductVariant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductVariant()
        {
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public int ProductVariantId { get; set; }

        [Required]
        [Display(Name = "Mã Sản Phẩm")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Mã Màu")]
        public int ColorId { get; set; }

        [Required]
        [Display(Name = "Mã Kích Cỡ")]
        public int SizeId { get; set; }

        [Required]
        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ProductColor ProductColor { get; set; }

        public virtual Product Product { get; set; }

        public virtual ProductSize ProductSize { get; set; }
    }
}
