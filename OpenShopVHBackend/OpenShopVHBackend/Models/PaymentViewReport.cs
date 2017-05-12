using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class PaymentViewReport
    {
        public Int32 PaymentId { get; set; }
        public String DocEntry { get; set; }
        public DateTime CreatedDate { get; set; }
        public String CardCode { get; set; }
        public String Name { get; set; }
        public Double TotalAmount { get; set; }
    }
}