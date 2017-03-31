using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Setting
    {
        public Int32 SettingId { get; set; }
        public Int32 ShopId { get; set; }
        public String Currency { get; set; }
        public double ISV { get; set; }
        public virtual Shop Shop { get; set; }
    }
}