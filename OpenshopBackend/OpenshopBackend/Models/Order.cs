using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Order
    {
        [Key]
        public Int32 OrderId { get; set; }
        public String RemoteId { get; set; }
        private String DateCreated { get; set; }
        public String Status { get; set; }
        public int Total { get; set; }
        public String Currency { get; set; }
        public String CardCode { get; set; }
        private String Comment { get; set; }
    }
}