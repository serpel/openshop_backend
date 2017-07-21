using OpenShopVHBackend.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace OpenShopVHBackend.BussinessLogic
{
    class SalesOrder
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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

        public String AddSalesOrder(int orderId, int userId)
        {
            String lastMessage = "";
            String key = "";
            OrderStatus status = OrderStatus.CreadoEnAplicacion;

            var user = db.DeviceUser.Where(w => w.DeviceUserId == userId).ToList().FirstOrDefault();
            String connection = user.Shop == null ? "" : user.Shop.ConnectionString;

            using (var db = new ApplicationDbContext(connection))
            {
                var order = db.Orders
                    .Include(i => i.Client).Include(i => i.DeviceUser)
                    .Where(w => w.OrderId == orderId)
                    .ToList()
                    .FirstOrDefault();

                try
                {
                    if (order != null)
                    {
                        if (String.IsNullOrEmpty(order.RemoteId))
                        {
                            if (_connection.Connect() == 0)
                            {
                                company = _connection.GetCompany();
                                salesOrder = company.GetBusinessObject(BoObjectTypes.oOrders);
                                //salesOrder.DocObjectCode = BoObjectTypes.oOrders;
                                salesOrder.CardCode = order.Client.CardCode;
                                salesOrder.Comments = order.Comment;
                                salesOrder.SalesPersonCode = order.DeviceUser.SalesPersonId;
                                salesOrder.DocDueDate = order.DeliveryDate != null ? DateTime.Parse(order.DeliveryDate) : DateTime.Now.AddDays(2);

                                if (salesOrder.UserFields.Fields.Count > 0)
                                {
                                    salesOrder.UserFields.Fields.Item("U_FacFecha").Value = DateTime.Now.ToShortDateString();
                                    salesOrder.UserFields.Fields.Item("U_FacNit").Value = order.Client.RTN;
                                    salesOrder.UserFields.Fields.Item("U_FacNom").Value = order.Client.Name;
                                }

                                foreach (var item in order.OrderItems)
                                {
                                    salesOrder.Lines.ItemCode = item.SKU;
                                    salesOrder.Lines.Quantity = item.Quantity;
                                    salesOrder.Lines.TaxCode = item.TaxCode;
                                    salesOrder.Lines.DiscountPercent = item.DiscountPercent;
                                    salesOrder.Lines.WarehouseCode = item.WarehouseCode;
                                    salesOrder.Lines.Add();
                                }

                                //Autorization
                                salesOrder.GetApprovalTemplates();
                                if(salesOrder.Document_ApprovalRequests.Count > 0 && salesOrder.Document_ApprovalRequests.ApprovalTemplatesID > 0)
                                {
                                    salesOrder.Document_ApprovalRequests.SetCurrentLine(0);
                                    salesOrder.Document_ApprovalRequests.Remarks = "Add Doc from DI API";
                                }

                                // add Sales Order to draft
                                if (salesOrder.Add() == 0)
                                {
                                    key = company.GetNewObjectKey();
                                    lastMessage = String.Format("Successfully, DocEntry: {0}", key);
                                    MyLogger.GetInstance.Info(lastMessage);
                                    status = OrderStatus.PreliminarEnSAP;
                                }
                                else
                                {
                                    lastMessage = "Error Code: "
                                            + company.GetLastErrorCode().ToString()
                                            + " - "
                                            + company.GetLastErrorDescription();

                                    status = OrderStatus.ErrorAlCrearEnSAP;
                                }
                                //recomended from http://www.appseconnect.com/di-api-memory-leak-in-sap-business-one-9-0/
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(salesOrder);
                                salesOrder = null;
                                //company.Disconnect();
                            }
                            else
                            {
                                lastMessage = "AddSalesOrder - Connection error: "
                                        + _connection.GetErrorMessage().ToString();

                                status = OrderStatus.ErrorAlCrearEnSAP;
                            }

                            if (order != null)
                            {
                                order.RemoteId = key;
                                order.LastErrorMessage = lastMessage;
                                order.Status = status;
                                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    lastMessage += "AddSalesOrder - error" + e.Message;
                    MyLogger.GetInstance.Error(lastMessage, e);

                    if (order != null)
                    {
                        order.LastErrorMessage = lastMessage;
                        order.Status = OrderStatus.ErrorAlCrearEnSAP;
                        db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return key;
            }
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
