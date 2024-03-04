using Newtonsoft.Json;
using System.Data;
using System.Runtime.InteropServices;
using NET_LAb_1.Tokens;
using Ydb.Sdk;
using Ydb.Sdk.Services.Table;

namespace NET_LAb_1
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        private string? iamToken;
        private Driver driver;
        private ExecuteDataQueryResponse queryResponse;
        private DataSet dataSetCompany = new DataSet("Company");
        public Form1()
        {
            InitializeComponent();

            if (Config.IsDebugMode)
                AllocConsole();
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
            await CreateIamTokenForServiceAccount();
            var tableClient = new TableClient(driver, new TableClientConfig());

            var response = await tableClient.SessionExec(async session =>
            {
                var query = @"
                    SELECT id, temp
                    FROM test;
                ";
                return await session.ExecuteDataQuery(
                    query: query,
                    txControl: TxControl.BeginSerializableRW()
                ); ;
            });
            response.Status.EnsureSuccess();
            queryResponse = (ExecuteDataQueryResponse)response;
            FillTableTest();
        }
        public void FillTableTest()
        {
            DataTable dataTable = new DataTable();
            DataColumn idColumn = new DataColumn("Id", typeof(UInt64));
            idColumn.Unique = true;
            idColumn.AllowDBNull = false;

            DataColumn tempColumn = new DataColumn("Temp", typeof(string));
            tempColumn.AllowDBNull = false;

            dataTable.Columns.Add(idColumn);
            dataTable.Columns.Add(tempColumn);

            Ydb.Sdk.Value.ResultSet resultSet = queryResponse.Result.ResultSets[0];
            foreach (var row in resultSet.Rows)
            {
                dataTable.Rows.Add(row["id"].GetOptionalUint64(), (string)row["temp"]);
            }
            dataGridView1.DataSource = dataTable;

        }
    }
}
