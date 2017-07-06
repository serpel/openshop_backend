using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public enum PaymentStatus
    {
        CreadoEnAplicacion = 1,
        CreadoEnSAP = 2,
        Error = 3,
        Canceled = 4
    }

    public class Payment
    {
        [Key]
        public Int32 PaymentId { get; set; }
        public String DocEntry { get; set; }
        public Int32? ClientId { get; set; }
        public Int32? CashId { get; set; }
        public Int32? TransferId { get; set; }
        public Int32? DeviceUserId { get; set; }
        public Double TotalAmount { get; set; }
        public String LastErrorMessage { get; set; }
        public PaymentStatus Status { get; set; }
        public String Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public String ReferenceNumber { get; set; }
        public virtual List<Check> Checks { get; set; }
        public virtual Cash Cash { get; set; }
        public virtual Transfer Transfer { get; set; }
        public virtual Client Client { get; set; }
        public virtual DeviceUser DeviceUser { get; set; }
        public virtual List<InvoiceItem> Invoices { get; set; }
    }
}