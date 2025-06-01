using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StyleZX.Models;
using PagedList;
using PagedList.Mvc;
using System.Drawing.Printing;
using StyleZX.Models.ViewModel;

namespace StyleZX.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private Model1 db = new Model1();
        private const int PageSize = 10;
        public ActionResult Profile()
        {
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
        public ActionResult Dashboards()
        {
            var totalUsers = db.Users.Count();
            var totalProducts = db.Products.Count();

            var recentOrders = db.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                RecentOrders = recentOrders
            };

            return View(viewModel);
        }



        public AdminController()
        {
            db = new Model1(); 
        }


        // GET: Admin/UserManagement
        public ActionResult Users(string searchString, int? page)
        {
            var users = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.PasswordHash.Contains(searchString)||
                    u.FullName.Contains(searchString)||
                    u.Email.Contains(searchString) ||
                    u.PhoneNumber.Contains(searchString)||
                    u.Role.Contains(searchString)||
                    u.CreatedAt.ToString().Contains(searchString));
            }

            var paginatedUsers = users
                .OrderBy(u => u.UserName)
                .ToPagedList(page ?? 1, PageSize);

            var model = new UserManagementViewModel
            {
                Users = paginatedUsers,
                SearchString = searchString
            };

            return View(model);
        }

        public ActionResult Categories(string searchString, int? page)
        {
            var category = db.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                category = category.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

            var paginatedCategories = category
                .OrderBy(u => u.CategoryId)
                .ToPagedList(page ?? 1, PageSize);

            var model = new CategoryManagementViewModel
            {
                Categories = paginatedCategories,
                SearchString = searchString
            };

            return View(model);
        }

        public ActionResult ProductImages(string searchString, int? page)
        {
            var image = db.ProductImages.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                image = image.Where(i =>
                    i.ProductId.ToString().Contains(searchString) ||
                    i.ImageUrl.Contains(searchString) ||
                    i.IsMain.ToString().Contains(searchString)||
                    i.AltText.Contains(searchString));
            }

            var paginatedProductImage = image
                .OrderBy(u => u.ImageId)
                .ToPagedList(page ?? 1, PageSize);

            var model = new ProductImagesManagementViewModel
            {
                ProductImages = paginatedProductImage,
                SearchString = searchString
            };

            return View(model);
        }

        // GET: Admin/ProductManagement
        public ActionResult Products(int? page, int? categoryId)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            var model = products
                .OrderBy(p => p.Name)
                .ToPagedList(page ?? 1, PageSize);

            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "Name");
            return View(model);
        }


        public ActionResult ProductVariants(string searchString, int? page, int? productId, int? colorId, int? sizeId)
        {
            ViewBag.Products = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.Colors = new SelectList(db.ProductColors, "ColorId", "ColorName");
            ViewBag.Sizes = new SelectList(db.ProductSizes, "SizeId", "NameSize");

            var variants = db.ProductVariants
                .Include(pv => pv.Product)
                .Include(pv => pv.ProductColor)
                .Include(pv => pv.ProductSize)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                variants = variants.Where(v =>
                    v.Product.Name.Contains(searchString) ||
                    v.ProductColor.ColorName.Contains(searchString) ||
                    v.ProductSize.NameSize.Contains(searchString) ||
                    v.Quantity.ToString().Contains(searchString));
            }

            if (productId.HasValue)
            {
                variants = variants.Where(v => v.ProductId == productId.Value);
            }

            if (colorId.HasValue)
            {
                variants = variants.Where(v => v.ColorId == colorId.Value);
            }

            if (sizeId.HasValue)
            {
                variants = variants.Where(v => v.SizeId == sizeId.Value);
            }

            int pageSize = PageSize;
            int pageNumber = (page ?? 1);

            var model = variants
                .OrderBy(v => v.ProductVariantId)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        public ActionResult AddProduct()
        {
            var model = new ProductViewModel();
            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "Name");
            return View(model);
        }

        // POST: Admin/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                db.Products.Add(product);
                db.SaveChanges();

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/ProductImages"), fileName);
                    imageFile.SaveAs(path);

                    var productImage = new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = "/Content/ProductImages/" + fileName,
                        IsMain = true,
                        AltText = model.Name
                    };

                    db.ProductImages.Add(productImage);
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công!";
                return RedirectToAction("ProductManagement");
            }

            ViewBag.Categories = new SelectList(db.Categories, "CategoryId", "Name");
            return View(model);
        }

        public ActionResult OrderManagement(int? page, string status)
        {
            var orders = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            var model = orders
                .OrderByDescending(o => o.OrderDate)
                .ToPagedList(page ?? 1, PageSize);

            ViewBag.Statuses = new SelectList(new List<string> { "Chờ Xác nhận", "Đang xử lý", "Đang giao", "Đã giao", "Hủy đơn" });
            return View(model);
        }

        // GET: Admin/OrderDetails/{id}
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant.Product))
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant.ProductColor))  
                .Include(o => o.OrderDetails.Select(od => od.ProductVariant.ProductSize))   
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            }
            return RedirectToAction("OrderManagement");
        }


        public ActionResult Reports()
        {
            var currentYear = DateTime.Now.Year;

            // Doanh thu theo tháng
            var revenueByMonth = db.Orders
                .Where(o => o.OrderDate.Year == currentYear && o.Status == "Đã giao")
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .ToList();

            // Sản phẩm bán chạy nhất
            var bestSellingProducts = db.OrderDetails
                .GroupBy(od => od.ProductVariant.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(5)
                .ToList();

            // ViewModel cho báo cáo
            var reportViewModel = new ReportViewModel
            {
                Year = currentYear,
                MonthlyRevenue = revenueByMonth.Select(m => new MonthlyRevenue
                {
                    Month = m.Month,
                    TotalRevenue = m.TotalRevenue,
                    OrderCount = m.OrderCount
                }).ToList(),

                TopSellingProducts = bestSellingProducts.Select(p => new ProductSales
                {
                    ProductName = p.ProductName,
                    QuantitySold = p.TotalQuantity
                }).ToList()
            };

            return View(reportViewModel);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SearchAll(string keyword, int userPage = 1, int categoryPage = 1, int productPage = 1, int orderPage = 1)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                var emptyModel = new AdminSearchViewModel
                {
                    Keyword = "",
                    Users = new List<User>().ToPagedList(userPage, 3),
                    Categories = new List<Category>().ToPagedList(categoryPage, 3),
                    Products = new List<Product>().ToPagedList(productPage, 3),
                    Orders = new List<Order>().ToPagedList(orderPage, 3)
                };
                return View(emptyModel);
            }

            keyword = keyword.ToLower();

            var model = new AdminSearchViewModel
            {
                Keyword = keyword,

                Users = db.Users
                    .Where(u => u.UserName.ToLower().Contains(keyword) || u.FullName.ToLower().Contains(keyword))
                    .OrderBy(u => u.UserName)
                    .ToPagedList(userPage, 3),

                Categories = db.Categories
                    .Where(c => c.Name.ToLower().Contains(keyword) || (c.Description != null && c.Description.ToLower().Contains(keyword)))
                    .OrderBy(c => c.Name)
                    .ToPagedList(categoryPage, 3),

                Products = db.Products
                    .Where(p => p.Name.ToLower().Contains(keyword) || (p.Description != null && p.Description.ToLower().Contains(keyword)))
                    .OrderBy(p => p.Name)
                    .ToPagedList(productPage, 3),

                Orders = db.Orders
                    .Where(o => o.OrderId.ToString().Contains(keyword) || (o.User != null && o.User.UserName.ToLower().Contains(keyword)))
                    .OrderByDescending(o => o.OrderDate)
                    .ToPagedList(orderPage, 3),
            };

            return View(model);
        }


    }

    // Updated View Models to match the database schema

    public class UserManagementViewModel
    {
        public IPagedList<User> Users { get; set; }
        public string SearchString { get; set; }
    }
    public class CategoryManagementViewModel
    {
        public IPagedList<Category> Categories { get; set; }
        public string SearchString { get; set; }
    }

    public class ProductImagesManagementViewModel
    {
        public IPagedList<ProductImage> ProductImages { get; set; }
        public string SearchString { get; set; }
    }
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Vai trò")]
        public string SelectedRole { get; set; }

        public string CurrentRole { get; set; }
    }

    public class ProductViewModel
    {
        [Required]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }
    }



}
