using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Shop
    {
        [Key]
        public Int32 ShopId { get; set; }
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        public String Url { get; set; }
        public String Logo { get; set; }
        public String GoogleUA { get; set; }
        public String Language { get; set; }
        public String Currency { get; set; }
        [Required]
        public String FlagIcon { get; set; }
    }
}