using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using StyleZX.Models;
using StyleZX.Models.ViewModel;

namespace StyleZX.Controllers
{
    public class AccountController : Controller
    {
        private Model1 db = new Model1();

        public class LoginViewModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            public string Role { get; set; }
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin đăng nhập.";
                return View(model);
            }

            var user = db.Users.FirstOrDefault(u => u.UserName == model.Username && u.PasswordHash == model.Password);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View(model);
            }

            Session["FullName"] = user.FullName;
            Session["UserName"] = user.UserName;
            Session["UserId"] = user.UserId;
            Session["Role"] = user.Role;

            TempData["SuccessMessage"] = "Đăng nhập thành công!";

            if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboards", "Admin");
            }
            else
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session["FullName"] = null;
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public class RegisterViewModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "Tên đăng nhập phải từ {2} đến {1} ký tự", MinimumLength = 6)]

            [Display(Name = "Tên người dùng")]
            public string UserName { get; set; }

            
            [Required]
            [StringLength(255, ErrorMessage = "Mật khẩu phải ít nhất {2} ký tự", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            
            [DataType(DataType.Password)]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            [Display(Name = "Xác nhận lại")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(100)]
            [Display(Name = "Họ tên")]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            [StringLength(100)]
            public string Email { get; set; }

            [StringLength(20)]
            [Phone]
            [Display(Name = "Điện thoại")]
            public string PhoneNumber { get; set; }

            [StringLength(255)]
            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    PasswordHash = model.Password,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Role = "User",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                using (var context = new Model1())
                {
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                System.Web.Security.FormsAuthentication.SetAuthCookie(user.UserName, false);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        public class ForgotPasswordViewModel
        {
            [Required]
            [StringLength(100)]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [StringLength(100)]
            public string Email { get; set; }

            [Required]
            [Phone]
            [StringLength(20)]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100)]
            public string NewPassword { get; set; }

            [Required]
            [StringLength(100)]
            [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmNewPassword { get; set; }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Model1())
                {
                    var user = await context.Users
                        .FirstOrDefaultAsync(u => u.UserName == model.UserName &&
                                                  u.Email == model.Email &&
                                                  u.PhoneNumber == model.PhoneNumber);

                    if (user != null)
                    {
                        user.PasswordHash = model.NewPassword;
                        user.UpdatedAt = DateTime.Now;

                        await context.SaveChangesAsync();

                        TempData["Message"] = "Mật khẩu đã được thay đổi!";
                        return RedirectToAction("Login");

                    }
                    else
                    {
                        ViewBag.Message = "Thông tin tài khoản không khớp, vui lòng kiểm tra lại!";
                        return View(model);
                    }
                }
            }
            return View(model);
        }

        public ActionResult Profile()
        {
            UpdateCartCount();
            if (Session["FullName"] != null)
            {
                var username = Session["UserName"]?.ToString();
                using (var db = new Model1())
                {
                    var user = db.Users.FirstOrDefault(u => u.UserName == username);
                    if (user != null)
                    {
                        ViewBag.UserName = user.UserName;
                        ViewBag.Password = user.PasswordHash;
                        ViewBag.FullName = user.FullName;
                        ViewBag.Email = user.Email;
                        ViewBag.PhoneNumber = user.PhoneNumber;
                        ViewBag.Address = user.Address;
                        ViewBag.Role = user.Role;
                        ViewBag.CreatedAt = user.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");

                        var orders = db.Orders
                            .Where(o => o.UserId == user.UserId)
                            .OrderByDescending(o => o.OrderDate)
                            .ToList();

                        var orderViewModels = new List<OrderViewModel>();

                        foreach (var order in orders)
                        {
                            var orderDetails = db.OrderDetails
                                .Where(od => od.OrderId == order.OrderId)
                                .Join(db.ProductVariants,
                                    od => od.ProductVariantId,
                                    pv => pv.ProductVariantId,
                                    (od, pv) => new { od, pv })
                                .Join(db.Products,
                                    temp => temp.pv.ProductId,
                                    p => p.ProductId,
                                    (temp, p) => new OrderDetailViewModel
                                    {
                                        ProductName = p.Name,
                                        Price = temp.od.Price,
                                        Quantity = temp.od.Quantity,
                                        Total = temp.od.Price * temp.od.Quantity
                                    })
                                .ToList();

                            orderViewModels.Add(new OrderViewModel
                            {
                                OrderId = order.OrderId,
                                OrderDate = order.OrderDate,
                                TotalAmount = order.TotalAmount,
                                Status = order.Status,
                                ShippingAddress = order.ShippingAddress,
                                PaymentMethod = order.PaymentMethod,
                                OrderDetails = orderDetails
                            });
                        }

                        ViewBag.Orders = orderViewModels;
                    }
                }
                return View();
            }
            else
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem thông tin!";
                return RedirectToAction("Login", "Account");
            }
        }
        public class EditProfileViewModel
        {
            [Required]
            [Display(Name = "Họ tên")]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Điện thoại")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }
        }
        // GET: EditProfile
        [HttpGet]
        public ActionResult EditProfile()
        {
            UpdateCartCount();
            if (Session["FullName"] != null)
            {
                var username = Session["UserName"]?.ToString();
                using (var db = new Model1())
                {
                    var user = db.Users.FirstOrDefault(u => u.UserName == username);
                    if (user != null)
                    {
                        var model = new EditProfileViewModel
                        {
                            FullName = user.FullName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Address = user.Address
                        };
                        return View(model);
                    }
                }
                return HttpNotFound();
            }
            else
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để chỉnh sửa thông tin!";
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileViewModel model)
        {
            UpdateCartCount();
            if (Session["FullName"] != null)
            {
                if (ModelState.IsValid)
                {
                    var username = Session["UserName"]?.ToString();
                    using (var db = new Model1())
                    {
                        var user = db.Users.FirstOrDefault(u => u.UserName == username);
                        if (user != null)
                        {
                            user.FullName = model.FullName;
                            user.Email = model.Email;
                            user.PhoneNumber = model.PhoneNumber;
                            user.Address = model.Address;

                            db.SaveChanges();
                            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                            return RedirectToAction("Profile");
                        }
                    }
                }
                return View(model);
            }
            else
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để chỉnh sửa thông tin!";
                return RedirectToAction("Login", "Account");
            }
        }
        private void UpdateCartCount()
        {
            var userId = Session["UserId"]?.ToString();
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                ViewBag.cartCount = db.Carts
                    .Where(c => c.UserId == userIdInt)
                    .Sum(c => (int?)c.Quantity) ?? 0;
            }
            else
            {
                ViewBag.cartCount = 0;
            }
        }


    }
}
