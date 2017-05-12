using System;

namespace OpenShopVHBackend.Models
{
    public class Transfer
    {
        public Int32 TransferId { get; set; }
        public String ReferenceNumber { get; set; }
        public Double Amount { get; set; }
        public DateTime Date { get; set; }
        public String GeneralAccount { get; set; }
    }
}