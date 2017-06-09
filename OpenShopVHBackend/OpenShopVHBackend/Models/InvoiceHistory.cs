using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenShopVHBackend.Models
{
    public class InvoiceHistory
    {
        [Key]
        public Int32 InvoiceId { get; set; }
        public String DocNum { get; set; }
        public String CardCode { get; set; }
        public String CardName { get; set; }
        public Double Total { get; set; }
    }
}