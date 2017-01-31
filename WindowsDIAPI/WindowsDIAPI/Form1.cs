using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsDIAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerConnection s = new ServerConnection();
            if(s.Connect() == 0)
            {
                this.label1.Text = s.GetCompany().CompanyName;
                s.Disconnect();
            }
            else
            {
                MessageBox.Show("Error Code", s.GetErrorCode() + "Message:" + s.GetErrorMessage());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SalesOrder order = new SalesOrder();
            if (order.ServerConnection.Connect() == 0)
            {
                String message = order.AddSalesOrder();
                label2.Text = message;
                order.ServerConnection.Disconnect();
            }
            else
            {
                MessageBox.Show("Error Code", order.ServerConnection.GetErrorCode() + "Message:" + order.ServerConnection.GetErrorMessage());
            }

        }
    }
}
