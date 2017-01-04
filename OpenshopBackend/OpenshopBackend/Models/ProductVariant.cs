using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenshopBackend.Models
{
    public class ProductVariant
    {
        [Key]
        public Int32 ProductVariantId { get; set; }
        [Required]
        public Int32 ColorId { get; set; }
        [Required]
        public Int32 SizeId { get; set; }
        public String Code { get; set; }
        public IList<String> Images { get; set; }
        public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }

    }
}