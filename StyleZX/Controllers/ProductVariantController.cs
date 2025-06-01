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
    public class ProductVariantController : Controller
    {
        private Model1 db = new Model1();

        // GET: ProductVariant
        public ActionResult Index()
        {
            var productVariants = db.ProductVariants.Include(p => p.Product).Include(p => p.ProductColor).Include(p => p.ProductSize);
            return View(productVariants.ToList());
        }

        // GET: ProductVariant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductVariant productVariant = db.ProductVariants.Find(id);
            if (productVariant == null)
            {
                return HttpNotFound();
            }
            return View(productVariant);
        }

        // GET: ProductVariant/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName");
            ViewBag.SizeId = new SelectList(db.ProductSizes, "SizeId", "NameSize");
            return View();
        }

        // POST: ProductVariant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductVariantId,ProductId,ColorId,SizeId,Quantity")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                db.ProductVariants.Add(productVariant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productVariant.ProductId);
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName", productVariant.ColorId);
            ViewBag.SizeId = new SelectList(db.ProductSizes, "SizeId", "NameSize", productVariant.SizeId);
            return View(productVariant);
        }

        // GET: ProductVariant/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductVariant productVariant = db.ProductVariants.Find(id);
            if (productVariant == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productVariant.ProductId);
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName", productVariant.ColorId);
            ViewBag.SizeId = new SelectList(db.ProductSizes, "SizeId", "NameSize", productVariant.SizeId);
            return View(productVariant);
        }

        // POST: ProductVariant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductVariantId,ProductId,ColorId,SizeId,Quantity")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productVariant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProductVariants","Admin");
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productVariant.ProductId);
            ViewBag.ColorId = new SelectList(db.ProductColors, "ColorId", "ColorName", productVariant.ColorId);
            ViewBag.SizeId = new SelectList(db.ProductSizes, "SizeId", "NameSize", productVariant.SizeId);
            return View(productVariant);
        }

        // GET: ProductVariant/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductVariant productVariant = db.ProductVariants.Find(id);
            if (productVariant == null)
            {
                return HttpNotFound();
            }
            return View(productVariant);
        }

        // POST: ProductVariant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductVariant productVariant = db.ProductVariants.Find(id);
            db.ProductVariants.Remove(productVariant);
            db.SaveChanges();
            return RedirectToAction("ProductVariants","Admin");
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
