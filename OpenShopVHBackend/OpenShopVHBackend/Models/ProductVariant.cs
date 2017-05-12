using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenShopVHBackend.Models
{
    public class ProductVariant
    {
        [Key]
        public Int32 ProductVariantId { get; set; }
        public Int32 ItemGroup { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 ColorId { get; set; }
        public Int32 SizeId { get; set; }
        [Required]
        public String Code { get; set; }
        public Int32 Quantity { get; set; }
        public Int32 IsCommitted { get; set; }
        public Double Price { get; set; }
        public String Currency { get; set; }
        public String WareHouseCode { get; set; }
        public IList<String> Images { get; set; }
        public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }
        public virtual Product Product { get; set; }

        public String GetPriceTotalFormated()
        {
            return this.Currency + ' ' + this.Price;
        }
    }
}