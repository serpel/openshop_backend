using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class InvoiceItem
    {
        public Int32 InvoiceItemId { get; set; }
        public Int32 PaymentId { get; set; }
        public Int32 DocEntry { get; set; }
        public String DocumentNumber { get; set; }
        public Double TotalAmount { get; set; }
        public Double PayedAmount { get; set; }
        public virtual Payment Payment { get; set; }
    }
}