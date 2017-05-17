using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenShopVHBackend.Models;
using Excel;
using System.IO;

namespace OpenShopVHBackend.Controllers
{
    public class FeesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Fees
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase uploadfile)
        {
            if (ModelState.IsValid)
            {
                if (uploadfile != null && uploadfile.ContentLength > 0)
                {
                    //ExcelDataReader works on binary excel file
                    Stream stream = uploadfile.InputStream;
                    //We need to written the Interface.
                    IExcelDataReader reader = null;
                    if (uploadfile.FileName.EndsWith(".xls"))
                    {
                        //reads the excel file with .xls extension
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (uploadfile.FileName.EndsWith(".xlsx"))
                    {
                        //reads excel file with .xlsx extension
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        //Shows error if uploaded file is not Excel file
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }
                    //treats the first row of excel file as Coluymn Names
                    reader.IsFirstRowAsColumnNames = true;
                    //Adding reader data to DataSet()
                    DataSet result = reader.AsDataSet();
                    reader.Close();

                    if (result != null)
                    {
                        foreach (DataRow row in result.Tables[0].Rows)
                        {
                            Fees fee = new Fees();

                            foreach (DataColumn col in result.Tables[0].Columns)
                            {
                                if (col.ColumnName.ToUpper() == "FECHA")
                                    fee.Date = DateTime.Parse(row[col.ColumnName].ToString());
                                if (col.ColumnName.ToUpper() == "CUOTA")
                                    fee.Amount = Double.Parse(row[col.ColumnName].ToString());
                                if (col.ColumnName.ToUpper() == "CODIGOVENDEDORSAP")
                                {
                                    Int32 salesperson = Int32.Parse(row[col.ColumnName].ToString());
                                    var user = db.DeviceUser.Where(w => w.SalesPersonId == salesperson).ToList().FirstOrDefault();
                                    fee.DeviceUserId = user.DeviceUserId;
                                }
                            }

                            db.Fees.Add(fee);
                        }

                        try { 
                            db.SaveChanges();
                        }catch(Exception e)
                        {
                            ModelState.AddModelError("File", e.Message);
                        }
                    }
                    //Sending result data to View
                    return View(result.Tables[0]);
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please upload your file");
            }
            return View();
        }


        // GET: Fees
        public ActionResult Index()
        {
            var fees = db.Fees
                .Include(f => f.DeviceUser)
                .OrderByDescending(o => o.Date)
                .ToList()
                .Select(s => new FeesViewModel
                {
                    FeesId = s.FeesId,
                    Date = s.Date,
                    Amount = s.Amount,
                    UserName = s.DeviceUser.Name
                });

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
