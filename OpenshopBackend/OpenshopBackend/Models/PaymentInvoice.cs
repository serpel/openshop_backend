using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class PaymentInvoice
    {
        public Int32 PaymentInvoiceId { get; set; }
        public Int32? PaymentId { get; set; }
        public Int32? DocumentId { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Document Document { get; set; }
    }
}