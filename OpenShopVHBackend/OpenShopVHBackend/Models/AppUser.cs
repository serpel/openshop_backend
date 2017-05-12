using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class AppUser
    {
        [Key]
        public Int32 AppUserId { get; set; }
        public String fbId { get; set; }
        public String Name { get; set; }
        public String AccessToken { get; set; }
    }
}