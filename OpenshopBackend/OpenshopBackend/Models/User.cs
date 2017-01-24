using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class DeviceUser
    {
        [Key]
        public Int32 DeviceUserId { get; set; }
        public String Name { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String AccessToken { get; set; }
    }
}