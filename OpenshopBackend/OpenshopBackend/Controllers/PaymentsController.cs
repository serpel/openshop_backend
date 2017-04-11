using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenshopBackend.Models;
using OpenshopBackend.BussinessLogic.SAP;
using Hangfire;

namespace OpenshopBackend.Controllers
{
    public class PaymentsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.Cash).Include(p => p.Client).Include(p => p.Transfer);
            return View(payments.ToList().OrderByDescending(o => o.CreatedDate).Take(20));
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            ViewBag.CashId = new SelectList(db.Cash, "CashId", "GeneralAccount");
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name");
            ViewBag.TransferId = new SelectList(db.Transfers, "TransferId", "ReferenceNumber");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentId,DocEntry,ClientId,CashId,TransferId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CashId = new SelectList(db.Cash, "CashId", "GeneralAccount", payment.CashId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name", payment.ClientId);
            ViewBag.TransferId = new SelectList(db.Transfers, "TransferId", "ReferenceNumber", payment.TransferId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CashId = new SelectList(db.Cash, "CashId", "GeneralAccount", payment.CashId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name", payment.ClientId);
            ViewBag.TransferId = new SelectList(db.Transfers, "TransferId", "ReferenceNumber", payment.TransferId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentId,DocEntry,ClientId,CashId,TransferId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CashId = new SelectList(db.Cash, "CashId", "GeneralAccount", payment.CashId);
            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Name", payment.ClientId);
            ViewBag.TransferId = new SelectList(db.Transfers, "TransferId", "ReferenceNumber", payment.TransferId);
            return View(payment);
        }

        [HttpGet]
        public ActionResult Process(int id)
        {
            BackgroundJob.Enqueue(() => CreatePaymentOnSAP(id));

            return RedirectToAction("Index");
        }

        [AutomaticRetry(Attempts = 0)]
        public void CreatePaymentOnSAP(int paymentId)
        {

            try
            {
                DraftPayment draftPayment = new DraftPayment();
                String message = draftPayment.MakePayment(paymentId);

            }catch(Exception e)
            {
                var payment = db.Payments.Where(w => w.PaymentId == paymentId).ToList().FirstOrDefault();
                
                if(payment != null)
                {
                    payment.LastErrorMessage = payment.LastErrorMessage + " - " + e.Message;
                    db.Entry(payment).State = EntityState.Modified;
                    db.SaveChanges();
                }            
            }
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
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
