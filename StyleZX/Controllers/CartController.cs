using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StyleZX.Models;
using StyleZX.Models.ViewModel;
namespace StyleZX.Controllers
{
    public class CartController : Controller
    {
        private Model1 db = new Model1();



        [HttpPost]
        public ActionResult AddToCart(int productId, string color, string size, int quantity)
        {
            try
            {
                var userId = Session["UserId"]?.ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm vào giỏ hàng" });
                }

                int userIdInt;
                if (!int.TryParse(userId, out userIdInt))
                {
                    return Json(new { success = false, message = "ID người dùng không hợp lệ" });
                }

                var productVariant = db.ProductVariants
                    .Include(pv => pv.ProductColor)
                    .Include(pv => pv.ProductSize)
                    .FirstOrDefault(pv => pv.ProductId == productId
                                      && pv.ProductColor.ColorName == color
                                      && pv.ProductSize.NameSize == size);

                if (productVariant == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm" });
                }

                if (quantity > productVariant.Quantity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Số lượng vượt quá tồn kho. Chỉ còn {productVariant.Quantity} sản phẩm"
                    });
                }

                var existingCartItem = db.Carts
                    .FirstOrDefault(c => c.UserId == userIdInt
                                      && c.ProductVariantId == productVariant.ProductVariantId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                    if (existingCartItem.Quantity > productVariant.Quantity)
                    {
                        existingCartItem.Quantity = productVariant.Quantity;
                    }
                }
                else
                {
                    var newCartItem = new Cart
                    {
                        UserId = userIdInt,
                        ProductVariantId = productVariant.ProductVariantId,
                        Quantity = quantity,
                        CreatedAt = DateTime.Now
                    };
                    db.Carts.Add(newCartItem);
                }

                db.SaveChanges();
                LoadCartToSession(userIdInt);
                var cartCount = db.Carts
                    .Where(c => c.UserId == userIdInt)
                    .Sum(c => (int?)c.Quantity) ?? 0;

                return Json(new
                {
                    success = true,
                    message = "Thêm vào giỏ hàng thành công",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi khi thêm vào giỏ hàng: " + ex.Message
                });
            }
        }

        public ActionResult ListCart()
        {
            var userId = Session["UserId"]?.ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            int userIdInt;
            if (!int.TryParse(userId, out userIdInt))
            {
                return RedirectToAction("Index", "Home");
            }

            var cartItems = db.Carts
                .Include(c => c.ProductVariant)
                .Include(c => c.ProductVariant.Product)
                .Include(c => c.ProductVariant.ProductColor)
                .Include(c => c.ProductVariant.ProductSize)
                .Where(c => c.UserId == userIdInt)
                .Select(c => new CartItemViewModel
                {
                    CartId = c.CartId,
                    ProductId = c.ProductVariant.ProductId,
                    ProductName = c.ProductVariant.Product.Name,
                    ImageUrl = c.ProductVariant.Product.ProductImages.FirstOrDefault().ImageUrl,
                    Color = c.ProductVariant.ProductColor.ColorName,
                    Size = c.ProductVariant.ProductSize.NameSize,
                    Price = c.ProductVariant.Product.Price,
                    Quantity = c.Quantity,
                    TotalPrice = c.ProductVariant.Product.Price * c.Quantity,
                    ProductVariantId = c.ProductVariantId,
                    AvailableColors = db.ProductVariants
                        .Where(pv => pv.ProductId == c.ProductVariant.ProductId)
                        .Select(pv => pv.ProductColor.ColorName)
                        .Distinct()
                        .ToList(),
                    AvailableSizes = db.ProductVariants
                        .Where(pv => pv.ProductId == c.ProductVariant.ProductId
                                  && pv.ProductColor.ColorName == c.ProductVariant.ProductColor.ColorName)
                        .Select(pv => pv.ProductSize.NameSize)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            ViewBag.CartCount = cartItems.Sum(c => c.Quantity);
            ViewBag.TotalAmount = cartItems.Sum(item => item.TotalPrice);

            return View(cartItems);
        }
        private void LoadCartToSession(int userId)
        {
            var cartItems = db.Carts
                .Include(c => c.ProductVariant.Product)
                .Include(c => c.ProductVariant.ProductColor)
                .Include(c => c.ProductVariant.ProductSize)
                .Where(c => c.UserId == userId)
                .Select(c => new CartItemViewModel
                {
                    CartId = c.CartId,
                    ProductId = c.ProductVariant.ProductId,
                    ProductName = c.ProductVariant.Product.Name,
                    ImageUrl = c.ProductVariant.Product.ProductImages.FirstOrDefault().ImageUrl,
                    Color = c.ProductVariant.ProductColor.ColorName,
                    Size = c.ProductVariant.ProductSize.NameSize,
                    Price = c.ProductVariant.Product.Price,
                    Quantity = c.Quantity,
                    TotalPrice = c.ProductVariant.Product.Price * c.Quantity,
                    ProductVariantId = c.ProductVariantId,
                    AvailableColors = db.ProductVariants
                        .Where(pv => pv.ProductId == c.ProductVariant.ProductId)
                        .Select(pv => pv.ProductColor.ColorName)
                        .Distinct()
                        .ToList(),
                    AvailableSizes = db.ProductVariants
                        .Where(pv => pv.ProductId == c.ProductVariant.ProductId
                                  && pv.ProductColor.ColorName == c.ProductVariant.ProductColor.ColorName)
                        .Select(pv => pv.ProductSize.NameSize)
                        .Distinct()
                        .ToList()
                }).ToList();

            Session["Cart"] = cartItems;
        }


        [HttpPost]
        public ActionResult UpdateVariant(int cartId, int productId, string variantType, string newValue)
        {

            try
            {
                var userId = Session["UserId"]?.ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var cartItem = db.Carts
                    .Include(c => c.ProductVariant)
                    .Include(c => c.ProductVariant.ProductColor)
                    .Include(c => c.ProductVariant.ProductSize)
                    .FirstOrDefault(c => c.CartId == cartId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
                }

                if (cartItem.UserId.ToString() != userId)
                {
                    return Json(new { success = false, message = "Bạn không có quyền thay đổi sản phẩm này" });
                }

                ProductVariant newVariant = null;

                if (variantType == "color")
                {
                    newVariant = db.ProductVariants
                        .Include(pv => pv.ProductColor)
                        .Include(pv => pv.ProductSize)
                        .FirstOrDefault(pv => pv.ProductId == productId
                                          && pv.ProductColor.ColorName == newValue
                                          && pv.ProductSize.NameSize == cartItem.ProductVariant.ProductSize.NameSize);
                }
                else if (variantType == "size")
                {
                    newVariant = db.ProductVariants
                        .Include(pv => pv.ProductColor)
                        .Include(pv => pv.ProductSize)
                        .FirstOrDefault(pv => pv.ProductId == productId
                                          && pv.ProductColor.ColorName == cartItem.ProductVariant.ProductColor.ColorName
                                          && pv.ProductSize.NameSize == newValue);
                }

                if (newVariant == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm phù hợp" });
                }

                if (cartItem.Quantity > newVariant.Quantity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Biến thể mới chỉ còn {newVariant.Quantity} sản phẩm. Vui lòng giảm số lượng trước khi chuyển đổi."
                    });
                }

                cartItem.ProductVariantId = newVariant.ProductVariantId;
                db.SaveChanges();
                LoadCartToSession(cartItem.UserId);

                var availableSizes = variantType == "color"
                    ? db.ProductVariants
                        .Where(pv => pv.ProductId == productId && pv.ProductColor.ColorName == newValue)
                        .Select(pv => pv.ProductSize.NameSize)
                        .Distinct()
                        .ToList()
                    : null;

                return Json(new
                {
                    success = true,
                    availableSizes = availableSizes
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
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
        [HttpPost]
        public ActionResult UpdateQuantity(int cartId, int quantity)
        {

            try
            {
                if (quantity < 1)
                    return Json(new { success = false, message = "Số lượng phải ≥ 1" });

                var cartItem = db.Carts
                    .Include(c => c.ProductVariant)
                    .FirstOrDefault(c => c.CartId == cartId);

                if (cartItem == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                if (quantity > cartItem.ProductVariant.Quantity)
                    return Json(new
                    {
                        success = false,
                        message = $"Chỉ còn {cartItem.ProductVariant.Quantity} sản phẩm"
                    });

                cartItem.Quantity = quantity;
                db.SaveChanges();
                LoadCartToSession(cartItem.UserId);
                var total = CalculateCartTotal(cartItem.UserId);

                return Json(new
                {
                    success = true,
                    newQuantity = quantity,
                    totalAmount = total.ToString("N0")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        private decimal CalculateCartTotal(int userId)
        {
            return db.Carts
                .Where(c => c.UserId == userId)
                .Sum(c => (decimal?)c.Quantity * c.ProductVariant.Product.Price) ?? 0;
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int cartId)
        {

            try
            {
                var userId = Session["UserId"]?.ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var cartItem = db.Carts.FirstOrDefault(c => c.CartId == cartId);
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
                }

                if (cartItem.UserId.ToString() != userId)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa sản phẩm này" });
                }

                db.Carts.Remove(cartItem);
                db.SaveChanges();
                LoadCartToSession(int.Parse(userId));

                var cartCount = db.Carts
                    .Where(c => c.UserId.ToString() == userId)
                    .Sum(c => (int?)c.Quantity) ?? 0;

                var totalAmount = CalculateCartTotal(int.Parse(userId));

                return Json(new
                {
                    success = true,
                    cartCount = cartCount,
                    totalAmount = totalAmount.ToString("N0")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }


        // GET: Cart
        public ActionResult Index()
        {
            var carts = db.Carts.Include(c => c.ProductVariant).Include(c => c.User);
            return View(carts.ToList());
        }

        // GET: Cart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.ProductVariantId = new SelectList(db.ProductVariants, "ProductVariantId", "ProductVariantId");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CartId,UserId,ProductVariantId,Quantity,CreatedAt")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductVariantId = new SelectList(db.ProductVariants, "ProductVariantId", "ProductVariantId", cart.ProductVariantId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", cart.UserId);
            return View(cart);
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductVariantId = new SelectList(db.ProductVariants, "ProductVariantId", "ProductVariantId", cart.ProductVariantId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", cart.UserId);
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CartId,UserId,ProductVariantId,Quantity,CreatedAt")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductVariantId = new SelectList(db.ProductVariants, "ProductVariantId", "ProductVariantId", cart.ProductVariantId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", cart.UserId);
            return View(cart);
        }

        // GET: Cart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
