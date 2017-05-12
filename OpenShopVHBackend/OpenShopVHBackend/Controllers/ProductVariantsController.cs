using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenShopVHBackend.Models;
using OpenShopVHBackend.BussinessLogic;

namespace OpenShopVHBackend.Controllers
{
    [AccessAuthorizeAttribute(Roles = "Admin, SalesAdmin")]
    public class ProductVariantsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductVariants
        public ActionResult Index()
        {
            var productVariants = db.ProductVariants.Include(p => p.Color).Include(p => p.Product).Include(p => p.Size);
            return View(productVariants.ToList());
        }

        // GET: ProductVariants/Details/5
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

        // GET: ProductVariants/Create
        public ActionResult Create()
        {
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "Value");
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name");
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Value");
            return View();
        }

        // POST: ProductVariants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductVariantId,ProductId,ColorId,SizeId,Code,Quantity,IsCommitted,Price,Currency")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                db.ProductVariants.Add(productVariant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "Value", productVariant.ColorId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productVariant.ProductId);
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Value", productVariant.SizeId);
            return View(productVariant);
        }

        // GET: ProductVariants/Edit/5
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
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "Value", productVariant.ColorId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productVariant.ProductId);
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Value", productVariant.SizeId);
            return View(productVariant);
        }

        // POST: ProductVariants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductVariantId,ProductId,ColorId,SizeId,Code,Quantity,IsCommitted,Price,Currency")] ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productVariant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColorId = new SelectList(db.Colors, "ColorId", "Value", productVariant.ColorId);
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", productVariant.ProductId);
            ViewBag.SizeId = new SelectList(db.Sizes, "SizeId", "Value", productVariant.SizeId);
            return View(productVariant);
        }

        // GET: ProductVariants/Delete/5
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

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductVariant productVariant = db.ProductVariants.Find(id);
            db.ProductVariants.Remove(productVariant);
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
