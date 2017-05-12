using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Fees
    {
        public Int32 FeesId { get; set; }
        public Int32 DeviceUserId { get; set; }
        public DateTime Date { get; set; }
        public Double Amount { get; set; }

        public DeviceUser DeviceUser { get; set; }
    }
}