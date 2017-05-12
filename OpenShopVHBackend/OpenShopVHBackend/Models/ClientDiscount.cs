using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class ClientDiscount
    {
        [Key]
        public Int32 ClientDiscountId { get; set; }
        public String CardCode { get; set; }
        public Int32 ItemGroup { get; set; }
        public double Discount { get; set; }
    }
}