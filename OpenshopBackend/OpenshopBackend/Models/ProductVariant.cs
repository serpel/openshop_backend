using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenshopBackend.Models
{
    public class ProductVariant
    {
        [Key]
        public Int32 ProductVariantId { get; set; }
        public Int32 ProductId { get; set; }
        public Int32 ColorId { get; set; }
        public Int32 SizeId { get; set; }
        [Required]
        public String Code { get; set; }
        public IList<String> Images { get; set; }
        public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }
        public virtual Product Product { get; set; }

    }
}