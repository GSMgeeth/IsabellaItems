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
            setProgress();
        }

        private void setProgress()
        {
            int received = 0;
            int issued = 0;

            try
            {
                MySqlDataReader readerRcv = DBConnection.getData("select SUM(receivedQty) AS received from received");

                if (readerRcv.HasRows)
                {
                    while (readerRcv.Read())
                    {
                        received = readerRcv.GetInt32("received");
                    }

                    readerRcv.Close();

                    MySqlDataReader readerIss = DBConnection.getData("select SUM(issuedQty) AS issued from issued");

                    while (readerIss.Read())
                    {
                        issued = readerIss.GetInt32("issued");
                    }

                    readerIss.Close();
                }

                int balance = received - issued;

                rcvLbl.Text = "" + received;
                issLbl.Text = "" + issued;
                balLbl.Text = "" + balance;

                string day = DateTime.Now.DayOfWeek.ToString();

                int noOfDay = getDayeNo(day);
                int i = noOfDay;

                DateTime date = DateTime.Today;

                do
                {
                    int chartRcv = 0;
                    int chartIss = 0;

                    MySqlDataReader reader = DBConnection.getData("select IFNULL(SUM(receivedQty),0) AS received from received where date<=date('" + date.AddDays(-i).ToString("yyyy/M/d") + "')");

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            chartRcv = reader.GetInt32("received");
                        }

                        reader.Close();

                        MySqlDataReader readerTwo = DBConnection.getData("select IFNULL(SUM(issuedQty),0) AS issued from issued where date<=date('" + date.AddDays(-i).ToString("yyyy/M/d") + "')");

                        while (readerTwo.Read())
                        {
                            chartIss = readerTwo.GetInt32("issued");
                        }

                        readerTwo.Close();

                        chart.Series["Received"].Points.AddXY(date.AddDays(-i).DayOfWeek.ToString(), chartRcv);
                        chart.Series["Balance"].Points.AddXY(date.AddDays(-i).DayOfWeek.ToString(), chartRcv - chartIss);

                        i--;
                    }

                } while (i >= 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int getDayeNo(string day)
        {
            if (day.Equals("Monday"))
                return 0;
            else if (day.Equals("Tuesday"))
                return 1;
            else if (day.Equals("Wednesday"))
                return 2;
            else if (day.Equals("Thursday"))
                return 3;
            else if (day.Equals("Friday"))
                return 4;
            else if (day.Equals("Saturday"))
                return 5;
            else if (day.Equals("Sunday"))
                return 6;
            else
                return -1;
        }

        private void fillIssuedCmb()
        {
            try
            {
                MySqlDataReader reader = DBConnection.getData("select * from place");

                issuedCmb.Items.Add("All");
                placeForMonthlyReport.Items.Add("All");

                while (reader.Read())
                {
                    issuedCmb.Items.Add(reader.GetString("place"));
                    placeForMonthlyReport.Items.Add(reader.GetString("place"));
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private System.Data.DataTable getIssuedPlace()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            try
            {
                MySqlDataReader reader = DBConnection.getData("select * from place");

                if (reader.HasRows)
                {
                    table.Load(reader);
                }
                else
                {
                    table.Load(null);
                }
            }
            catch (Exception)
            {
                table.Load(null);

                throw;
            }

            return table;
        }
        
        private System.Data.DataTable getItems()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            try
            {
                MySqlDataReader reader = DBConnection.getData("SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                "FROM received r " +
                "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                "INNER JOIN batch b on r.batch_id=b.batch_id " +
                "GROUP BY r.batch_id;");

                if (reader.HasRows)
                {
                    table.Load(reader);
                }
                else
                {
                    table.Load(null);
                }
            }
            catch (Exception)
            {
                table.Load(null);

                throw;
            }
            
            return table;
        }
        
        private void addFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel Workbook|*.xlsx|Excel Workbook 2003|*.xls";
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string name = openFileDialog1.SafeFileName;

                if (name.Contains(".xlsx") || name.Contains(".xls"))
                {
                    _Application excel = new _Excel.Application();
                    Workbook wb;
                    Worksheet ws;

                    string path = "D:/PackingSocks/" + name;

                    wb = excel.Workbooks.Open(path);
                    ws = wb.Worksheets[1];

                    int x = 2;

                    while (ws.Cells[x, 1].Value2 != null)
                    {
                        string article = ws.Cells[x, 1].Value2;
                        //double tmpColor = ws.Cells[x, 2].Value2;
                        string color = ws.Cells[x, 2].Value2;
                        string size = ws.Cells[x, 3].Value2;

                        //DataTextBox.Text = "article : " + article + "\nsize : " + size + "\nColor : " + color + "\n";

                        Batch batch = Database.getBatch(color, size, article);

                        double qty = 0;

                        try
                        {
                            qty = ws.Cells[x, 5].Value2;

                            //DataTextBox.AppendText("Qty : " + qty + "\n");

                            Received rcv = new Received(batch, DateTime.Now, (int)qty);

                            Database.receive(rcv);

                            x++;
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show("Something wrong with the qty cell in excel file!\n" + exc, "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            itemDataGridView.DataSource = getItems();
                            setProgress();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something wrong with the excel file!", "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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

                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id " +
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
            DateTime from = dateTimePickerFrom.Value;
            DateTime to = dateTimePickerTo.Value;
            int type = 0;
            string qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as total, (r.receivedQty - i.issued) as balance " +
                "FROM received r " +
                "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued WHERE date BETWEEN date('" + from.ToString("yyyy/M/d") + "') and date('" + to.ToString("yyyy/M/d") + "') GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                "INNER JOIN batch b on r.batch_id=b.batch_id WHERE r.date BETWEEN date('" + from.ToString("yyyy/M/d") + "') and date('" + to.ToString("yyyy/M/d") + "') " +
                "GROUP BY r.batch_id";

            object tmpPlace = placeForMonthlyReport.SelectedItem;

            if (tmpPlace != null)
            {
                string place = tmpPlace.ToString();

                if (!place.Equals("All"))
                {
                    int place_id = 1;
                    MySqlDataReader reader = DBConnection.getData("select * from place where place='" + place + "'");

                    while (reader.Read())
                    {
                        place_id = reader.GetInt32("place_id");
                    }

                    reader.Close();

                    type = 1;

                    qry = "SELECT b.color, b.size, b.article, IFNULL(i.issued, 0) as issued " +
                        "FROM received r " +
                        "LEFT JOIN (SELECT place_id, batch_id, SUM(issuedQty) as issued FROM issued WHERE place_id=" + place_id + " and date BETWEEN date('" + from.ToString("yyyy/M/d") + "') and date('" + to.ToString("yyyy/M/d") + "') GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                        "INNER JOIN batch b on r.batch_id=b.batch_id WHERE i.place_id=" + place_id + " and  r.date BETWEEN date('" + from.ToString("yyyy/M/d") + "') and date('" + to.ToString("yyyy/M/d") + "') " +
                        "GROUP BY r.batch_id";
                }
            }

            ReportForm rptFrm = new ReportForm(qry, type);

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
            setProgress();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string qry = "select SUM(r.receivedQty) as received, IFNULL(ip.issuedQty, 0) as pallekale, IFNULL(ih.issuedQty, 0) as henz, IFNULL((SUM(r.receivedQty) - (IFNULL(ip.issuedQty, 0) + IFNULL(ih.issuedQty, 0))), 0) as balance " +
                "from received r " +
                "join (select SUM(issuedQty) as issuedQty from issued where place_id=1) ip " +
                "join (select SUM(issuedQty) as issuedQty from issued where place_id=2) ih;";

            ReportForm rptFrm = new ReportForm(qry, 2);

            rptFrm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int length = itemDataGridView.RowCount;

            Role.Issued[] issue = new Role.Issued[length];

            for (int i = 0; i < length; i++)
            {
                string color = itemDataGridView.Rows[i].Cells[0].Value.ToString();
                string size = itemDataGridView.Rows[i].Cells[1].Value.ToString();
                string article = itemDataGridView.Rows[i].Cells[2].Value.ToString();
                string balance = itemDataGridView.Rows[i].Cells[5].Value.ToString();

                Batch batch = new Batch(color, size, article);
                Place place = new Place();

                issue[i] = new Role.Issued(batch, place, DateTime.Now, int.Parse(balance));
            }

            issueAllForm frm = new issueAllForm();

            frm.getIssues(issue);

            frm.ShowDialog(this);

            itemDataGridView.DataSource = getItems();
            setProgress();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DBConnection.backupDB();
        }
    }
}
