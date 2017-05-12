using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
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
        public String Status { get; set; }
        [JsonProperty(PropertyName = "sales_person_code")]
        public Int32 SalesPersonCode { get; set; }
        public String SalesPersonName { get; set; }
        [JsonProperty(PropertyName = "series")]
        public Int32 Series { get; set; }
        public String LastErrorMessage { get; set; }
        [JsonProperty(PropertyName = "delivery_date")]
        public String DeliveryDate { get; set; }
        public Double Total { get; set; }
    }
}