using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity; 
using StyleZX.Models;
using StyleZX.Models.ViewModel;

namespace StyleZX.Controllers
{
    public class HomeController : Controller
    {
        private readonly Model1 _context = new Model1();

        public class ProductViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public int CategoryId { get; set; } 
        }

        public class CategoryWithProductsViewModel
        {
            public Category Category { get; set; }
            public ProductImage ProductImage { get; set; }
            public List<ProductViewModel> Products { get; set; }
        }

        public ActionResult Index(string sortBy = "newest")
        {
            UpdateCartCount();

            var categories = _context.Categories
                .Include(c => c.Products.Select(p => p.ProductImages))
                .ToList();

            var viewModel = categories.Select(c => new CategoryWithProductsViewModel
            {
                Category = c,
                Products = c.Products
                    .Select(p => new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ProductImages
                            .Where(img => img.IsMain.HasValue && img.IsMain.Value)
                            .Select(img => img.ImageUrl)
                            .FirstOrDefault()
                            ?? p.ProductImages
                                .Select(img => img.ImageUrl)
                                .FirstOrDefault()
                    })
                    .OrderBy(p => sortBy == "price_asc" ? p.Price : 0)
                    .ThenByDescending(p => sortBy == "price_desc" ? p.Price : 0)
                    .ThenByDescending(p => sortBy == "newest" ? p.ProductId : 0)
                    .Take(8)
                    .ToList()
            }).ToList();

            ViewBag.SortBy = sortBy;
            return View(viewModel);
        }



        public ActionResult HotTrend(string sortBy = "newest")
        {
            UpdateCartCount();

            var model = GetCategoryProducts(15, 18, sortBy);
            ViewBag.SortBy = sortBy;
            return View(model);
        }

        public ActionResult XuanHe(string sortBy = "newest")
        {
            UpdateCartCount();
            var model = GetCategoryProducts(1, 4, sortBy);
            ViewBag.SortBy = sortBy;
            return View(model);
        }

        public ActionResult ThuDong(string sortBy = "newest")
        {
            UpdateCartCount();
            var model = GetCategoryProducts(5, 7, sortBy);
            ViewBag.SortBy = sortBy;
            return View(model);
        }

        public ActionResult Quan(string sortBy = "newest")
        {
            UpdateCartCount();
            var model = GetCategoryProducts(8, 11, sortBy);
            ViewBag.SortBy = sortBy;
            return View(model);
        }

        public ActionResult PhuKien(string sortBy = "newest")
        {
            UpdateCartCount();
            var model = GetCategoryProducts(12, 14, sortBy);
            ViewBag.SortBy = sortBy;
            return View(model);
        }
        private List<CategoryWithProductsViewModel> GetCategoryProducts(int startCategoryId, int endCategoryId, string sortBy)
        {
            return _context.Categories
                .Where(c => c.CategoryId >= startCategoryId && c.CategoryId <= endCategoryId)
                .Select(c => new CategoryWithProductsViewModel
                {
                    Category = c,
                    Products = c.Products
                        .Select(p => new ProductViewModel
                        {
                            ProductId = p.ProductId,
                            ProductName = p.Name,
                            Price = p.Price,
                            ImageUrl = p.ProductImages
                                .OrderByDescending(img => img.IsMain ?? false)
                                .Select(img => img.ImageUrl)
                                .FirstOrDefault(),
                        })
                        .OrderBy(p => sortBy == "price_asc" ? p.Price : 0)
                        .ThenByDescending(p => sortBy == "price_desc" ? p.Price : 0)
                        .ThenByDescending(p => sortBy == "newest" ? p.ProductId : 0)
                        .ToList()
                })
                .ToList();
        }
        public ActionResult TinTuc()
        {
            ViewBag.Message = "Your contact page.";
            UpdateCartCount();
            return View();

        }
        private void UpdateCartCount()
        {
            var userId = Session["UserId"]?.ToString();
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                ViewBag.cartCount = _context.Carts
                    .Where(c => c.UserId == userIdInt)
                    .Sum(c => (int?)c.Quantity) ?? 0;
            }
            else
            {
                ViewBag.cartCount = 0;
            }
        }
        public ActionResult Search(string query, string sortBy = "newest")
        {
            var results = _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .Select(p => new ProductSearchResultViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ProductImages.FirstOrDefault().ImageUrl
                });

            // Sắp xếp
            switch (sortBy)
            {
                case "price_asc":
                    results = results.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    results = results.OrderByDescending(p => p.Price);
                    break;
                default:
                    results = results.OrderByDescending(p => p.ProductId); // newest
                    break;
            }

            ViewBag.SortBy = sortBy;
            ViewBag.Query = query;
            return View(results.ToList());
        }
        public ActionResult SanPhamTheoDanhMuc(int categoryId, string sortBy = "newest")
        {
            UpdateCartCount();

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return HttpNotFound();
            }

            var productsQuery = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId ?? 0,
                    ImageUrl = p.ProductImages
                        .Where(i => i.IsMain.HasValue && i.IsMain.Value)
                        .OrderByDescending(i => i.IsMain.Value)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                });

            switch (sortBy.ToLower())
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case "newest":
                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.ProductId);
                    break;
            }

            var model = new CategoryWithProductsViewModel
            {
                Category = category,
                Products = productsQuery.ToList()
            };

            ViewBag.SortBy = sortBy;
            ViewBag.CategoryId = categoryId;

            return View("SanPhamTheoDanhMuc", model);
        }

        public ActionResult ChonSize()
        {
            return View();
        }





    }

}
