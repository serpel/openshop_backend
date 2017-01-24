using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Cart
    {
        [Key]
        public Int32 CartId { get; set; } 
        public Int32 DeviceUserId { get; set; }
        public Double TotalPrice { get; set; }
        public String TotalPriceFormatted { get; set; }
        public String Currency { get; set; }
        public virtual DeviceUser DeviceUser { get; set; }
        public virtual ICollection<CartProductItem> CartProductItems { get; set; }

        public Double GetProductTotalPrice()
        {
            return CartProductItems.Sum(s => s.TotalItemPrice);
        }

        public Double GetProductCount()
        {
            return CartProductItems.Count;
        }
    }
}