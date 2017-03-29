using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Cash
    {
        [Key]
        public Int32 CashId { get; set; }
        public String GeneralAccount { get; set; }
        public Double Amount { get; set; }
    }
}