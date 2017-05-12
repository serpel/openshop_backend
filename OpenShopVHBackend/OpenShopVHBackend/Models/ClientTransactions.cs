using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class ClientTransactions
    {
        [Key]
        public Int32 ClientTransactionId { get; set; }
        public Int32 ReferenceNumber { get; set; }
        public String CardCode { get; set; }
        public String Description { get; set; }
        public Double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}