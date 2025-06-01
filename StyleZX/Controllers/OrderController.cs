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
    public class OrderController : Controller
    {
        private Model1 db = new Model1();


        // GET: Order/Checkout
        public ActionResult CheckOut()
        {
            UpdateCartCount();
            var cart = Session["Cart"] as List<CartItemViewModel>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ListCart", "Cart");
            }

            var userId = Session["UserId"] as int?;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                TotalAmount = cart.Sum(x => x.Price * x.Quantity),
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ShippingAddress = user.Address,
                PaymentMethod = "Thanh toán khi nhận hàng"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(CheckoutViewModel model)
        {
            UpdateCartCount();
            var cart = Session["Cart"] as List<CartItemViewModel>;
            var userId = Session["UserId"] as int?;

            if (cart == null || !cart.Any() || userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            CreateOrder(cart, model, userId.Value);
            Session.Remove("Cart");

            return RedirectToAction("Success");
        }
        public ActionResult Success()
        {
            UpdateCartCount();
            ViewBag.Message = "Đơn hàng của bạn đã được đặt thành công!";
            return View();
        }

        public ActionResult OrderNow(int productVariantId)
        {
            var userId = Session["UserId"] as int?;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var variant = db.ProductVariants
                           .Include(v => v.Product)
                           .Include(v => v.ProductColor)
                           .Include(v => v.ProductSize)
                           .FirstOrDefault(v => v.ProductVariantId == productVariantId);

            if (variant == null)
            {
                return HttpNotFound();
            }

            var mainImage = db.ProductImages
                             .FirstOrDefault(img => img.ProductId == variant.ProductId &&
                                                   (img.IsMain ?? false));

            var user = db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = Session["Cart"] as List<CartItemViewModel> ?? new List<CartItemViewModel>();

            var existingItem = cart.FirstOrDefault(x => x.ProductVariantId == productVariantId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItemViewModel
                {
                    ProductVariantId = variant.ProductVariantId,
                    ProductName = variant.Product.Name,
                    ImageUrl = mainImage?.ImageUrl ?? "default-image.jpg",
                    Size = variant.ProductSize.NameSize,
                    Color = variant.ProductColor.ColorName,
                    Quantity = 1,
                    Price = variant.Product.Price
                };
                cart.Add(cartItem);
            }

            Session["Cart"] = cart;

            return RedirectToAction("CheckOut", "Order");
        }



        private void CreateOrder(List<CartItemViewModel> cart, CheckoutViewModel model, int userId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.Sum(x => x.Quantity * x.Price),
                        ShippingAddress = model.ShippingAddress,
                        Status = "Chờ xác nhận",
                        PaymentMethod = model.PaymentMethod
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    foreach (var item in cart)
                    {
                        var variant = db.ProductVariants.Find(item.ProductVariantId);
                        if (variant == null) continue;

                        variant.Quantity -= item.Quantity;
                        if (variant.Quantity < 0)
                        {
                            throw new Exception($"Not enough stock for product variant {item.ProductVariantId}");
                        }

                        var detail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductVariantId = item.ProductVariantId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        };
                        db.OrderDetails.Add(detail);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // GET: Order
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.User);
            return View(orders.ToList());
        }

        // GET: Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,UserId,OrderDate,TotalAmount,ShippingAddress,Status,PaymentMethod")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", order.UserId);
            return View(order);
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", order.UserId);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,UserId,OrderDate,TotalAmount,ShippingAddress,Status,PaymentMethod")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", order.UserId);
            return View(order);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
