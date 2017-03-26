using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class CategoryViewModel
    {
        public Int32 CategoryId { get; set; }
        public Int32 Id { get; set; }
        public Int32 PartentId { get; set; }
        public Int32 RemoteId { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
        public List<CategoryViewModel> Children { get; set; }
    }
}