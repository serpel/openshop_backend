using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class OrderItem
    {
        [Key]
        public Int32 OrderItemId { get; set; }
        public Int32 OrderId { get; set; }
        public String SKU { get; set; }
        public Int32 Quantity { get; set; }
        public Double Price { get; set; }
        public Double Discount { get; set; }
        public Double DiscountPercent { get; set; }
        public Double TaxValue { get; set; }
        public String TaxCode { get; set; }
        public String WarehouseCode { get; set; }
        public virtual Order Order { get; set; }
    }
}