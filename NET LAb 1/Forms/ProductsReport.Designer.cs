namespace NET_LAb_1.Forms
{
    partial class ProductsReport
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
            dateTimePicker_first = new DateTimePicker();
            label_first = new Label();
            button_makeReport = new Button();
            label_second = new Label();
            dateTimePicker_second = new DateTimePicker();
            SuspendLayout();
            // 
            // dateTimePicker_first
            // 
            dateTimePicker_first.Location = new Point(57, 82);
            dateTimePicker_first.Name = "dateTimePicker_first";
            dateTimePicker_first.Size = new Size(250, 27);
            dateTimePicker_first.TabIndex = 0;
            dateTimePicker_first.Value = new DateTime(2024, 3, 18, 21, 24, 23, 0);
            // 
            // label_first
            // 
            label_first.AutoSize = true;
            label_first.Location = new Point(57, 48);
            label_first.Name = "label_first";
            label_first.Size = new Size(112, 20);
            label_first.TabIndex = 2;
            label_first.Text = "С какого числа";
            // 
            // button_makeReport
            // 
            button_makeReport.Location = new Point(88, 265);
            button_makeReport.Name = "button_makeReport";
            button_makeReport.Size = new Size(176, 42);
            button_makeReport.TabIndex = 2;
            button_makeReport.Text = "Сформировать";
            button_makeReport.UseVisualStyleBackColor = true;
            button_makeReport.Click += button_makeReport_Click;
            // 
            // label_second
            // 
            label_second.AutoSize = true;
            label_second.Location = new Point(56, 151);
            label_second.Name = "label_second";
            label_second.Size = new Size(117, 20);
            label_second.TabIndex = 5;
            label_second.Text = "По какое число";
            // 
            // dateTimePicker_second
            // 
            dateTimePicker_second.Location = new Point(56, 185);
            dateTimePicker_second.Name = "dateTimePicker_second";
            dateTimePicker_second.Size = new Size(250, 27);
            dateTimePicker_second.TabIndex = 1;
            dateTimePicker_second.Value = new DateTime(2024, 3, 18, 21, 24, 23, 0);
            // 
            // ProductsReport
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(363, 362);
            Controls.Add(label_second);
            Controls.Add(dateTimePicker_second);
            Controls.Add(button_makeReport);
            Controls.Add(label_first);
            Controls.Add(dateTimePicker_first);
            Name = "ProductsReport";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ProductsReport";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker dateTimePicker_first;
        private Label label_first;
        private Button button_makeReport;
        private Label label_second;
        private DateTimePicker dateTimePicker_second;
    }
}