namespace StyleZX.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên Đăng Nhập")]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Mật Khẩu")]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ Tên")]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Điện Thoại")]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa Chỉ")]
        public string Address { get; set; }

        [Required]
        [StringLength(25)]
        [Display(Name = "Vai Trò")]
        public string Role { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime UpdatedAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
