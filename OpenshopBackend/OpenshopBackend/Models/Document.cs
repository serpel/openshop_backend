using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenshopBackend.Models
{
    public class Document
    {
        [Key]
        public Int32 DocumentId { get; set; }
        public String DocumentCode { get; set; }
        public String CreatedDate { get; set; }
        public String DueDate { get; set; }
        public Double TotalAmount { get; set; }
        public Double PayedAmount { get; set; }
        public Int32 ClientId { get; set; }
        public Int32 DocEntry { get; set; }
        public virtual Client Client { get; set; }
        public virtual List<PaymentInvoice> PaymentInvoices { get; set; }
    }
}