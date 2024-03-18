using NET_LAb_1.Tokens;
using Newtonsoft.Json;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text;
using Ydb.Sdk.Services.Table;
using Ydb.Sdk;
using System.Data;

namespace NET_LAb_1.Forms
{
    public partial class CustomersReport : Form
    {
        private string iamToken;
        private Driver driver;
        private ExecuteDataQueryResponse queryResponse;
        public CustomersReport()
        {
            InitializeComponent();
            fillCustomerTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            ResponseYdbReport();
        }

        public async Task fillCustomerTable()
        {
            try
            {
                System.Data.DataTable customersTable = new System.Data.DataTable();

                DataColumn idColumn = new DataColumn("Id", typeof(UInt64));
                idColumn.Unique = true;
                idColumn.AllowDBNull = false;
                idColumn.ReadOnly = true;

                DataColumn customerColumn = new DataColumn("Имя", typeof(string));
                customerColumn.AllowDBNull = false;
                customerColumn.ReadOnly = true;

                customersTable.Columns.Add(idColumn);
                customersTable.Columns.Add(customerColumn);

                await CreateIamTokenForServiceAccount();
                var tableClient = new TableClient(driver, new TableClientConfig());

                var response = await tableClient.SessionExec(async session =>
                {
                    var query = @$"
                        SELECT id, name FROM customers
                        ;";
                    return await session.ExecuteDataQuery(
                        query: query,
                        txControl: TxControl.BeginSerializableRW().Commit()
                    );
                });
                response.Status.EnsureSuccess();
                queryResponse = (ExecuteDataQueryResponse)response;


                Ydb.Sdk.Value.ResultSet resultSet = queryResponse.Result.ResultSets[0];
                foreach (var row in resultSet.Rows)
                {
                    customersTable.Rows.Add(
                        row["id"].GetOptionalUint64(),
                        Encoding.UTF8.GetString(row["name"].GetOptionalString())
                        );
                }
                dataGridView1.DataSource = customersTable;
                dataGridView1.Columns[0].Visible = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task CreateIamTokenForServiceAccount()
        {
            // Чтение файла ключа и получение IAM-токена
            string json = File.ReadAllText(Config.keyPath);
            ServiceAccount account = JsonConvert.DeserializeObject<ServiceAccount>(json);
            try
            {
                var _ = new IAMToken();
                this.iamToken = await _.createAsync(account.private_key,
                                                    account.id,
                                                    account.service_account_id);

                // Обращение к YaDB
                var config = new DriverConfig(
                endpoint: Config.UserEndpoint,
                    database: Config.UserDatabasePath,
                    credentials: new Ydb.Sdk.Auth.TokenProvider(iamToken)
                );
                driver = new Driver(config: config);
                await driver.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task ResponseYdbReport()
        {
            try
            {
                string customersReq = "";
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    customersReq += "c.id = " + row.Cells[0].Value.ToString() + " or ";
                }
                customersReq = customersReq.Substring(0, customersReq.Length - 3);
                await CreateIamTokenForServiceAccount();
                var tableClient = new TableClient(driver, new TableClientConfig());

                var response = await tableClient.SessionExec(async session =>
                {
                    var query = @$"
                        SELECT c.name as name, SUM(ol.total) as total
                            FROM customers as c
                            LEFT JOIN customer_orders as co
                                ON c.id = co.id_customer
                            LEFT JOIN order_list as ol
                                ON co.id = ol.id_order
                            LEFT JOIN products as p
                                ON ol.id_product = p.id
                            WHERE {customersReq}
                            GROUP BY c.name
                        ;

                        SELECT c.name as name, SUM(ol.total) as total
                            FROM customers as c
                            LEFT JOIN customer_orders as co
                                ON c.id = co.id_customer
                            LEFT JOIN order_list as ol
                                ON co.id = ol.id_order
                            LEFT JOIN products as p
                                ON ol.id_product = p.id
                            WHERE ol.remaining = 0
                                and ({customersReq})
                            GROUP BY c.name
                        ;";
                    return await session.ExecuteDataQuery(
                        query: query,
                        txControl: TxControl.BeginSerializableRW().Commit()
                    );
                });
                response.Status.EnsureSuccess();
                queryResponse = (ExecuteDataQueryResponse)response;

                Ydb.Sdk.Value.ResultSet resultSet1 = queryResponse.Result.ResultSets[0];
                Ydb.Sdk.Value.ResultSet resultSet2 = queryResponse.Result.ResultSets[1];

                if (resultSet1.Rows.Count == 0 && resultSet2.Rows.Count == 0)
                {
                    MessageBox.Show("Записи по критерию не найдены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel._Worksheet worksheet = workbook.Sheets[1];

                //Добавление заголовков столбцов
                worksheet.Cells[1, 1] = "Сумма заказов";
                worksheet.Cells[2, 1] = "ФИО";
                worksheet.Cells[2, 2] = "Сумма";

                worksheet.Cells[1, 6] = "Сумма доставок";
                worksheet.Cells[2, 6] = "ФИО";
                worksheet.Cells[2, 7] = "Сумма";

                //Добавление данных
                int i = 0;
                foreach (var row in resultSet1.Rows)
                {
                    worksheet.Cells[3 + i, 1] = Encoding.UTF8.GetString(row["name"].GetOptionalString());
                    worksheet.Cells[3 + i, 2] = row["total"].GetOptionalDouble();
                    i++;
                }
                i = 0;
                foreach (var row in resultSet2.Rows)
                {
                    if (row["total"].GetOptionalDouble() != 0)
                    {
                        worksheet.Cells[3 + i, 6] = Encoding.UTF8.GetString(row["name"].GetOptionalString());
                        worksheet.Cells[3 + i, 7] = row["total"].GetOptionalDouble();
                    }
                    i++;
                }

                workbook.Close();
                excelApp.Quit();
                this.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
