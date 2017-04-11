using OpenshopBackend.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenshopBackend.BussinessLogic.SAP
{
    public class DraftPayment
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ServerConnection _connection;

        public DraftPayment(){
            _connection = new ServerConnection();
        }

        private ICompany company { get; set; }

        public String MakePayment(int paymentId) {

            String lastMessage = "";
            String key = "";

            var p = db.Payments
                    .Where(w => w.PaymentId == paymentId)
                    .ToList()
                    .FirstOrDefault();

            if(p != null)
            {
                if (String.IsNullOrEmpty(p.DocEntry))
                {
                    if (_connection.Connect() == 0)
                    {
                        company = _connection.GetCompany();

                        Double totalAmount = p.TotalAmount;
                        Double amountLeft = totalAmount;

                        Payments payment = company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                        payment.DocObjectCode = BoPaymentsObjectType.bopot_IncomingPayments;
                        payment.DocType = BoRcptTypes.rCustomer;
                        payment.CardCode = p.Client.CardCode;
                        payment.DocDate = DateTime.Now;
                        payment.VatDate = DateTime.Now;
                        payment.DueDate = DateTime.Now;

                        if (p.Cash != null)
                        {
                            payment.CashAccount = p.Cash.GeneralAccount;
                            payment.CashSum = p.Cash.Amount;
                        }

                        if (p.Transfer != null)
                        {
                            payment.TransferAccount = p.Transfer.GeneralAccount;
                            payment.TransferDate = p.Transfer.Date;
                            payment.TransferReference = p.Transfer.ReferenceNumber;
                            payment.TransferSum = p.Transfer.Amount;
                        }

                        if (p.Checks != null)
                        {
                            foreach (Check check in p.Checks)
                            {
                                payment.Checks.CheckAccount = check.GeneralAccount;
                                payment.Checks.CheckSum = check.Amount;
                                payment.Checks.DueDate = check.DueDate;
                                payment.Checks.BankCode = check.Bank.FormatCode;
                                payment.Checks.Add();
                            }
                        }

                        if (p.Invoices != null)
                        {
                            foreach (InvoiceItem invoice in p.Invoices)
                            {
                                if (amountLeft > 0)
                                {
                                    payment.Invoices.DocEntry = invoice.DocEntry;
                                    payment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                    //Si aun hay cash entonces pago la factura completa sino la pago incompleta
                                    payment.Invoices.SumApplied = invoice.PayedAmount <= amountLeft ? invoice.PayedAmount : amountLeft;
                                    amountLeft -= payment.Invoices.SumApplied;
                                    payment.Invoices.Add();
                                }
                            }

                            int errorCode = payment.Add();

                            if (errorCode != 0)
                            {
                                lastMessage = "Error Code: "
                                            + company.GetLastErrorCode().ToString()
                                            + " - "
                                            + company.GetLastErrorDescription();
                            }
                            else
                            {
                                key = company.GetNewObjectKey();
                            }
                        }

                        payment = null;
                        company.Disconnect();
                    }
                    else
                    {
                        lastMessage = "Error Msg: "
                                    + _connection.GetErrorMessage().ToString();
                    }

                    var pay = db.Payments
                        .Where(w => w.PaymentId == paymentId)
                        .ToList()
                        .FirstOrDefault();

                    if (pay != null)
                    {
                        pay.DocEntry = key;
                        pay.LastErrorMessage = lastMessage;
                        db.Entry(pay).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            return lastMessage;
        }
    }
}