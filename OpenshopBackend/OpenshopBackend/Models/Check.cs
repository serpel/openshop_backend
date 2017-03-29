using System;
using System.ComponentModel.DataAnnotations;

namespace OpenshopBackend.Models
{
    public class Check
    {
        [Key]
        public Int32 CheckId { get; set; }
        public Int32 PaymentId { get; set; }
        public String RefenceNumber { get; set; }
        public Int32 BankId { get; set; }
        public Double Amount { get; set; }
        public String GeneralAccount { get; set; }
        public DateTime DueDate { get; set; }
        public virtual Bank Bank { get; set; } 
        public virtual Payment Payment { get; set; }
    }
}