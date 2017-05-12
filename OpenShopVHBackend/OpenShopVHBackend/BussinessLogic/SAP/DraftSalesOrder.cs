using OpenShopVHBackend.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenShopVHBackend.BussinessLogic
{
    class DraftSalesOrder
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ServerConnection _connection;

        public DraftSalesOrder(){
            _connection = new ServerConnection();
        }

        public DraftSalesOrder(ServerConnection serverConnection)
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

        public String AddSalesOrder(int orderId)
        {
            String lastMessage = "";
            String key = "";

            try
            {
                var order = db.Orders
                        .Where(w => w.OrderId == orderId)
                        .ToList()
                        .FirstOrDefault();

                if (order != null)
                {
                    if (String.IsNullOrEmpty(order.RemoteId))
                    {
                        if (_connection.Connect() == 0)
                        {
                            company = _connection.GetCompany();
                            salesOrder = company.GetBusinessObject(BoObjectTypes.oOrders);
                            salesOrder.CardCode = order.Client.CardCode;
                            salesOrder.Comments = order.Comment;
                            salesOrder.SalesPersonCode = order.DeviceUser.SalesPersonId;
                            salesOrder.DocDueDate = order.DeliveryDate.Count() > 0 ? DateTime.Parse(order.DeliveryDate) : DateTime.Now.AddDays(2);

                            if(salesOrder.UserFields.Fields.Count > 0)
                            {
                                salesOrder.UserFields.Fields.Item("U_FacFecha").Value = DateTime.Now; 
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
                            // add Sales Order to draft
                            if (salesOrder.SaveDraftToDocument() == 0)
                            {
                                lastMessage = String.Format("Successfully, DocEntry: {0}", company.GetNewObjectKey());
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
                            company.Disconnect();
                        }
                        else
                        {
                            lastMessage = "Error Msg: "
                                    + _connection.GetErrorMessage().ToString();
                        }

                        //var ord = db.Orders
                        //    .Where(w => w.OrderId == orderId)
                        //    .ToList()
                        //    .FirstOrDefault();

                        if (order != null)
                        {
                            order.RemoteId = key;
                            order.LastErrorMessage = lastMessage;
                            order.Status = String.IsNullOrEmpty(key) ? OrderStatus.CreadoEnAplicacion : OrderStatus.ErrorAlCrearEnSAP;
                            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
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
