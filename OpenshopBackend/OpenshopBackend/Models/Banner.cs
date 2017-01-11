using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Banner
    {
        [Key]
        public Int32 BannerId { get; set; }
        public String Name { get; set; }
        public String Target { get; set; }
        public String ImageUrl { get; set; }
    }
}