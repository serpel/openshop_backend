using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenShopVHBackend.Models
{
    public class WishlistItem
    {
        [Key]
        public Int32 WishlistItemId { get; set; }
        public Int32? WishlistProductVariantId { get; set; }
        public Int32? DeviceUserId { get; set; }

        public WishlistProductVariant WishlistProductVariant { get; set; }
        public DeviceUser DeviceUser { get; set; }
    }
}