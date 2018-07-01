using IsabellaItems.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsabellaItems.Report
{
    public partial class ReportForm : Form
    {
        string qry = "";

        public ReportForm(string qry)
        {
            this.qry = qry;

            InitializeComponent();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            DataTable table = new DataTable();

            MySqlDataReader reader = null;
            
            table.Columns.Add("Color", typeof(string));
            table.Columns.Add("Size", typeof(string));
            table.Columns.Add("Article", typeof(string));
            table.Columns.Add("Total Quantity", typeof(int));
            table.Columns.Add("Issued Quantity", typeof(int));
            table.Columns.Add("Balance Quantity", typeof(int));

            try
            {
                reader = DBConnection.getData(qry);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Object o;

                        try
                        {
                            o = reader.GetString("balance");
                        }
                        catch (Exception)
                        {
                            o = null;
                        }

                        if (o != null)
                            table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), reader.GetInt32("total"), reader.GetInt32("total") - reader.GetInt32("balance"), reader.GetInt32("balance"));
                        else
                            table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), reader.GetInt32("total"), 0, reader.GetInt32("total"));
                    }

                    reader.Close();

                    Report.CrystalReportOfItems rpt = new Report.CrystalReportOfItems();

                    rpt.Database.Tables["Item"].SetDataSource(table);

                    ItemsCrystalReportViewer.ReportSource = null;
                    ItemsCrystalReportViewer.ReportSource = rpt;
                }
                else
                {
                    reader.Close();

                    MessageBox.Show("No records!", "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No records\n" + ex.Message + "\n" + ex.StackTrace, "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ItemsCrystalReportViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
