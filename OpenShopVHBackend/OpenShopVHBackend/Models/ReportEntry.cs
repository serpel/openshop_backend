using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class ReportEntry
    {
        [JsonProperty(PropertyName = "x")]
        public Double X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public Double Y { get; set;  }
        [JsonProperty(PropertyName = "label")]
        public String Label { get; set; }
        [JsonProperty(PropertyName = "index")]
        public Int32 Index { get; set; }
    }
}