using NET_LAb_1.Forms;

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
            formDeliveries.ShowDialog(this);
        }

        private void button_productReport_Click(object sender, EventArgs e)
        {
            Form report = new ProductsReport();
            report.ShowDialog(this);
        }

        private void button_customerReport_Click(object sender, EventArgs e)
        {
            Form report = new CustomersReport();
            report.ShowDialog(this);
        }
    }
}
