using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class OrderViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public Int32 OrderId { get; set; }
        [JsonProperty(PropertyName = "remote_id")]
        public String RemoteId { get; set; }
        [JsonProperty(PropertyName = "date_created")]
        public String DateCreated { get; set; }
        [JsonProperty(PropertyName = "card_code")]
        public String CardCode { get; set; }
        public String Comment { get; set; }
        [JsonProperty(PropertyName = "sales_person_code")]
        public Int32 SalesPersonCode { get; set; }
        [JsonProperty(PropertyName = "series")]
        public Int32 Series { get; set; }
    }
}