using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using OpenShopVHBackend.Models;
using OpenShopVHBackend.Models.Helper;
using Newtonsoft.Json;
using Hangfire;
using OpenShopVHBackend.BussinessLogic;
using System.Web.Script.Serialization;
using System.Text;
using OpenShopVHBackend.Controllers;
using OpenShopVHBackend.BussinessLogic.SAP;
using Syncfusion.EJ.ReportViewer;
using Syncfusion.Reports.EJ;

namespace OpenShopVHBackend.Api
{
    public class ReportApiController : ApiController, IReportController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Post action for processing the rdl/rdlc report 
        public object PostReportAction(Dictionary<string, object> jsonResult)
        {
            return ReportHelper.ProcessReport(jsonResult, this);
        }

        //Get action for getting resources from the report
        [System.Web.Http.ActionName("GetResource")]
        [AcceptVerbs("GET")]
        public object GetResource(string key, string resourcetype, bool isPrint)
        {
            return ReportHelper.GetResource(key, resourcetype, isPrint);
        }

        //Method will be called when initialize the report options before start processing the report        
        public void OnInitReportOptions(ReportViewerOptions reportOption)
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

            reportOption.ReportModel.DataSources.Clear();
            reportOption.ReportModel.DataSources.Add(new ReportDataSource { Name = "Payments", Value = new {  PaimentId = 1, DocEntry = 1 } });
        }



        //Method will be called when reported is loaded
        public void OnReportLoaded(ReportViewerOptions reportOption)
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

            reportOption.ReportModel.DataSources.Add(new ReportDataSource() { Name = "Payments", Value = payments });
        }
    }
}