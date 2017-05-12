using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Color
    {
        [Key]
        public Int32 ColorId { get; set; }
        public Int32 RemoteId { get; set; }
        public String Value { get; set; }
        public String Code { get; set; }
        public String Image { get; set; }
        public String Description { get; set; }
    }
}