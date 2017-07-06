using System;

namespace OpenShopVHBackend.Models.Helper
{
    public class FilterType
    {
        public Int32 id { get; set; }
        public String name { get; set; }
        public String type { get; set; }
        public String label { get; set; }
        public Object values { get; set; }
    }
}