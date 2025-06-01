using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StyleZX.Models;
namespace StyleZX.Controllers
{
    public class ProductController : Controller
    {
        private Model1 db = new Model1();

        // GET: Product
        public ActionResult GetProductVariantId(int productId, string color, string size)
        {
            var variant = db.ProductVariants.FirstOrDefault(pv =>
                pv.ProductId == productId &&
                pv.ProductColor.ColorName.ToLower() == color.ToLower() &&
                pv.ProductSize.NameSize.ToLower() == size.ToLower());

            if (variant == null)
            {
                return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, productVariantId = variant.ProductVariantId }, JsonRequestBehavior.AllowGet);
        }

        public class ProductDetailViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public List<string> AvailableSizes { get; set; }
            public List<string> AvailableColors { get; set; }
            public List<string> RelatedImages { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public ActionResult DetailProduct(int id)
        {
            UpdateCartCount();
            var productEntity = db.Products.FirstOrDefault(p => p.ProductId == id);

            if (productEntity == null)
            {
                return HttpNotFound();
            }

            var productId = productEntity.ProductId;

            var mainImage = db.ProductImages
                              .Where(pi => pi.ProductId == productId && pi.IsMain == true)
                              .Select(pi => pi.ImageUrl)
                              .FirstOrDefault();

            var relatedImages = db.ProductImages
                                  .Where(pi => pi.ProductId == productId && pi.IsMain != true)
                                  .Select(pi => pi.ImageUrl)
                                  .ToList();

            var availableSizes = (from pv in db.ProductVariants
                                  join s in db.ProductSizes on pv.SizeId equals s.SizeId
                                  where pv.ProductId == productId
                                  select s.NameSize).Distinct().ToList();

            var availableColors = (from pv in db.ProductVariants
                                   join c in db.ProductColors on pv.ColorId equals c.ColorId
                                   where pv.ProductId == productId
                                   select c.ColorName).Distinct().ToList();

            if (string.IsNullOrEmpty(mainImage) && relatedImages.Any())
            {
                mainImage = relatedImages.First();
                relatedImages.RemoveAt(0);
            }

            var product = new ProductDetailViewModel
            {
                ProductId = productEntity.ProductId,
                ProductName = productEntity.Name,
                Price = productEntity.Price,
                Description = productEntity.Description,
                ImageUrl = mainImage,
                RelatedImages = relatedImages,
                AvailableSizes = availableSizes,
                AvailableColors = availableColors
            };

            return View(product);
        }


        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,Name,Description,Price,CategoryId,CreatedAt,UpdatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,Name,Description,Price,CategoryId,CreatedAt,UpdatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                if (ModelState.IsValid)
                {
                    product.UpdatedAt = DateTime.Now;
                }
                db.SaveChanges();
                return RedirectToAction("Products","Admin");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
