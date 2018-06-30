namespace IsabellaItems.Report
{
    partial class ReportForm
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
            this.ItemsCrystalReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // ItemsCrystalReportViewer
            // 
            this.ItemsCrystalReportViewer.ActiveViewIndex = -1;
            this.ItemsCrystalReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ItemsCrystalReportViewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.ItemsCrystalReportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemsCrystalReportViewer.Location = new System.Drawing.Point(0, 0);
            this.ItemsCrystalReportViewer.Name = "ItemsCrystalReportViewer";
            this.ItemsCrystalReportViewer.Size = new System.Drawing.Size(1196, 654);
            this.ItemsCrystalReportViewer.TabIndex = 0;
            this.ItemsCrystalReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.ItemsCrystalReportViewer.Load += new System.EventHandler(this.ItemsCrystalReportViewer_Load);
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1196, 654);
            this.Controls.Add(this.ItemsCrystalReportViewer);
            this.Name = "ReportForm";
            this.Text = "ReportForm";
            this.Load += new System.EventHandler(this.ReportForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer ItemsCrystalReportViewer;
    }
}