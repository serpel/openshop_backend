using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Bank
    {
        [Key]
        public Int32 BankId { get; set; }
        public String Name { get; set; }
        public String FormatCode { get; set; }
        public String GeneralAccount { get; set; }
    }
}