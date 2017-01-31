using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDIAPI
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

        public SalesOrder()
        {
        }

        public SalesOrder(ServerConnection serverConnection)
        {
            this._connection = serverConnection;
        }

        public ICompany company { get; set; }
        // declare Document object
        public IDocuments salesOrder { get; set; }

        public String AddSalesOrder()
        {
            String message = "";

            try
            {
                company = _connection.GetCompany();
                salesOrder = company.GetBusinessObject(BoObjectTypes.oOrders);
                salesOrder.CardCode = "C0037";
                salesOrder.Comments = "Esto es una prueba de creacion de documentos";
                //71 Mayorista
                salesOrder.Series = 71;
                salesOrder.SalesPersonCode = 1;
                salesOrder.DocDueDate = DateTime.Now;

                salesOrder.Lines.ItemCode = "CQ0012-AZU-4";
                salesOrder.Lines.Quantity = 5;
                salesOrder.Lines.TaxCode = "IVA";
                salesOrder.Lines.WarehouseCode = "01";
                salesOrder.Lines.Add();
                salesOrder.Lines.ItemCode = "CA0014-BLA-16";
                salesOrder.Lines.WarehouseCode = "01";
                salesOrder.Lines.Quantity = 2;
                salesOrder.Lines.TaxCode = "IVA";
                salesOrder.Lines.Add();

                // add Sales Order
                if (salesOrder.Add() == 0)
                {
                    message = String.Format("Successfully added Sales Order DocEntry: {0}", company.GetNewObjectKey());
                }
                else
                {
                    message = "Error Code: "
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
                message += e.Message;
            }

            return message;
        }
    }
}
