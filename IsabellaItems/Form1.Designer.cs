namespace IsabellaItems
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.addFile = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.searchArticleTxt = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.searchSizeTxt = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.searchColortxt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.showAllBtn = new System.Windows.Forms.Button();
            this.searchBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.issuedCmb = new System.Windows.Forms.ComboBox();
            this.itemDataGridView = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.getMonthlyReportBtn = new System.Windows.Forms.Button();
            this.placeForMonthlyReport = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.dataGridViewIssuedPlace = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemDataGridView)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIssuedPlace)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1171, 629);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.RoyalBlue;
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1163, 600);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Add Data File";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightBlue;
            this.panel1.Controls.Add(this.addFile);
            this.panel1.Location = new System.Drawing.Point(0, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1167, 597);
            this.panel1.TabIndex = 0;
            // 
            // addFile
            // 
            this.addFile.Location = new System.Drawing.Point(6, 99);
            this.addFile.Name = "addFile";
            this.addFile.Size = new System.Drawing.Size(96, 37);
            this.addFile.TabIndex = 0;
            this.addFile.Text = "Add File";
            this.addFile.UseVisualStyleBackColor = true;
            this.addFile.Click += new System.EventHandler(this.addFile_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.RoyalBlue;
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1163, 600);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Search Items";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightBlue;
            this.panel2.Controls.Add(this.searchArticleTxt);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.searchSizeTxt);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.searchColortxt);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.showAllBtn);
            this.panel2.Controls.Add(this.searchBtn);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.issuedCmb);
            this.panel2.Controls.Add(this.itemDataGridView);
            this.panel2.Location = new System.Drawing.Point(-2, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1167, 597);
            this.panel2.TabIndex = 1;
            // 
            // searchArticleTxt
            // 
            this.searchArticleTxt.Location = new System.Drawing.Point(460, 45);
            this.searchArticleTxt.Name = "searchArticleTxt";
            this.searchArticleTxt.Size = new System.Drawing.Size(132, 22);
            this.searchArticleTxt.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(457, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 17);
            this.label9.TabIndex = 11;
            this.label9.Text = "Article :";
            // 
            // searchSizeTxt
            // 
            this.searchSizeTxt.Location = new System.Drawing.Point(814, 44);
            this.searchSizeTxt.Name = "searchSizeTxt";
            this.searchSizeTxt.Size = new System.Drawing.Size(132, 22);
            this.searchSizeTxt.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(811, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 17);
            this.label8.TabIndex = 9;
            this.label8.Text = "Size :";
            // 
            // searchColortxt
            // 
            this.searchColortxt.Location = new System.Drawing.Point(633, 45);
            this.searchColortxt.Name = "searchColortxt";
            this.searchColortxt.Size = new System.Drawing.Size(132, 22);
            this.searchColortxt.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(630, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 7;
            this.label7.Text = "Color :";
            // 
            // showAllBtn
            // 
            this.showAllBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showAllBtn.Location = new System.Drawing.Point(9, 171);
            this.showAllBtn.Name = "showAllBtn";
            this.showAllBtn.Size = new System.Drawing.Size(107, 24);
            this.showAllBtn.TabIndex = 6;
            this.showAllBtn.Text = "Show All";
            this.showAllBtn.UseVisualStyleBackColor = true;
            this.showAllBtn.Click += new System.EventHandler(this.showAllBtn_Click);
            // 
            // searchBtn
            // 
            this.searchBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBtn.Location = new System.Drawing.Point(1035, 93);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(89, 24);
            this.searchBtn.TabIndex = 5;
            this.searchBtn.Text = "Search";
            this.searchBtn.UseVisualStyleBackColor = true;
            this.searchBtn.Click += new System.EventHandler(this.searchBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Issued Place :";
            // 
            // issuedCmb
            // 
            this.issuedCmb.FormattingEnabled = true;
            this.issuedCmb.Location = new System.Drawing.Point(9, 45);
            this.issuedCmb.Name = "issuedCmb";
            this.issuedCmb.Size = new System.Drawing.Size(248, 24);
            this.issuedCmb.TabIndex = 1;
            // 
            // itemDataGridView
            // 
            this.itemDataGridView.AllowUserToAddRows = false;
            this.itemDataGridView.AllowUserToDeleteRows = false;
            this.itemDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.itemDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.itemDataGridView.Location = new System.Drawing.Point(9, 201);
            this.itemDataGridView.Name = "itemDataGridView";
            this.itemDataGridView.ReadOnly = true;
            this.itemDataGridView.RowTemplate.Height = 24;
            this.itemDataGridView.Size = new System.Drawing.Size(1150, 389);
            this.itemDataGridView.TabIndex = 0;
            this.itemDataGridView.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.itemDataGridView_RowHeaderMouseDoubleClick);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.RoyalBlue;
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1163, 600);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Generate Report";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.LightBlue;
            this.panel3.Controls.Add(this.dateTimePickerTo);
            this.panel3.Controls.Add(this.getMonthlyReportBtn);
            this.panel3.Controls.Add(this.placeForMonthlyReport);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.dateTimePickerFrom);
            this.panel3.Location = new System.Drawing.Point(-2, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1167, 597);
            this.panel3.TabIndex = 1;
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Location = new System.Drawing.Point(282, 77);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(268, 22);
            this.dateTimePickerTo.TabIndex = 4;
            // 
            // getMonthlyReportBtn
            // 
            this.getMonthlyReportBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.getMonthlyReportBtn.Location = new System.Drawing.Point(788, 67);
            this.getMonthlyReportBtn.Name = "getMonthlyReportBtn";
            this.getMonthlyReportBtn.Size = new System.Drawing.Size(122, 32);
            this.getMonthlyReportBtn.TabIndex = 3;
            this.getMonthlyReportBtn.Text = "Get Report";
            this.getMonthlyReportBtn.UseVisualStyleBackColor = true;
            this.getMonthlyReportBtn.Click += new System.EventHandler(this.getMonthlyReportBtn_Click);
            // 
            // placeForMonthlyReport
            // 
            this.placeForMonthlyReport.FormattingEnabled = true;
            this.placeForMonthlyReport.Location = new System.Drawing.Point(564, 75);
            this.placeForMonthlyReport.Name = "placeForMonthlyReport";
            this.placeForMonthlyReport.Size = new System.Drawing.Size(169, 24);
            this.placeForMonthlyReport.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Select Date range";
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Location = new System.Drawing.Point(8, 77);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(268, 22);
            this.dateTimePickerFrom.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.RoyalBlue;
            this.tabPage4.Controls.Add(this.panel4);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1163, 600);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Configuration";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LightBlue;
            this.panel4.Controls.Add(this.splitContainer1);
            this.panel4.Location = new System.Drawing.Point(-2, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1167, 597);
            this.panel4.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.button2);
            this.splitContainer1.Panel2.Controls.Add(this.textBox2);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewIssuedPlace);
            this.splitContainer1.Size = new System.Drawing.Size(1167, 597);
            this.splitContainer1.SplitterDistance = 576;
            this.splitContainer1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 20);
            this.label6.TabIndex = 7;
            this.label6.Text = "Issued Places";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(236, 147);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(159, 24);
            this.button2.TabIndex = 6;
            this.button2.Text = "Add Issued Place";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(13, 149);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(193, 22);
            this.textBox2.TabIndex = 5;
            // 
            // dataGridViewIssuedPlace
            // 
            this.dataGridViewIssuedPlace.AllowUserToAddRows = false;
            this.dataGridViewIssuedPlace.AllowUserToDeleteRows = false;
            this.dataGridViewIssuedPlace.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewIssuedPlace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewIssuedPlace.Location = new System.Drawing.Point(13, 225);
            this.dataGridViewIssuedPlace.Name = "dataGridViewIssuedPlace";
            this.dataGridViewIssuedPlace.ReadOnly = true;
            this.dataGridViewIssuedPlace.RowTemplate.Height = 24;
            this.dataGridViewIssuedPlace.Size = new System.Drawing.Size(522, 365);
            this.dataGridViewIssuedPlace.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1196, 654);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Second Quality Items";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemDataGridView)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIssuedPlace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button addFile;
        private System.Windows.Forms.DataGridView itemDataGridView;
        private System.Windows.Forms.ComboBox issuedCmb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button showAllBtn;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.Button getMonthlyReportBtn;
        private System.Windows.Forms.ComboBox placeForMonthlyReport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.DataGridView dataGridViewIssuedPlace;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox searchColortxt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox searchSizeTxt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox searchArticleTxt;
        private System.Windows.Forms.Label label9;
    }
}

