namespace NET_LAb_1.Forms
{
    partial class DeliveriesForm
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
            button_addEdit = new Button();
            panel1 = new Panel();
            numericUpDown_total = new NumericUpDown();
            label_total = new Label();
            numericUpDown_quantity = new NumericUpDown();
            label_quantity = new Label();
            label_employee = new Label();
            comboBox_employee = new ComboBox();
            label_product = new Label();
            comboBox_product = new ComboBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_total).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_quantity).BeginInit();
            SuspendLayout();
            // 
            // button_addEdit
            // 
            button_addEdit.Location = new Point(176, 266);
            button_addEdit.Name = "button_addEdit";
            button_addEdit.Size = new Size(94, 29);
            button_addEdit.TabIndex = 5;
            button_addEdit.Text = "Действие";
            button_addEdit.UseVisualStyleBackColor = true;
            button_addEdit.Click += button_addEdit_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(numericUpDown_total);
            panel1.Controls.Add(label_total);
            panel1.Controls.Add(numericUpDown_quantity);
            panel1.Controls.Add(label_quantity);
            panel1.Controls.Add(label_employee);
            panel1.Controls.Add(comboBox_employee);
            panel1.Controls.Add(label_product);
            panel1.Controls.Add(comboBox_product);
            panel1.Controls.Add(button_addEdit);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(358, 325);
            panel1.TabIndex = 1;
            // 
            // numericUpDown_total
            // 
            numericUpDown_total.DecimalPlaces = 2;
            numericUpDown_total.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown_total.Location = new Point(29, 223);
            numericUpDown_total.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            numericUpDown_total.Name = "numericUpDown_total";
            numericUpDown_total.Size = new Size(150, 27);
            numericUpDown_total.TabIndex = 4;
            // 
            // label_total
            // 
            label_total.AutoSize = true;
            label_total.Location = new Point(29, 200);
            label_total.Name = "label_total";
            label_total.Size = new Size(55, 20);
            label_total.TabIndex = 8;
            label_total.Text = "Сумма";
            // 
            // numericUpDown_quantity
            // 
            numericUpDown_quantity.Location = new Point(29, 162);
            numericUpDown_quantity.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown_quantity.Name = "numericUpDown_quantity";
            numericUpDown_quantity.Size = new Size(150, 27);
            numericUpDown_quantity.TabIndex = 3;
            // 
            // label_quantity
            // 
            label_quantity.AutoSize = true;
            label_quantity.Location = new Point(29, 139);
            label_quantity.Name = "label_quantity";
            label_quantity.Size = new Size(90, 20);
            label_quantity.TabIndex = 6;
            label_quantity.Text = "Количество";
            // 
            // label_employee
            // 
            label_employee.AutoSize = true;
            label_employee.Location = new Point(29, 80);
            label_employee.Name = "label_employee";
            label_employee.Size = new Size(82, 20);
            label_employee.TabIndex = 4;
            label_employee.Text = "Сотрудник";
            // 
            // comboBox_employee
            // 
            comboBox_employee.FormattingEnabled = true;
            comboBox_employee.Location = new Point(29, 103);
            comboBox_employee.Name = "comboBox_employee";
            comboBox_employee.Size = new Size(294, 28);
            comboBox_employee.Sorted = true;
            comboBox_employee.TabIndex = 2;
            // 
            // label_product
            // 
            label_product.AutoSize = true;
            label_product.Location = new Point(29, 20);
            label_product.Name = "label_product";
            label_product.Size = new Size(51, 20);
            label_product.TabIndex = 2;
            label_product.Text = "Товар";
            // 
            // comboBox_product
            // 
            comboBox_product.FormattingEnabled = true;
            comboBox_product.Location = new Point(29, 43);
            comboBox_product.Name = "comboBox_product";
            comboBox_product.Size = new Size(294, 28);
            comboBox_product.Sorted = true;
            comboBox_product.TabIndex = 1;
            // 
            // DeliveriesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Name = "DeliveriesForm";
            Text = "DeliveriesForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_total).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_quantity).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button_addEdit;
        private Panel panel1;
        private ComboBox comboBox_product;
        private Label label_product;
        private Label label_employee;
        private ComboBox comboBox_employee;
        private NumericUpDown numericUpDown_quantity;
        private Label label_quantity;
        private NumericUpDown numericUpDown_total;
        private Label label_total;
    }
}