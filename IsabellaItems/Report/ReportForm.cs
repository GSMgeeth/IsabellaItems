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
            MySqlDataReader readerBal = null;
            
            table.Columns.Add("Color", typeof(string));
            table.Columns.Add("Size", typeof(string));
            table.Columns.Add("Article", typeof(string));
            table.Columns.Add("Total Quantity", typeof(int));
            table.Columns.Add("Balance Quantity", typeof(int));

            try
            {
                reader = DBConnection.getData(qry);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        readerBal = DBConnection.getDataViaTmpConnection("SELECT COUNT(item_id) as balance from item where issued=0 and color='" + reader.GetString("color") + "' and size='" + reader.GetString("size") + "' and article='" + reader.GetString("article") + "'");

                        if (readerBal.HasRows)
                        {
                            while (readerBal.Read())
                            {
                                table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), reader.GetInt32("total"), readerBal.GetInt32("balance"));
                            }

                            readerBal.Close();
                            DBConnection.closeTmpConnection();
                        }
                    }

                    reader.Close();

                    if (readerBal != null)
                        if (!readerBal.IsClosed)
                            readerBal.Close();

                    Report.CrystalReportOfItems rpt = new Report.CrystalReportOfItems();

                    rpt.Database.Tables["Item"].SetDataSource(table);

                    ItemsCrystalReportViewer.ReportSource = null;
                    ItemsCrystalReportViewer.ReportSource = rpt;
                }
                else
                {
                    reader.Close();

                    if (readerBal != null)
                        if (!readerBal.IsClosed)
                            readerBal.Close();

                    MessageBox.Show("No records!", "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (readerBal != null)
                    if (!readerBal.IsClosed)
                        readerBal.Close();

                MessageBox.Show("No records\n" + ex.Message + "\n" + ex.StackTrace, "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ItemsCrystalReportViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
