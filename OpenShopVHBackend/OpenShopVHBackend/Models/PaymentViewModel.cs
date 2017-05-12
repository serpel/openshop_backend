using System;

namespace OpenShopVHBackend.Models
{
    public class PaymentViewModel
    {
        public PaymentViewModel(){}

        public int PaymentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DocEntry { get; set; }
        public string LastErrorMessage { get; set; }
        public string Comment { get; set; }
        public string SalesPersonName { get; set; }
        public string Status { get; set; }
        public double TotalAmount { get; set; }
        public double TotalTransfer { get; set; }
        public string Invoices { get; set; }
    }
}