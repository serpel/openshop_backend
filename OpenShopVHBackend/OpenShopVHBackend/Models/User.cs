using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class DeviceUser
    {
        [Key]
        public Int32 DeviceUserId { get; set; }
        public String Name { get; set; }
        public Int32 SalesPersonId { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String AccessToken { get; set; }
        public String PrintBluetoothAddress { get; set; }
        public String CollectId { get; set; }
        public Int32? ShopId { get; set; }
        public virtual List<Order> Orders { get; set; }
        public virtual Shop Shop { get; set; }
    }
}