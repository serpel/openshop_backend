using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenShopVHBackend.Models;

namespace OpenShopVHBackend.Controllers
{
    public class FeesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Fees
        public ActionResult Index()
        {
            var fees = db.Fees.Include(f => f.DeviceUser);
            return View(fees.ToList());
        }

        // GET: Fees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fees fees = db.Fees.Find(id);
            if (fees == null)
            {
                return HttpNotFound();
            }
            return View(fees);
        }

        // GET: Fees/Create
        public ActionResult Create()
        {
            ViewBag.DeviceUserId = new SelectList(db.DeviceUser, "DeviceUserId", "Name");
            return View();
        }

        // POST: Fees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeesId,DeviceUserId,Date,Amount")] Fees fees)
        {
            if (ModelState.IsValid)
            {
                db.Fees.Add(fees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeviceUserId = new SelectList(db.DeviceUser, "DeviceUserId", "Name", fees.DeviceUserId);
            return View(fees);
        }

        // GET: Fees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fees fees = db.Fees.Find(id);
            if (fees == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceUserId = new SelectList(db.DeviceUser, "DeviceUserId", "Name", fees.DeviceUserId);
            return View(fees);
        }

        // POST: Fees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeesId,DeviceUserId,Date,Amount")] Fees fees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeviceUserId = new SelectList(db.DeviceUser, "DeviceUserId", "Name", fees.DeviceUserId);
            return View(fees);
        }

        // GET: Fees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fees fees = db.Fees.Find(id);
            if (fees == null)
            {
                return HttpNotFound();
            }
            return View(fees);
        }

        // POST: Fees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fees fees = db.Fees.Find(id);
            db.Fees.Remove(fees);
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
