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
        public String Address { get; set; }
    }
}