using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class CartProductVariant
    {
        [Key]
        public Int32 CartProductVariantId { get; set; }
        public Int32? ProductVariantId { get; set; }
        public String Url { get; set; }
        public String Name { get; set; }
        public Double Price { get; set; }
        public Double Discount { get; set; }
        public Double DiscountPercent { get; set; }
        public String PriceFormatted { get; set; }
        public Int32 CategoryId { get; set; }
        public String MainImage { get; set; }
        public String WareHouseCode { get; set; }
        public Int32 ColorId { get; set; }
        public Int32 SizeId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}