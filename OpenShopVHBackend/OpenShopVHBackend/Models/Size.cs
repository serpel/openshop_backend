using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Size
    {
        [Key]
        public Int32 SizeId { get; set; }
        public Int32 RemoteId { get; set; }
        [Required]
        public String Value { get; set; }
        public String Description { get; set; }
    }
}