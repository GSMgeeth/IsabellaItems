using IsabellaItems.Core;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
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
using IsabellaItems.Role;
using System.Runtime.InteropServices;
using IsabellaItems.Report;

namespace IsabellaItems
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            itemDataGridView.DataSource = getItems();
            dataGridViewIssuedPlace.DataSource = getIssuedPlace();
            fillIssuedCmb();
        }

        private void fillIssuedCmb()
        {
            MySqlDataReader reader = DBConnection.getData("select * from place");

            issuedCmb.Items.Add("All");

            while (reader.Read())
            {
                issuedCmb.Items.Add(reader.GetString("place"));
            }

            reader.Close();
        }

        private System.Data.DataTable getIssuedPlace()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            MySqlDataReader reader = DBConnection.getData("select * from place");

            table.Load(reader);

            return table;
        }
        
        private System.Data.DataTable getItems()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            MySqlDataReader reader = DBConnection.getData("SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                "FROM received r " +
                "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                "INNER JOIN batch b on r.batch_id=b.batch_id " +
                "GROUP BY r.batch_id;");

            table.Load(reader);

            return table;
        }
        
        private void addFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel Workbook|*.xlsx";
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {/*
            try
            {
                string name = openFileDialog1.SafeFileName;

                if (name.Contains(".xlsx"))
                {
                    _Application excel = new _Excel.Application();
                    Workbook wb;
                    Worksheet ws;

                    string path = "D:/SQItems/" + name;

                    wb = excel.Workbooks.Open(path);
                    ws = wb.Worksheets[1];

                    string deptTmp = ws.Cells[2, 1].Value2;

                    int deptNo = 1;

                    MySqlDataReader readerDept = DBConnection.getData("select * from department where deptName='" + deptTmp + "'");

                    if (readerDept.HasRows)
                    {
                        while (readerDept.Read())
                            deptNo = readerDept.GetInt32("deptNo");

                        readerDept.Close();

                        string date = ws.Cells[2, 2].Value2.ToString();
                        double qty = ws.Cells[2, 3].Value2;
                        double dayBagNo = ws.Cells[2, 4].Value2;

                        string day = date.Substring(1, date.IndexOf('/') - 1);
                        string tmpMonth = date.Substring(date.IndexOf('/') + 1);
                        string month = tmpMonth.Substring(0, tmpMonth.IndexOf('/'));
                        string year = tmpMonth.Substring((tmpMonth.IndexOf('/') + 1), 4);

                        DateTime d = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                        int q = (int)qty;
                        int bNo = (int)dayBagNo;
                        Department dept = new Department((int)deptNo);

                        Bag bag = new Bag(d, q, dept, bNo);

                        if (Database.isBagExists(bag))
                        {
                            MessageBox.Show("Bag already exists!", "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            DataTextBox.Text = "Bag deptNo : " + deptNo + "\nBag sent date : " + date + "\nQuantity : " + qty + "\nBagNo : " + dayBagNo + "\n";
                            DataTextBox.AppendText("\nyear : " + year + "  " + d.Year);
                            DataTextBox.AppendText("\nmonth : " + month + "  " + d.Month);
                            DataTextBox.AppendText("\nday : " + day + "  " + d.Day + "\n\n");

                            for (int i = 0; i < (int)qty; i++)
                            {
                                string color = ws.Cells[(i + 5), 1].Value2;
                                string size = ws.Cells[(i + 5), 2].Value2;
                                string article = ws.Cells[(i + 5), 3].Value2;

                                string tmp = "\nItem " + (i + 1) + " : " + color + " " + size + " " + article;

                                DataTextBox.AppendText(tmp);

                                bag.addItem(i, color, size, article);
                            }

                            try
                            {
                                MySqlDataReader reader = DBConnection.getData("select * from department");

                                while (reader.Read())
                                {
                                    int dNo = reader.GetInt32("deptNo");
                                    string deptName = reader.GetString("deptName");

                                    string tmp2 = "\nDept : " + dNo + " " + deptName;

                                    DataTextBox.AppendText(tmp2);
                                }

                                reader.Close();

                                Database.saveBag(bag);

                                itemDataGridView.DataSource = getItems();
                            }
                            catch (Exception exc)
                            {
                                DataTextBox.AppendText("\n" + exc.Message);
                                DataTextBox.AppendText("\n\n" + exc.StackTrace);
                            }
                            finally
                            {
                                wb.Close();
                                excel.Quit();

                                Marshal.ReleaseComObject(wb);
                                Marshal.ReleaseComObject(excel);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wrong Department name in the Excel file!", "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something wrong with the excel file!", "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string place = "Pallekale";
            //DateTime date = datePicker.Value;
            string qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                "FROM received r " +
                "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                "INNER JOIN batch b on r.batch_id=b.batch_id " +
                "GROUP BY r.batch_id";

            Object tmpPlaceObj = issuedCmb.SelectedItem;
            string color = searchColortxt.Text;
            string size = searchSizeTxt.Text;
            string article = searchArticleTxt.Text;

            if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                "FROM received r " +
                "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                "INNER JOIN batch b on r.batch_id=b.batch_id " +
                "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj != null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj == null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (!color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.color='" + color + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (!color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' and b.color='" + color + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.article='" + article + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj != null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj != null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj != null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article='" + article + "' " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj != null) && (!color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj != null) && (!color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.article='" + article + "' " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.article='" + article + "' " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj != null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' and b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
                else
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article='" + article + "' and b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
            }

            try
            {
                MySqlDataReader reader = DBConnection.getData(qry);

                if (reader.HasRows)
                {
                    System.Data.DataTable table = new System.Data.DataTable();

                    table.Load(reader);

                    itemDataGridView.DataSource = table;
                }
                else
                {
                    reader.Close();
                    MessageBox.Show("No records for this data!", "Items finder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Invalid data!\n" + exc.StackTrace, "Items finder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showAllBtn_Click(object sender, EventArgs e)
        {
            issuedCmb.SelectedIndex = -1;
            searchColortxt.Clear();
            searchSizeTxt.Clear();
            searchArticleTxt.Clear();
            itemDataGridView.DataSource = getItems();
        }

        private void getMonthlyReportBtn_Click(object sender, EventArgs e)
        {
            string qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as total, (r.receivedQty - i.issued) as balance " +
                "FROM received r " +
                "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                "INNER JOIN batch b on r.batch_id=b.batch_id " +
                "GROUP BY r.batch_id";

            ReportForm rptFrm = new ReportForm(qry);

            rptFrm.Show();
        }

        private void itemDataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string color = itemDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
            string size = itemDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
            string article = itemDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
            string balance = itemDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();

            IssueForm frm = new IssueForm(color, size, article, Int32.Parse(balance));

            frm.ShowDialog(this);

            itemDataGridView.DataSource = getItems();
        }
    }
}
