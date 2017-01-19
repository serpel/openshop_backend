using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Category
    {
        [Key]
        public Int32 CategoryId { get; set; }
        public Int32 RemoteId { get; set; }
        public String Code { get; set; }
        public Int32 PartentId { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
    }
}