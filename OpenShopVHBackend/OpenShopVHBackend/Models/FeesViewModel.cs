using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class FeesViewModel
    {
        public Int32 FeesId { get; set; }
        public String UserName { get; set; }
        public DateTime Date { get; set; }
        public Double Amount { get; set; }
    }
}