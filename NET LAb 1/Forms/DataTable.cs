using Newtonsoft.Json;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using NET_LAb_1.Tokens;
using Ydb.Sdk;
using Ydb.Sdk.Services.Table;
using NET_LAb_1.Forms;
using System.Text;

namespace NET_LAb_1
{
    public partial class DataTable : Form
    {
        private string? iamToken;
        private Driver driver;
        private ExecuteDataQueryResponse queryResponse;
        System.Data.DataTable orderListTable;
        System.Data.DataTable customerOrdersTable;
        System.Data.DataTable deliveryNoteTable;
        private string table;
        private bool sl = true;
        public DataTable(string table)
        {
            this.table = table;
            InitializeComponent();
            switch (table)
            {
                case "delivery_note":
                    dataGridView1.Dock = DockStyle.Fill;
                    break;

                case "customer_orders":
                    ToolStripMenuItem_add.Visible = false;
                    ToolStripMenuItem_edit.Visible = false;
                    ToolStripMenuItem_delete.Visible = false;
                    ToolStripMenuItem_report.Visible = true;
                    dataGridView2.Visible = true;
                    break;
            }
            ResponseYdbTest(table);
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
        public async Task ResponseYdbTest(string table)
        {
            await CreateIamTokenForServiceAccount();
            var tableClient = new TableClient(driver, new TableClientConfig());

            var response = await tableClient.SessionExec(async session =>
            {

                var query = @$"SELECT * FROM {table};";
                switch (this.table)
                {
                    case "customer_orders":
                        query = @$"SELECT `a.id` as id, `date_order`, `b.name` as name, `total`, `status` 
                            FROM customer_orders a LEFT JOIN customers as b
                            ON a.id_customer = b.id;
                            
                            SELECT `a.id` as id, `name`, `quantity`, `total`, `remaining` 
                            FROM order_list as a LEFT JOIN products as b
                            ON a.id_product = b.id;
                            ;";
                        break;
                    case "delivery_note":
                        query = $@"
                            SELECT dn.id as id, dn.id_product as id_product, 
                                dn.id_employee as id_employee, date, quantity, total,
                                p.name as product, e.name as employee
                            FROM delivery_note as dn
                            LEFT JOIN employees as e
                                ON dn.id_employee = e.id
                            LEFT JOIN products as p
                                ON dn.id_product = p.id
;
                        ";
                        break;
                }

                return await session.ExecuteDataQuery(
                    query: query,
                    txControl: TxControl.BeginSerializableRW()
                ); ;
            });
            response.Status.EnsureSuccess();
            queryResponse = (ExecuteDataQueryResponse)response;
            switch (this.table)
            {
                case "customer_orders":
                    customerOrdersFillTable();
                    break;

                case "delivery_note":
                    deliveryNotesFillTable();
                    break;
            }
        }
        public void customerOrdersFillTable()
        {
            try
            {
                customerOrdersTable = new System.Data.DataTable();

                DataColumn idColumn = new DataColumn("Id", typeof(UInt64));
                idColumn.Unique = true;
                idColumn.AllowDBNull = false;
                idColumn.ReadOnly = true;

                DataColumn dateOrderColumn = new DataColumn("Дата заказа", typeof(DateTime));
                dateOrderColumn.AllowDBNull = false;
                dateOrderColumn.ReadOnly = true;

                DataColumn customerColumn = new DataColumn("Имя", typeof(string));
                customerColumn.AllowDBNull = false;
                customerColumn.ReadOnly = true;

                DataColumn totalColumn = new DataColumn("Сумма", typeof(Double));
                totalColumn.AllowDBNull = false;
                totalColumn.ReadOnly = true;

                DataColumn statusColumn = new DataColumn("Статус", typeof(string));
                statusColumn.AllowDBNull = false;
                statusColumn.ReadOnly = true;

                customerOrdersTable.Columns.Add(idColumn);
                customerOrdersTable.Columns.Add(dateOrderColumn);
                customerOrdersTable.Columns.Add(customerColumn);
                customerOrdersTable.Columns.Add(totalColumn);
                customerOrdersTable.Columns.Add(statusColumn);

                Ydb.Sdk.Value.ResultSet resultSet = queryResponse.Result.ResultSets[0];
                foreach (var row in resultSet.Rows)
                {
                    customerOrdersTable.Rows.Add(
                        row["id"].GetOptionalUint64(),
                        row["date_order"].GetOptionalDatetime(),
                        Encoding.UTF8.GetString(row["name"].GetOptionalString()),
                        row["total"].GetOptionalDouble(),
                        Encoding.UTF8.GetString(row["status"].GetOptionalString())
                        );
                    Console.WriteLine(row["date_order"].GetOptionalDatetime());
                }
                dataGridView1.DataSource = customerOrdersTable;
                sl = false;
                selectorDGV();
                sl = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void deliveryNotesFillTable()
        {
            try
            {
                deliveryNoteTable = new System.Data.DataTable();

                DataColumn idColumn = new DataColumn("id", typeof(UInt64));
                idColumn.Unique = true;
                idColumn.AllowDBNull = false;
                idColumn.ReadOnly = true;

                DataColumn idProductColumn = new DataColumn("id_product", typeof(UInt64));
                idProductColumn.AllowDBNull = false;
                idProductColumn.ReadOnly = true;

                DataColumn idEmployeeColumn = new DataColumn("id_employee", typeof(UInt64));
                idProductColumn.AllowDBNull = false;
                idProductColumn.ReadOnly = true;

                DataColumn productColumn = new DataColumn("Товар", typeof(string));
                productColumn.AllowDBNull = false;
                productColumn.ReadOnly = true;

                DataColumn employeeColumn = new DataColumn("Сотрудник", typeof(string));
                employeeColumn.AllowDBNull = false;
                employeeColumn.ReadOnly = true;

                DataColumn dateColumn = new DataColumn("Дата", typeof(DateTime));
                dateColumn.AllowDBNull = false;
                dateColumn.ReadOnly = true;

                DataColumn quantityolumn = new DataColumn("Количество", typeof(UInt64));
                quantityolumn.AllowDBNull = false;
                quantityolumn.ReadOnly = true;

                DataColumn totalColumn = new DataColumn("Сумма", typeof(Double));
                totalColumn.AllowDBNull = false;
                totalColumn.ReadOnly = true;
                deliveryNoteTable.Columns.Add(idColumn);
                deliveryNoteTable.Columns.Add(idProductColumn);
                deliveryNoteTable.Columns.Add(idEmployeeColumn);
                deliveryNoteTable.Columns.Add(productColumn);
                deliveryNoteTable.Columns.Add(employeeColumn);
                deliveryNoteTable.Columns.Add(dateColumn);
                deliveryNoteTable.Columns.Add(quantityolumn);
                deliveryNoteTable.Columns.Add(totalColumn);

                Ydb.Sdk.Value.ResultSet resultSet = queryResponse.Result.ResultSets[0];
                foreach (var row in resultSet.Rows)
                {
                    deliveryNoteTable.Rows.Add(
                        row["id"].GetOptionalUint64(),
                        row["id_product"].GetOptionalUint64(),
                        row["id_employee"].GetOptionalUint64(),
                        Encoding.UTF8.GetString(row["product"].GetOptionalString()),
                        Encoding.UTF8.GetString(row["employee"].GetOptionalString()),
                        row["date"].GetOptionalDatetime(),
                        row["quantity"].GetOptionalUint64(),
                        row["total"].GetOptionalDouble()
                        );
                }
                dataGridView1.DataSource = deliveryNoteTable;
                dataGridView1.Columns["id_product"].Visible = false;
                dataGridView1.Columns["id_employee"].Visible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void updateForm()
        {
            ResponseYdbTest(this.table);
            this.Show();
        }

        public void selectorDGV()
        {
            if (dataGridView1.Rows.Count == 0)
                return;
            ulong id = Convert.ToUInt64(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

            try
            {
                orderListTable = new System.Data.DataTable();

                DataColumn idColumn = new DataColumn("Id", typeof(UInt64));
                idColumn.Unique = true;
                idColumn.AllowDBNull = false;
                idColumn.ReadOnly = true;

                DataColumn productColumn = new DataColumn("Товар", typeof(string));
                productColumn.AllowDBNull = false;
                productColumn.ReadOnly = true;

                DataColumn quantityColumn = new DataColumn("Количество", typeof(UInt64));
                quantityColumn.AllowDBNull = false;
                quantityColumn.ReadOnly = true;

                DataColumn totalColumn = new DataColumn("Сумма", typeof(Double));
                totalColumn.AllowDBNull = false;
                totalColumn.ReadOnly = true;

                DataColumn remainingColumn = new DataColumn("Остаток", typeof(UInt64));
                remainingColumn.AllowDBNull = false;
                remainingColumn.ReadOnly = true;

                orderListTable.Columns.Add(idColumn);
                orderListTable.Columns.Add(productColumn);
                orderListTable.Columns.Add(quantityColumn);
                orderListTable.Columns.Add(totalColumn);
                orderListTable.Columns.Add(remainingColumn);

                Ydb.Sdk.Value.ResultSet resultSet = queryResponse.Result.ResultSets[1];
                foreach (var row in resultSet.Rows)
                {
                    orderListTable.Rows.Add(
                        row["id"].GetOptionalUint64(),
                        Encoding.UTF8.GetString(row["name"].GetOptionalString()),
                        row["quantity"].GetOptionalUint64(),
                        row["total"].GetOptionalDouble(),
                        row["remaining"].GetOptionalUint64()
                        );
                }
                dataGridView2.DataSource = orderListTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ToolStripMenuItem_add_Click(object sender, EventArgs e)
        {
            Form addForm;
            switch (this.table)
            {
                case "delivery_note":
                    addForm = new DeliveriesForm((uint)dataGridView1.Rows.Count + 1, "add", iamToken, driver);
                    addForm.FormClosed += (s, e) => updateForm();
                    this.Hide();
                    addForm.ShowDialog(this);
                    break;
            }
        }

        private void ToolStripMenuItem_edit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            Form editForm;
            switch (this.table)
            {
                case "delivery_note":
                    editForm = new DeliveriesForm("edit", iamToken, driver, dataGridView1.SelectedRows[0]);
                    editForm.FormClosed += (s, e) => updateForm();
                    this.Hide();
                    editForm.ShowDialog(this);
                    break;
            }
        }

        private void ToolStripMenuItem_report_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel._Worksheet worksheet = workbook.Sheets[1];

            // Добавление заголовков столбцов
            for (int i = 0; i < customerOrdersTable.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = customerOrdersTable.Columns[i].ColumnName;
            }

            for (int i = 0; i < orderListTable.Columns.Count; i++)
            {
                worksheet.Cells[4, i + 1] = orderListTable.Columns[i].ColumnName;
            }

            // Добавление данных из DataTable
            for (int i = 0; i < orderListTable.Rows.Count; i++)
            {
                for (int j = 0; j < orderListTable.Columns.Count; j++)
                {
                    worksheet.Cells[5 + i, j + 1] = orderListTable.Rows[i][j].ToString();
                }
            }
            for (int i = 0; i < customerOrdersTable.Columns.Count; i++)
            {
                worksheet.Cells[2, i + 1] = dataGridView1.SelectedRows[0].Cells[i].Value.ToString();
            }

            workbook.Close();
            excelApp.Quit();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (sl && table == "customer_orders")
                selectorDGV();
        }

        private void ToolStripMenuItem_delete_Click(object sender, EventArgs e)
        {
            ResponseYdbDelete();
        }

        public async Task ResponseYdbDelete()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            ulong deleteId = Convert.ToUInt64(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            await CreateIamTokenForServiceAccount();
            var tableClient = new TableClient(driver, new TableClientConfig());

            var response = await tableClient.SessionExec(async session =>
            {
                var query = @$"DELETE FROM delivery_note WHERE id = {deleteId};";
                return await session.ExecuteDataQuery(
                    query: query,
                    txControl: TxControl.BeginSerializableRW()
                ); ;
            });
            response.Status.EnsureSuccess();
            queryResponse = (ExecuteDataQueryResponse)response;
            updateForm();
        }
    }
}
