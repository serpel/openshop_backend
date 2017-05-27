using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public enum CartType
    {
        CART = 0,
        PRELIMINAR = 1
    };

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
        public int Type { get; set; }

        public Double GetProductTotalPrice()
        {
            return (GetProductSubtotalPrice() - GetProductDiscountPrice()) + GetProductISVPrice();
        }
        public Double GetProductSubtotalPrice()
        {
            return CartProductItems.Sum(s => s.TotalItemPrice);
        }

        public Double GetProductISVPrice()
        {
            return CartProductItems.Sum(s => s.ISV);
        }

        public Double GetProductDiscountPrice()
        {
            return CartProductItems.Sum(s => s.Discount);
        }

        public Double GetProductCount()
        {
            return CartProductItems.Count;
        }
    }
}