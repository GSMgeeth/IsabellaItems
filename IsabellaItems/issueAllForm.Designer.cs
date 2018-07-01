namespace IsabellaItems
{
    partial class issueAllForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.IssueBtn = new System.Windows.Forms.Button();
            this.IssueingPlaceCmb = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightBlue;
            this.panel1.Controls.Add(this.IssueBtn);
            this.panel1.Controls.Add(this.IssueingPlaceCmb);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(0, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 445);
            this.panel1.TabIndex = 1;
            // 
            // IssueBtn
            // 
            this.IssueBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IssueBtn.Location = new System.Drawing.Point(15, 157);
            this.IssueBtn.Name = "IssueBtn";
            this.IssueBtn.Size = new System.Drawing.Size(115, 29);
            this.IssueBtn.TabIndex = 4;
            this.IssueBtn.Text = "Issue All";
            this.IssueBtn.UseVisualStyleBackColor = true;
            this.IssueBtn.Click += new System.EventHandler(this.IssueBtn_Click);
            // 
            // IssueingPlaceCmb
            // 
            this.IssueingPlaceCmb.FormattingEnabled = true;
            this.IssueingPlaceCmb.Location = new System.Drawing.Point(98, 96);
            this.IssueingPlaceCmb.Name = "IssueingPlaceCmb";
            this.IssueingPlaceCmb.Size = new System.Drawing.Size(186, 24);
            this.IssueingPlaceCmb.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Place     :";
            // 
            // issueAllForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "issueAllForm";
            this.Text = "Issue All Form";
            this.Load += new System.EventHandler(this.issueAllForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button IssueBtn;
        private System.Windows.Forms.ComboBox IssueingPlaceCmb;
        private System.Windows.Forms.Label label2;
    }
}