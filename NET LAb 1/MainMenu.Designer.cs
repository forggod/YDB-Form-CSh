namespace NET_LAb_1
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_customersOrders = new Button();
            button_deliveryNote = new Button();
            button_productReport = new Button();
            button_customerReport = new Button();
            SuspendLayout();
            // 
            // button_customersOrders
            // 
            button_customersOrders.Location = new Point(50, 25);
            button_customersOrders.Name = "button_customersOrders";
            button_customersOrders.Size = new Size(170, 40);
            button_customersOrders.TabIndex = 0;
            button_customersOrders.Text = "Заказы клиентов";
            button_customersOrders.UseVisualStyleBackColor = true;
            button_customersOrders.Click += button_customersOrders_Click;
            // 
            // button_deliveryNote
            // 
            button_deliveryNote.Location = new Point(50, 81);
            button_deliveryNote.Name = "button_deliveryNote";
            button_deliveryNote.Size = new Size(170, 40);
            button_deliveryNote.TabIndex = 1;
            button_deliveryNote.Text = "Прибытие";
            button_deliveryNote.UseVisualStyleBackColor = true;
            button_deliveryNote.Click += button_deliveryNote_Click;
            // 
            // button_productReport
            // 
            button_productReport.Location = new Point(50, 137);
            button_productReport.Name = "button_productReport";
            button_productReport.Size = new Size(170, 40);
            button_productReport.TabIndex = 2;
            button_productReport.Text = "Отчет по товарам";
            button_productReport.UseVisualStyleBackColor = true;
            button_productReport.Click += button_productReport_Click;
            // 
            // button_customerReport
            // 
            button_customerReport.Location = new Point(50, 193);
            button_customerReport.Name = "button_customerReport";
            button_customerReport.Size = new Size(170, 40);
            button_customerReport.TabIndex = 3;
            button_customerReport.Text = "Отчет по клиентам";
            button_customerReport.UseVisualStyleBackColor = true;
            button_customerReport.Click += button_customerReport_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(272, 253);
            Controls.Add(button_customerReport);
            Controls.Add(button_productReport);
            Controls.Add(button_deliveryNote);
            Controls.Add(button_customersOrders);
            Name = "MainMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainMenu";
            ResumeLayout(false);
        }

        #endregion

        private Button button_customersOrders;
        private Button button_deliveryNote;
        private Button button_productReport;
        private Button button_customerReport;
    }
}