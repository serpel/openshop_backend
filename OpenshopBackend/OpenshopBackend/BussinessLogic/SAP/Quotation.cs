using OpenshopBackend.Models;
using SAPbobsCOM;
using System.Linq;
using System;
using System.Collections.Generic;

namespace OpenshopBackend.BussinessLogic
{
    class Quotation
    {
        private ServerConnection _connection;
        private ApplicationDbContext db;

        public Quotation()
        {
            this._connection = new ServerConnection();
            this.db = new ApplicationDbContext();
        }

        public Quotation(ApplicationDbContext db)
        {
            this.db = db;
            this._connection = new ServerConnection();
        }

        public Quotation(ServerConnection serverConnection)
        {
            this._connection = serverConnection;
        }

        private ICompany company { get; set; }
        // declare Document object
        private IDocuments salesOrder { get; set; }

        private String lastMessage;

        public String LastMessage
        {
            set { this.lastMessage = value; }
            get { return this.lastMessage; }
        }

        public String AddQuotation(int orderId)
        {
            String key = "";
            try
            {
                var order = db.Orders
                    .ToList()
                    .Where(w => w.OrderId == orderId)
                    .FirstOrDefault();

                if (_connection.Connect() == 0)
                {
                    if (order != null)
                    {
                        company = _connection.GetCompany();
                        salesOrder = company.GetBusinessObject(BoObjectTypes.oQuotations);
                        salesOrder.CardCode = order.Client.CardCode;
                        salesOrder.Comments = order.Comment;
                        //salesOrder.Series = order.Series;
                        salesOrder.SalesPersonCode = order.DeviceUser.SalesPersonId;
                        salesOrder.DocDueDate = DateTime.Now;

                        foreach (var item in order.OrderItems)
                        {
                            salesOrder.Lines.ItemCode = item.SKU;
                            salesOrder.Lines.Quantity = item.Quantity;
                            salesOrder.Lines.TaxCode = item.TaxCode;
                            salesOrder.Lines.WarehouseCode = item.WarehouseCode;
                            salesOrder.Lines.Add();
                        }
                        // add Sales Order
                        if (salesOrder.Add() == 0)
                        {
                            lastMessage = String.Format("Successfully added Sales Order DocEntry: {0}", company.GetNewObjectKey());
                            MyLogger.GetInstance.Debug(lastMessage);
                            key = company.GetNewObjectKey();
                        }
                        else
                        {
                            lastMessage = "Error Code: "
                                    + company.GetLastErrorCode().ToString()
                                    + " - "
                                    + company.GetLastErrorDescription();
                        }

                        //recomended from http://www.appseconnect.com/di-api-memory-leak-in-sap-business-one-9-0/
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(salesOrder);
                        //salesOrder = null;
                        company.Disconnect();
                    }
                }
                else
                {
                    lastMessage = String.Format("Error: {0}", _connection.GetErrorMessage());
                    MyLogger.GetInstance.Debug(lastMessage);
                }

                order.LastErrorMessage = lastMessage;
                order.Status = OrderStatus.PreliminarEnSAP.ToString();
                order.RemoteId = key != null ? key : "";
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MyLogger.GetInstance.Error(e.Message, e);
                lastMessage += e.Message;
            }

            return key;
        }
    }
}
