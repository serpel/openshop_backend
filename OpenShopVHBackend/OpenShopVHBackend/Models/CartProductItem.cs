using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class CartProductItem
    {
        [Key]
        public Int32 CartProductItemId { get; set; }
        public Int32 CartProductVariantId { get; set; }
        public Int32 CartId { get; set; }
        public Int32 RemoteId { get; set; }
        //TODO: the best to include orderId
        public Int32? OrderId { get; set; }
        public int Quantity { get; set; }
        public Double Discount { get; set; }
        public Double DiscountPercent { get; set; }
        public Double ISV { get; set; }
        public Double TotalItemPrice { get; set; }
        public String TotalItemPriceFormatted { get; set; }
        public int Expiration { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual Order Order { get; set; }
        public virtual CartProductVariant CartProductVariant { get; set; }
    }
}