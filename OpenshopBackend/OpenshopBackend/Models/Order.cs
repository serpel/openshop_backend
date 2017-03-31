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
        CreadoEnAplicacion = 0,
        PreliminarEnSAP = 1,
        Autorizado = 2,
        ErrorAlCrearEnSAP = 3
    }

    public class Order
    {
        [Key]
        public Int32 OrderId { get; set; }
        public String RemoteId { get; set; }
        public String DateCreated { get; set; }
        public String Status { get; set; }
        public Int32? ClientId { get; set; }
        public Int32? DeviceUserId { get; set; }
        public String Comment { get; set; }
        public String LastErrorMessage { get; set; }
        public Int32 Series { get; set; }
        
        public Double GetTotal()
        {
            return (GetSubtotal() - GetDiscount()) + GetIVA();
        }

        public Double GetItemCount()
        {
            return this.OrderItems.Count;
        }

        public Double GetSubtotal()
        {
            return this.OrderItems.Sum(s => s.Quantity * s.Price);
        }

        public Double GetIVA()
        {
            return this.OrderItems.Sum(s => ((s.Quantity * s.Price) - s.Discount) * s.TaxValue);
        }

        public Double GetDiscount()
        {
            return this.OrderItems.Sum(s => s.Discount);
        }

        public virtual Client Client { get; set; }
        public virtual DeviceUser DeviceUser { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }    
    }
}