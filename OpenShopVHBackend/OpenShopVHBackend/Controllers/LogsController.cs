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
    public class LogsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Logs
        public ActionResult Index()
        {
            return View(db.Logs.ToList());
        }

        // GET: Logs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogEntry logEntry = db.Logs.Find(id);
            if (logEntry == null)
            {
                return HttpNotFound();
            }
            return View(logEntry);
        }

        // GET: Logs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Logs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Username,Level,Message,Exception,Logger,CallSite,ServerName,Port,Url,RemoteAddress")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
                db.Logs.Add(logEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(logEntry);
        }

        // GET: Logs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogEntry logEntry = db.Logs.Find(id);
            if (logEntry == null)
            {
                return HttpNotFound();
            }
            return View(logEntry);
        }

        // POST: Logs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Username,Level,Message,Exception,Logger,CallSite,ServerName,Port,Url,RemoteAddress")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(logEntry);
        }

        // GET: Logs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogEntry logEntry = db.Logs.Find(id);
            if (logEntry == null)
            {
                return HttpNotFound();
            }
            return View(logEntry);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LogEntry logEntry = db.Logs.Find(id);
            db.Logs.Remove(logEntry);
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
