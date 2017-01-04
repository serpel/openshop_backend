using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Size
    {
        [Key]
        public Int32 SizeId { get; set; }
        [Required]
        public String Name { get; set; }
        public String Code { get; set; }
    }
}