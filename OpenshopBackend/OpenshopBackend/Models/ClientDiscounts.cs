using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class ClientDiscounts
    {
        [Key]
        public Int32 ClientDiscountsId { get; set; }
        public Int32 GroupId { get; set; }
        public String CardCode { get; set; }
        public decimal Discount { get; set; }
    }
}