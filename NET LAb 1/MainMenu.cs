using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NET_LAb_1
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void button_customersOrders_Click(object sender, EventArgs e)
        {
            Form formOrders = new DataTable("customer_orders");
            formOrders.Show();
        }

        private void button_deliveryNote_Click(object sender, EventArgs e)
        {
            Form formDeliveries = new DataTable("delivery_note");
            formDeliveries.Show();
        }

        private void button_productReport_Click(object sender, EventArgs e)
        {

        }

        private void button_customerReport_Click(object sender, EventArgs e)
        {

        }
    }
}
