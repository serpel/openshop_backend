using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class Collection
    {
        [Key]
        public Int32 CollectionId { get; set; }
        public String CollectionText { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}