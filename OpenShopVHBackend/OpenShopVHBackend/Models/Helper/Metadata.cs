using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models.Helper
{
    public class Metadata
    {
        public Link links { get; set; }
        public string sorting { get; set; }
        public int records_count { get; set; }
        public List<FilterType> filters { get; set; }
    }
}