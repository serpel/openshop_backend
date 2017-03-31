using OpenshopBackend.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;

namespace OpenshopBackend.BussinessLogic
{
    class SalesOrder
    {
        private ServerConnection _connection;
        public ServerConnection ServerConnection {
            get {

                if(this._connection == null)
                {
                    _connection = new ServerConnection();
                }

                return _connection;
            }
            set
            {
                this._connection = value;
            }
        }

        public SalesOrder(){}

        public SalesOrder(ServerConnection serverConnection)
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

        public String AddSalesOrder(Order order, List<OrderItem> items)
        {
            String key = "";
            try
            {
                company = _connection.GetCompany();
                salesOrder = company.GetBusinessObject(BoObjectTypes.oOrders);
                salesOrder.CardCode = order.Client.CardCode;
                salesOrder.Comments = order.Comment;
                //71 Mayorista
                salesOrder.Series = order.Series;
                salesOrder.SalesPersonCode = order.DeviceUser.SalesPersonId;
                salesOrder.DocDueDate = DateTime.Now;

                foreach(var item in items)
                {
                    salesOrder.Lines.ItemCode = item.SKU;
                    salesOrder.Lines.Quantity = item.Quantity;
                    salesOrder.Lines.TaxCode = item.TaxCode;
                    salesOrder.Lines.DiscountPercent = item.DiscountPercent;
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
                System.Runtime.InteropServices.Marshal.ReleaseComObject(salesOrder);
                salesOrder = null;
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
