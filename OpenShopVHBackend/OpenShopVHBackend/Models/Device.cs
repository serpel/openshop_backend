using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Device
    {
        [Key]
        public Int32 DeviceId { get; set; }
        public String DeviceToken { get; set; }
        public String Platform { get; set; }
    }
}