using NET_LAb_1.Tokens;
using Newtonsoft.Json;
using Ydb.Sdk.Services.Table;
using Excel = Microsoft.Office.Interop.Excel;
using Ydb.Sdk;
using Ydb.Sdk.Value;
using System.Text;

namespace NET_LAb_1.Forms
{
    public partial class ProductsReport : Form
    {
        private string iamToken;
        private Driver driver;
        private ExecuteDataQueryResponse queryResponse;
        public ProductsReport()
        {
            InitializeComponent();
        }

        private void button_makeReport_Click(object sender, EventArgs e)
        {
            DateTime dateFirst = dateTimePicker_first.Value;
            DateTime dateSecond = dateTimePicker_second.Value;

            if (dateFirst > dateSecond) return;

            ResponseYdbTest();
        }
        public async Task CreateIamTokenForServiceAccount()
        {
            // Чтение файла ключа и получение IAM-токена
            string json = File.ReadAllText(Config.keyPath);
            ServiceAccount account = JsonConvert.DeserializeObject<ServiceAccount>(json);
            try
            {
                if (Config.IsWriteKey)
                {
                    Console.WriteLine($"id: {account.id}");
                    Console.WriteLine($"service account id: {account.service_account_id}");
                    Console.WriteLine($"created at: {account.created_at}");
                    Console.WriteLine($"key algorithm: {account.key_algorithm}");
                    Console.WriteLine($"public key: {account.public_key}");
                    Console.WriteLine($"private key: {account.private_key}");
                }
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
        public async Task ResponseYdbTest()
        {
            try
            {
                await CreateIamTokenForServiceAccount();
                var tableClient = new TableClient(driver, new TableClientConfig());

                var response = await tableClient.SessionExec(async session =>
                {
                    var query = @"
                    DECLARE $dateFirst AS DateTime;
                    DECLARE $dateSecond AS DateTime;

                    SELECT c.name as name, SUM(a.total) as total
                        FROM order_list as a LEFT JOIN customer_orders as b
                            ON a.id_order = b.id
                        LEFT JOIN products as c
                            ON a.id_product = c.id
                        WHERE b.date_order >= $dateFirst and
                            b.date_order <= $dateSecond
                        GROUP BY c.name;
    
                    SELECT c.name as name, SUM(a.total) as total
                        FROM order_list as a LEFT JOIN customer_orders as b
                            ON a.id_order = b.id
                        LEFT JOIN products as c
                            ON a.id_product = c.id
                        WHERE a.remaining = 0 and
                            b.date_order >= $dateFirst and
                            b.date_order <= $dateSecond
                        GROUP BY c.name;
                ";
                    return await session.ExecuteDataQuery(
                        query: query,
                        txControl: TxControl.BeginSerializableRW().Commit(),
                        parameters: new Dictionary<string, YdbValue>{
                        { "$dateFirst", YdbValue.MakeDatetime(dateTimePicker_first.Value)},
                        { "$dateSecond", YdbValue.MakeDatetime(dateTimePicker_second.Value)}
                        }
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
                worksheet.Cells[2, 1] = "Товар";
                worksheet.Cells[2, 2] = "Сумма";

                worksheet.Cells[1, 6] = "Сумма доставок";
                worksheet.Cells[2, 6] = "Товар";
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
                    worksheet.Cells[3 + i, 6] = Encoding.UTF8.GetString(row["name"].GetOptionalString());
                    worksheet.Cells[3 + i, 7] = row["total"].GetOptionalDouble();
                }

                workbook.Close();
                excelApp.Quit();
                this.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
