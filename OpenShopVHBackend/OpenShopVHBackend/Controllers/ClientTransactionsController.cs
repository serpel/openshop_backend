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
    public class ClientTransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ClientTransactions
        public ActionResult Index()
        {
            return View(db.ClientTransactions.ToList());
        }

        // GET: ClientTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTransactions clientTransactions = db.ClientTransactions.Find(id);
            if (clientTransactions == null)
            {
                return HttpNotFound();
            }
            return View(clientTransactions);
        }

        // GET: ClientTransactions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientTransactionId,ReferenceNumber,CardCode,Description,Amount,CreatedDate")] ClientTransactions clientTransactions)
        {
            if (ModelState.IsValid)
            {
                db.ClientTransactions.Add(clientTransactions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientTransactions);
        }

        // GET: ClientTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTransactions clientTransactions = db.ClientTransactions.Find(id);
            if (clientTransactions == null)
            {
                return HttpNotFound();
            }
            return View(clientTransactions);
        }

        // POST: ClientTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientTransactionId,ReferenceNumber,CardCode,Description,Amount,CreatedDate")] ClientTransactions clientTransactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientTransactions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientTransactions);
        }

        // GET: ClientTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientTransactions clientTransactions = db.ClientTransactions.Find(id);
            if (clientTransactions == null)
            {
                return HttpNotFound();
            }
            return View(clientTransactions);
        }

        // POST: ClientTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientTransactions clientTransactions = db.ClientTransactions.Find(id);
            db.ClientTransactions.Remove(clientTransactions);
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
