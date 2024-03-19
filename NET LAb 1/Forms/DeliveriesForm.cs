using System.Text;
using Ydb.Sdk.Services.Table;
using Ydb.Sdk;
using Ydb.Sdk.Value;

namespace NET_LAb_1.Forms
{
    public partial class DeliveriesForm : Form
    {
        ulong id;
        string mode;
        ulong id_product;
        ulong id_employee;
        DateTime date;
        ulong quantity;
        double total;
        Driver driver;
        List<Element> productsList = new List<Element>();
        List<Element> employeesList = new List<Element>();
        public DeliveriesForm(ulong id, string mode, string iamtoken, Driver driver)
        {
            InitializeComponent();
            this.id = id;
            this.mode = mode;
            this.driver = driver;

            button_addEdit.Text = "Добавить";
            responseYdbProductAndEmployees();
        }

        public DeliveriesForm(string mode, string iamtoken, Driver driver, DataGridViewRow selectedRow)
        {
            InitializeComponent();
            this.id = Convert.ToUInt64(selectedRow.Cells[0].Value.ToString());
            this.id_product = Convert.ToUInt64(selectedRow.Cells[1].Value.ToString());
            this.id_employee = Convert.ToUInt64(selectedRow.Cells[2].Value.ToString());
            this.date = Convert.ToDateTime(selectedRow.Cells[5].Value.ToString());
            numericUpDown_quantity.Value = Convert.ToUInt64(selectedRow.Cells[6].Value.ToString());
            numericUpDown_total.Value = Convert.ToDecimal(selectedRow.Cells[7].Value.ToString());
            this.mode = mode;
            this.driver = driver;

            button_addEdit.Text = "Изменить";
            responseYdbProductAndEmployees();
        }

        private void button_addEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ulong idProduct = 1;
                ulong idEmployee = 1;
                if (comboBox_product.SelectedItem == null || comboBox_employee.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля", "Ошибка",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                foreach (Element element in productsList)
                    if (element.name == comboBox_product.SelectedItem.ToString())
                        idProduct = element.id;

                foreach (Element element in employeesList)
                    if (element.name == comboBox_employee.SelectedItem.ToString())
                        idEmployee = element.id;

                switch (mode)
                {
                    case "add":
                        SendResponse(
                            id,
                            idProduct,
                            idEmployee,
                            DateTime.Now,
                            Convert.ToUInt64(numericUpDown_quantity.Value),
                            Convert.ToDouble(numericUpDown_total.Value));
                        break;
                    case "edit":
                        SendResponse(
                            id,
                            idProduct,
                            idEmployee,
                            date,
                            Convert.ToUInt64(numericUpDown_quantity.Value),
                            Convert.ToDouble(numericUpDown_total.Value));
                        break;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public async Task SendResponse(ulong id, ulong idProduct, ulong idEmployee, DateTime date, ulong quantity, Double total)
        {
            try
            {
                var tableClient = new TableClient(driver, new TableClientConfig());

                var response = await tableClient.SessionExec(async session =>
                {
                    var query = @"
                    DECLARE $id AS Uint64;
                    DECLARE $id_product AS Uint64;
                    DECLARE $id_employee AS Uint64;
                    DECLARE $date AS DateTime;
                    DECLARE $quantity AS Uint64;
                    DECLARE $total AS Double;

                    UPSERT INTO `delivery_note`
                    ( `id`, `date`, `id_employee`, `id_product`, `quantity`, `total` )
                    VALUES ($id, $date, $id_employee, $id_product, $ quantity, $total);
                ";
                    return await session.ExecuteDataQuery(
                        query: query,
                        txControl: TxControl.BeginSerializableRW().Commit(),
                        parameters: new Dictionary<string, YdbValue>{
                        { "$id", YdbValue.MakeUint64(id) },
                        { "$date", YdbValue.MakeDatetime(date)},
                        { "$id_product", YdbValue.MakeUint64(idProduct)},
                        { "$id_employee", YdbValue.MakeUint64(idEmployee)},
                        { "$quantity", YdbValue.MakeUint64(quantity)},
                        { "$total", YdbValue.MakeDouble(total)}
                        }
                    );
                });

                response.Status.EnsureSuccess();
                if (response.Status.StatusCode == StatusCode.Success)
                {
                    MessageBox.Show("Данные сохранены.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
        public async Task responseYdbProductAndEmployees()
        {
            try
            {

                var tableClient = new TableClient(driver, new TableClientConfig());

                var response = await tableClient.SessionExec(async session =>
                {

                    var query = @"SELECT * FROM products;";

                    return await session.ExecuteDataQuery(
                        query: query,
                        txControl: TxControl.BeginSerializableRW()
                    ); ;
                });
                response.Status.EnsureSuccess();
                ExecuteDataQueryResponse queryResponse = (ExecuteDataQueryResponse)response;

                Ydb.Sdk.Value.ResultSet resultSet = queryResponse.Result.ResultSets[0];

                foreach (var row in resultSet.Rows)
                {
                    Element el = new Element((ulong)row["id"].GetOptionalUint64(), Encoding.UTF8.GetString(row["name"].GetOptionalString()));
                    productsList.Add(el);
                    comboBox_product.Items.Add(el.name);
                }
                if (mode == "edit" && id_product != null)
                {
                    comboBox_product.SelectedIndex = comboBox_product.Items.IndexOf(productsList[(int)id_product - 1].name);
                }

                tableClient = new TableClient(driver, new TableClientConfig());

                response = await tableClient.SessionExec(async session =>
                {

                    var query = @"SELECT * FROM employees;";

                    return await session.ExecuteDataQuery(
                        query: query,
                        txControl: TxControl.BeginSerializableRW()
                    ); ;
                });
                response.Status.EnsureSuccess();
                queryResponse = (ExecuteDataQueryResponse)response;

                resultSet = queryResponse.Result.ResultSets[0];


                foreach (var row in resultSet.Rows)
                {
                    Element el = new Element((ulong)row["id"].GetOptionalUint64(), Encoding.UTF8.GetString(row["name"].GetOptionalString()));
                    employeesList.Add(el);
                    comboBox_employee.Items.Add(el.name);
                }

                if (mode == "edit" && id_employee != null)
                {
                    comboBox_employee.SelectedIndex = comboBox_employee.Items.IndexOf(employeesList[(int)id_employee - 1].name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    class Element
    {
        public UInt64 id { get; set; }
        public string name { get; set; }

        public Element(UInt64 id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}