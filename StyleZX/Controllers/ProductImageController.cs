using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StyleZX.Models;

namespace StyleZX.Controllers
{
    public class ProductImageController : Controller
    {
        private Model1 db = new Model1();

        // GET: ProductImage
        public ActionResult Index()
        {
            var productImages = db.ProductImages.Include(p => p.Product);
            return View(productImages.ToList());
        }

        // GET: ProductImage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // GET: ProductImage/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name");
            return View();
        }

        // POST: ProductImage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImageId,ProductId,ImageUrl,IsMain,AltText")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                db.ProductImages.Add(productImage);
                db.SaveChanges();
                return RedirectToAction("ProductImages", "Admin");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: ProductImage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase ImageUrl)
        {
            if (ImageUrl != null && ImageUrl.ContentLength > 0)
            {
                var fileName = Path.GetFileName(ImageUrl.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                ImageUrl.SaveAs(path);

                ViewBag.Message = "Upload thành công!";
            }
            else
            {
                ViewBag.Message = "Vui lòng chọn tệp ảnh!";
            }

            return View();
        }

        // POST: ProductImage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductImage model, HttpPostedFileBase ImageUrl, string OldImageUrl)
        {
            if (ImageUrl != null && ImageUrl.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageUrl.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                ImageUrl.SaveAs(path);
                model.ImageUrl = fileName;
            }
            else
            {
                model.ImageUrl = OldImageUrl; // nếu không đổi ảnh thì giữ ảnh cũ
            }

            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProductImages", "Admin");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", model.ProductId);
            return View(model);
        }


        // GET: ProductImage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: ProductImage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductImage productImage = db.ProductImages.Find(id);
            db.ProductImages.Remove(productImage);
            db.SaveChanges();
            return RedirectToAction("ProductImages", "Admin");
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
