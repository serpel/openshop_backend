using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenShopVHBackend.Models;
using OpenShopVHBackend.BussinessLogic.SAP;
using Hangfire;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
//using Syncfusion.Reports.EJ;

namespace OpenShopVHBackend.Controllers
{
    public class PaymentsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments
                .Include(p => p.Cash).Include(p => p.Client).Include(p => p.Transfer)
                .OrderByDescending(o => o.CreatedDate)
                .ToList()
                .Select(s => new PaymentViewModel()
                {
                    PaymentId = s.PaymentId,
                    CreatedDate = s.CreatedDate,
                    DocEntry = s.DocEntry,
                    Comment = s.Comment,
                    SalesPersonName = s.DeviceUser.Name,
                    Status = s.Status.ToString(),
                    TotalTransfer = s.Transfer.Amount,
                    Invoices = string.Join(", ", s.Invoices.ToList().Select(a => a.DocumentNumber).ToArray()),
                    LastErrorMessage = s.LastErrorMessage,
                    TotalAmount = s.TotalAmount
                });

            ViewBag.datasource = payments;

            return View(payments.ToList());
        }

        //public ReportDataSource GetDataSource()
        //{
        //    var payments = db.Payments.Include(i => i.Cash).Include(i => i.Transfer).Include(i => i.Client)
        //       .ToList()
        //       .Select(s => new
        //       {
        //           s.PaymentId,
        //           s.DocEntry,
        //           s.CreatedDate,
        //           CardCode = s.Client.CardCode,
        //           Name = s.Client.Name,
        //           s.TotalAmount,
        //           TransferReferenceNumber = s.Transfer.ReferenceNumber,
        //           TransferAmount = s.Transfer.Amount,
        //           TransferDate = s.Transfer.Date,
        //           CashAmount = s.Cash.Amount
        //       })
        //      .ToList();

        //    //ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
        //    //dataSources.Add(new ReportDataSource("DataSourceVanShop", payments));
        //    //return dataSources;

        //    return new ReportDataSource("DataSourceVanShop", payments);
        //}

        public ActionResult SeguimientoCobranza()
        {
            var payments = db.Payments.Include(i => i.Cash).Include(i => i.Transfer).Include(i => i.Client)
             .ToList()
             .Select(s => new
             {
                 s.PaymentId,
                 s.DocEntry,
                 s.CreatedDate,
                 CardCode = s.Client.CardCode,
                 Name = s.Client.Name,
                 s.TotalAmount,
                 TransferReferenceNumber = s.Transfer.ReferenceNumber,
                 TransferAmount = s.Transfer.Amount,
                 TransferDate = s.Transfer.Date,
                 CashAmount = s.Cash.Amount
             })
            .ToList();

            ViewData["DataSource"] = payments;

            return View();
        }

        public ActionResult SeguimientoCobranzaProcess(String begin = "", String end = "" )
        {
            DateTime beginDate = DateTime.Parse(begin);
            DateTime endDate = DateTime.Parse(end);

            var payments = db.Payments.Include(i => i.Cash).Include(i => i.Transfer).Include(i => i.Client)
                .Where(w => w.CreatedDate > beginDate && w.CreatedDate < endDate)
                .ToList()
                .Select(s => new
                {
                    s.PaymentId,
                    s.DocEntry,
                    s.CreatedDate,
                    CardCode = s.Client.CardCode,
                    Name = s.Client.Name,
                    s.TotalAmount,
                    TransferReferenceNumber = s.Transfer.ReferenceNumber,
                    TransferAmount = s.Transfer.Amount,
                    TransferDate = s.Transfer.Date,
                    CashAmount = s.Cash.Amount
                })
               .ToList();

            ViewData["DataSource"] = payments;

            return View();
        }

        //public void ExportToExcel(string GridModel)
        //{
        //    var payments = db.Payments
        //       .Include(p => p.Cash).Include(p => p.Client).Include(p => p.Transfer)
        //       .OrderByDescending(o => o.CreatedDate)
        //       .ToList()
        //       .Select(s => new PaymentViewModel()
        //       {
        //           PaymentId = s.PaymentId,
        //           CreatedDate = s.CreatedDate,
        //           DocEntry = s.DocEntry,
        //           LastErrorMessage = s.LastErrorMessage,
        //           TotalAmount = s.TotalAmount
        //       });

        //    ExcelExport exp = new ExcelExport();

        //    GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);

        //    exp.Export(obj, payments, "Export.xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

        //}

        //public void ExportToPdf(string GridModel)
        //{

        //    var payments = db.Payments
        //       .Include(p => p.Cash).Include(p => p.Client).Include(p => p.Transfer)
        //       .OrderByDescending(o => o.CreatedDate)
        //       .ToList()
        //       .Select(s => new PaymentViewModel()
        //       {
        //           PaymentId = s.PaymentId,
        //           CreatedDate = s.CreatedDate,
        //           DocEntry = s.DocEntry,
        //           LastErrorMessage = s.LastErrorMessage,
        //           TotalAmount = s.TotalAmount
        //       });

        //    PdfExport exp = new PdfExport();

        //    GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);

        //    exp.Export(obj, payments, "Export.pdf", false, false, "flat-saffron");

        //}


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
