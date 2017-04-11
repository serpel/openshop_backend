using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Client
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public Int32 ClientId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }
        [JsonProperty(PropertyName = "card_code")]
        public String CardCode { get; set; }
        [JsonProperty(PropertyName = "phone")]
        public String PhoneNumber { get; set; }
        [JsonProperty(PropertyName = "address")]
        public Double CreditLimit { get; set; }
        [JsonProperty(PropertyName = "credit_limit")]
        public Double Balance { get; set; }
        [JsonProperty(PropertyName = "balance")]
        public Double InOrders { get; set; }
        [JsonProperty(PropertyName = "in_oders")]
        public String PayCondition { get; set; }
        [JsonProperty(PropertyName = "pay_condition")]
        public String Address { get; set; }
        [JsonProperty(PropertyName = "discount_percent")]
        public String RTN { get; set; }
        public Double past_due { get; set; }
        public Double to_pay { get; set; }
        public Double to_pay_future { get; set; }
        public String ContactPerson { get; set; }
        public virtual List<Document> Invoices { get; set; }
       
    }
}