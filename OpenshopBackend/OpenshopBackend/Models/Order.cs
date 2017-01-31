using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{

    enum OrderStatus
    {
        Created = 0,
        Procesed = 1,
        Fail = 2
    }

    public class Order
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public Int32 OrderId { get; set; }
        [JsonProperty(PropertyName = "remote_id")]
        public String RemoteId { get; set; }
        [JsonProperty(PropertyName = "date_created")]
        public String DateCreated { get; set; }
        [JsonProperty(PropertyName = "status")]
        public String Status { get; set; }
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
        [JsonProperty(PropertyName = "card_code")]
        public String CardCode { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public String Comment { get; set; }
        [JsonProperty(PropertyName = "sales_person_code")]
        public Int32 SalesPersonCode { get; set; }
        [JsonProperty(PropertyName = "series")]
        public Int32 Series { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}