using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models.Helper
{
    public class Link
    {
        public string first { get; set; }
        public string last { get; set; }
        public string prev { get; set; }
        public string next { get; set; }
        public string self { get; set; }
    }
}