using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Brand
    {
        [Key]
        public Int32 BrandId { get; set; }
        [Required]
        public String Name { get; set; }
        public String BrandImg { get; set; }
        public bool IsPremium { get; set; }
    }
}