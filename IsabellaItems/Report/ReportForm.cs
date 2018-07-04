using CrystalDecisions.Shared;
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
        int type;

        public ReportForm(string qry, int type)
        {
            this.qry = qry;
            this.type = type;

            InitializeComponent();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            if (type == 1)
            {
                DataTable table = new DataTable();

                MySqlDataReader reader = null;

                table.Columns.Add("Color", typeof(string));
                table.Columns.Add("Size", typeof(string));
                table.Columns.Add("Article", typeof(string));
                table.Columns.Add("Issued Quantity", typeof(int));

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
                                o = reader.GetString("issued");
                            }
                            catch (Exception)
                            {
                                o = null;
                            }

                            if (o != null)
                                table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), reader.GetInt32("issued"));
                            else
                                table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), 0);
                        }

                        reader.Close();

                        Report.CrystalReportIssued rpt = new Report.CrystalReportIssued();

                        rpt.Database.Tables["Item"].SetDataSource(table);
                        /*
                        ExportOptions exportOptions;
                        DiskFileDestinationOptions diskFileDestinationOptions = new DiskFileDestinationOptions();

                        SaveFileDialog sfd = new SaveFileDialog();

                        sfd.Filter = "Pdf Files|*.pdf";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            diskFileDestinationOptions.DiskFileName = sfd.FileName;
                        }

                        exportOptions = rpt.ExportOptions;
                        {
                            exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                            exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                            exportOptions.DestinationOptions = diskFileDestinationOptions;
                            exportOptions.ExportFormatOptions = new PdfRtfWordFormatOptions();
                        }

                        rpt.Export();
                        */
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
                    MessageBox.Show("" + ex, "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (type == 0)
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
                        /*
                        ExportOptions exportOptions;
                        DiskFileDestinationOptions diskFileDestinationOptions = new DiskFileDestinationOptions();

                        SaveFileDialog sfd = new SaveFileDialog();

                        sfd.Filter = "Pdf Files|*.pdf";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            diskFileDestinationOptions.DiskFileName = sfd.FileName;
                        }

                        exportOptions = rpt.ExportOptions;
                        {
                            exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                            exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                            exportOptions.DestinationOptions = diskFileDestinationOptions;
                            exportOptions.ExportFormatOptions = new PdfRtfWordFormatOptions();
                        }

                        rpt.Export();
                        */
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
                    MessageBox.Show("" + ex, "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (type == 2)
            {
                DataTable table = new DataTable();

                MySqlDataReader reader = null;

                table.Columns.Add("Received", typeof(int));
                table.Columns.Add("Pallekale", typeof(int));
                table.Columns.Add("Henz", typeof(int));
                table.Columns.Add("Balance", typeof(int));

                try
                {
                    reader = DBConnection.getData(qry);

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            /*
                            Object o;

                            try
                            {
                                o = reader.GetString("issued");
                            }
                            catch (Exception)
                            {
                                o = null;
                            }
                            
                            if (o != null)
                                table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), reader.GetInt32("issued"));
                            else
                                table.Rows.Add(reader.GetString("color"), reader.GetString("size"), reader.GetString("article"), 0);
                            */

                            table.Rows.Add(reader.GetInt32("received"), reader.GetInt32("pallekale"), reader.GetInt32("henz"), reader.GetInt32("balance"));
                        }

                        reader.Close();

                        Report.CrystalReportSummary rpt = new Report.CrystalReportSummary();

                        rpt.Database.Tables["Summary"].SetDataSource(table);
                        /*
                        ExportOptions exportOptions;
                        DiskFileDestinationOptions diskFileDestinationOptions = new DiskFileDestinationOptions();

                        SaveFileDialog sfd = new SaveFileDialog();

                        sfd.Filter = "Pdf Files|*.pdf";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            diskFileDestinationOptions.DiskFileName = sfd.FileName;
                        }

                        exportOptions = rpt.ExportOptions;
                        {
                            exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                            exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                            exportOptions.DestinationOptions = diskFileDestinationOptions;
                            exportOptions.ExportFormatOptions = new PdfRtfWordFormatOptions();
                        }

                        rpt.Export();
                        */
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
                    MessageBox.Show("" + ex, "Items picker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ItemsCrystalReportViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
