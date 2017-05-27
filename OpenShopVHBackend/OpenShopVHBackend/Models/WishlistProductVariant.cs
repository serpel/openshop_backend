using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class WishlistProductVariant
    {
        [Key]
        public Int32 WishlistProductVariantId { get; set; }
        public Int32 ProductId { get; set; }
        public String Name { get; set; }
        public Int32 CategoryId { get; set; }
        public Double Price { get;set; }
        public Double DiscountPrice { get; set; }
        public String PriceFormatted { get; set; }
        public String DiscountPriceFormatted { get; set; }
        public String Currency { get; set; }
        public String Code { get; set; }
        public String Description { get; set; }
        public String MainImage { get; set; }
        public String MainImageHighRes { get; set; }
    }
}