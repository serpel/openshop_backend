using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Product
    {
        [Key]
        public Int32 ProductId { get; set; }
        public Int32 RemoteId { get; set; }
        [Required]
        public String Name { get; set; }
        public String Code { get; set; }
        [Required]
        public Double Price { get; set; }
        public String PriceFormated { get; set; }
        public Double DisountedPrice { get; set; }
        public String DisountedPriceFormated { get; set; }
        [Required]
        public Int32 CategoryId { get; set; }
        [Required]
        public Int32 BrandId { get; set; }
        [Required]
        public String Currency { get; set; }
        public String Description { get; set; }
        public String MainImage { get; set; }
        public String MainImageHighRes { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<Product> Related { get; set; }
    }
}