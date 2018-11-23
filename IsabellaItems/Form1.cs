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
            dataGridViewInPlace.DataSource = getInPlace();

            dataGridViewIssuedPlace.Columns[0].Visible = false;
            dataGridViewInPlace.Columns[0].Visible = false;

            fillIssuedCmb();
            fillInCmb();
            setProgress();
        }

        private void setProgress()
        {
            int received = 0;
            int issued = 0;

            MySqlDataReader readerRcv = null;
            MySqlDataReader readerIss = null;

            try
            {
                readerRcv = DBConnection.getData("select IFNULL(SUM(receivedQty), 0) AS received from received");

                if (readerRcv.HasRows)
                {
                    while (readerRcv.Read())
                    {
                        received = readerRcv.GetInt32("received");
                    }

                    readerRcv.Close();

                    readerIss = DBConnection.getData("select IFNULL(SUM(issuedQty), 0) AS issued from issued");

                    while (readerIss.Read())
                    {
                        issued = readerIss.GetInt32("issued");
                    }

                    readerIss.Close();
                }
                else
                {
                    readerRcv.Close();
                }

                int balance = received - issued;

                rcvLbl.Text = " ";
                issLbl.Text = " ";
                balLbl.Text = " ";

                rcvLbl.Text = "" + received;
                issLbl.Text = "" + issued;
                balLbl.Text = "" + balance;
            }
            catch (Exception)
            {
                if (readerRcv != null)
                    readerRcv.Close();

                if (readerIss != null)
                    readerIss.Close();
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
                
                if (reader.HasRows)
                {
                    issuedCmb.Items.Clear();
                    placeForMonthlyReport.Items.Clear();

                    issuedCmb.Items.Add("All");
                    placeForMonthlyReport.Items.Add("All");

                    while (reader.Read())
                    {
                        issuedCmb.Items.Add(reader.GetString("place"));
                        placeForMonthlyReport.Items.Add(reader.GetString("place"));
                    }
                }
                
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void fillInCmb()
        {
            try
            {
                MySqlDataReader reader = DBConnection.getData("select * from in_place");

                if (reader.HasRows)
                {
                    inCmb.Items.Clear();
                    //placeForMonthlyReport.Items.Clear();

                    inCmb.Items.Add("All");
                    //placeForMonthlyReport.Items.Add("All");

                    while (reader.Read())
                    {
                        inCmb.Items.Add(reader.GetString("in_place_name"));
                        //placeForMonthlyReport.Items.Add(reader.GetString("place"));
                    }
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
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return table;
        }

        private System.Data.DataTable getInPlace()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            try
            {
                MySqlDataReader reader = DBConnection.getData("select * from in_place");

                if (reader.HasRows)
                {
                    table.Load(reader);
                }
                else
                {
                    reader.Close();
                }
            }
            catch (Exception)
            {
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
                "FROM received r LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id GROUP BY r.batch_id;");

                if (reader.HasRows)
                {
                    table.Load(reader);
                }
                else
                {
                    reader.Close();
                }
            }
            catch (Exception)
            {
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
            int tracker = 1;
            
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

                    Place inPlace = new Place();
                    string article, size, color;
                    double qty = 0;

                    if (ws.Cells[2, 6].Value2 == null)
                    {
                        MessageBox.Show("First row of the excell sheet must have a in place!", "File Reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        while (ws.Cells[x, 1].Value2 != null)
                        {
                            tracker++;

                            if (ws.Cells[x, 1].Value2 is double)
                            {
                                article = "" + (int)ws.Cells[x, 1].Value2;
                            }
                            else
                            {
                                article = ws.Cells[x, 1].Value2;
                            }

                            if (ws.Cells[x, 2].Value2 is double)
                            {
                                color = "" + (int)ws.Cells[x, 2].Value2;
                            }
                            else
                            {
                                color = ws.Cells[x, 2].Value2;
                            }

                            if (ws.Cells[x, 3].Value2 is double)
                            {
                                size = "" + (int)ws.Cells[x, 3].Value2;
                            }
                            else
                            {
                                size = ws.Cells[x, 3].Value2;
                            }

                            Batch batch = Database.getBatch(color, size, article);

                            qty = ws.Cells[x, 5].Value2;

                            if (ws.Cells[x, 6].Value2 != null)
                            {
                                if (ws.Cells[x, 6].Value2 is double)
                                    inPlace.SetPlace("" + (int)ws.Cells[x, 6].Value2);
                                else
                                    inPlace.SetPlace(ws.Cells[x, 6].Value2);
                            }

                            try
                            {
                                Received rcv = new Received(DateTime.Now, (int)qty, batch, inPlace);

                                Database.receive(rcv);

                                setProgress();

                                x++;
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show("Something wrong with the qty cell in excel file line no " + tracker + "!\n" + exc, "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        itemDataGridView.DataSource = getItems();
                        setProgress();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something wrong with the excel file line no " + tracker + "!\n" + exception, "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string place = "Pallekale";
            string inPlace = "";
            string qry = "";

            Object tmpPlaceObj = issuedCmb.SelectedItem;
            Object tmpInPlaceObj = inCmb.SelectedItem;
            string color = searchColortxt.Text;
            string size = searchSizeTxt.Text;
            string article = searchArticleTxt.Text;

            if (tmpInPlaceObj == null)
            {
                button3.Visible = false;

                if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id GROUP BY r.batch_id";
                }
                else if ((tmpPlaceObj != null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                {
                    place = issuedCmb.SelectedItem.ToString();

                    if (place.Equals("All"))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id GROUP BY r.batch_id";
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

                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued " +
                        "FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id WHERE i.place_id=" + place_id + " GROUP BY r.batch_id";
                    }
                }
                else if ((tmpPlaceObj == null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' GROUP BY r.batch_id";
                }
                else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' GROUP BY r.batch_id";
                }
                else if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' GROUP BY r.batch_id";
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.color='" + color + "' " +
                        "GROUP BY r.batch_id";
                }
                else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                        "FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.article like '%" + article + "' " +
                        "GROUP BY r.batch_id";
                }
                else if ((tmpPlaceObj == null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                        "FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.color='" + color + "' and b.article like '%" + article + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.article like '%" + article + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.article like '%" + article + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.size='" + size + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' and b.size='" + size + "' " +
                        "GROUP BY r.batch_id";
                    }
                }
                else if ((tmpPlaceObj != null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                {
                    place = issuedCmb.SelectedItem.ToString();

                    if (place.Equals("All"))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                        "FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                        "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' " +
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
                        "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' " +
                        "GROUP BY r.batch_id";
                    }
                }
            }
            else
            {
                inPlace = inCmb.SelectedItem.ToString();

                if (inPlace.Equals("All"))
                {
                    if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj != null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id GROUP BY r.batch_id";
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

                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued " +
                            "FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id WHERE i.place_id=" + place_id + " GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj == null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' GROUP BY r.batch_id";
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.color='" + color + "' " +
                            "GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.article like '%" + article + "' " +
                            "GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.color='" + color + "' and b.article like '%" + article + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.article like '%" + article + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.article like '%" + article + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.size='" + size + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' and b.size='" + size + "' " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' " +
                            "GROUP BY r.batch_id";
                        }
                    }
                }
                else
                {
                    int in_place_id = 1;
                    MySqlDataReader readerIn = DBConnection.getData("select * from in_place where in_place_name='" + inPlace + "'");

                    while (readerIn.Read())
                    {
                        in_place_id = readerIn.GetInt32("in_place_id");
                    }

                    readerIn.Close();

                    if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r " +
                        "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id WHERE r.in_place_id=" + in_place_id + " GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj != null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id WHERE r.in_place_id=" + in_place_id + " GROUP BY r.batch_id";
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

                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r LEFT JOIN (SELECT batch_id, place_id, SUM(issuedQty) as issued " +
                            "FROM issued where place_id=" + place_id + " GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id WHERE i.place_id=" + place_id + " and r.in_place_id=" + in_place_id + " GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj == null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and r.in_place_id=" + in_place_id + " GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and r.in_place_id=" + in_place_id + " GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id INNER JOIN batch b on r.batch_id=b.batch_id where b.article='" + article + "' and r.in_place_id=" + in_place_id + " GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (!color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.color='" + color + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (!color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.color='" + color + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.article like '%" + article + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj == null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.color='" + color + "' and b.article like '%" + article + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                    }
                    else if ((tmpPlaceObj != null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and r.in_place_id=" + in_place_id + " r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.size='" + size + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (!color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.size='" + size + "' and r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.size='" + size + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (!color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.article like '%" + article + "' and r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.article like '%" + article + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article like '%" + article + "' and b.size='" + size + "' and r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article like '%" + article + "' and b.size='" + size + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
                    else if ((tmpPlaceObj != null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
                    {
                        place = issuedCmb.SelectedItem.ToString();

                        if (place.Equals("All"))
                        {
                            qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued " +
                            "FROM received r " +
                            "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                            "INNER JOIN batch b on r.batch_id=b.batch_id where b.article LIKE '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' and r.in_place_id=" + in_place_id + " " +
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
                            "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article LIKE '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' and r.in_place_id=" + in_place_id + " " +
                            "GROUP BY r.batch_id";
                        }
                    }
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
            inCmb.SelectedItem = null;
            searchColortxt.Clear();
            searchSizeTxt.Clear();
            searchArticleTxt.Clear();

            button3.Visible = true;

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
            Object tmpInPlaceObj = inCmb.SelectedItem;

            if (tmpInPlaceObj == null)
            {
                try
                {
                    string color = itemDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                    string size = itemDataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                    string article = itemDataGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                    string balance = itemDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString();

                    IssueWithInPlaceForm frm = new IssueWithInPlaceForm(color, size, article, Int32.Parse(balance));

                    frm.ShowDialog(this);

                    itemDataGridView.DataSource = getItems();
                    setProgress();
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("You can't issue with Issued place is selected!", "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("You can't issue with Issued place is selected!", "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string qry = "select SUM(r.receivedQty) as received, IFNULL(ip.issuedQty, 0) as pallekale, IFNULL(ih.issuedQty, 0) as henz, IFNULL((SUM(r.receivedQty) - (IFNULL(ip.issuedQty, 0) + IFNULL(ih.issuedQty, 0))), 0) as balance " +
                "from received r " +
                "join (select SUM(issuedQty) as issuedQty from issued where place_id=1) ip " +
                "join (select SUM(issuedQty) as issuedQty from issued where place_id=2) ih;";

            string qryTmp = "SELECT COUNT(i.item_id) as itemQty, t.place as place FROM issued b " +
                            "LEFT JOIN place t ON b.place_id=t.place_id " +
                            "INNER JOIN item i on b.bag_id=i.bag_id " +
                            "WHERE issued=1 " +
                            "GROUP BY b.place_id;";
            string qryRec = "select SUM(receivedQty) as received from received;";

            string qryIss = "select p.place, IFNULL(SUM(i.issuedQty), 0) as issued" +
                          " from place p left join issued i on p.place_id=i.place_id group by i.place_id";

            ReportForm rptFrm = new ReportForm(qryIss, qryRec, 3);

            rptFrm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string newIssuedPlace = newIssuedPlaceTxtBox.Text;

            if ((newIssuedPlace != null) && (!newIssuedPlace.Equals("")))
            {
                Place place = new Place(newIssuedPlace);

                MySqlDataReader reader = DBConnection.getData("select * from place where place='" + newIssuedPlace + "'");

                if (reader.HasRows)
                {
                    reader.Close();
                    MessageBox.Show("This place already exists!", "Add place", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    reader.Close();
                    Database.savePlace(place);
                    
                    dataGridViewIssuedPlace.DataSource = getIssuedPlace();
                    fillIssuedCmb();

                    newIssuedPlaceTxtBox.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Please enter the new place name!", "Add place", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

        private void searchArticleTxt_TextChanged(object sender, EventArgs e)
        {
            string place = "Pallekale";
            string qry = "";

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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article LIKE '%" + article + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article LIKE '%" + article + "' and b.color='" + color + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.article LIKE '%" + article + "' " +
                    "GROUP BY r.batch_id";
            }
            else if ((tmpPlaceObj == null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.size='" + size + "' and b.color='" + color + "' and b.article LIKE '%" + article + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article LIKE '%" + article + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article LIKE '%" + article + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.color='" + color + "' and b.article LIKE '%" + article + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.color='" + color + "' and b.article LIKE '%" + article + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article LIKE '%" + article + "' and b.size='" + size + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article LIKE '%" + article + "' and b.size='" + size + "' " +
                    "GROUP BY r.batch_id";
                }
            }
            else if ((tmpPlaceObj != null) && (!color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                place = issuedCmb.SelectedItem.ToString();

                if (place.Equals("All"))
                {
                    qry = "SELECT b.color, b.size, b.article, SUM(r.receivedQty) as received, IFNULL(i.issued, 0) as issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                    "FROM received r " +
                    "LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued GROUP BY batch_id) i on r.batch_id=i.batch_id " +
                    "INNER JOIN batch b on r.batch_id=b.batch_id where b.article LIKE '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' " +
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
                    "INNER JOIN batch b on r.batch_id=b.batch_id where i.place_id=" + place_id + " and b.article LIKE '%" + article + "' and b.size='" + size + "' and b.color='" + color + "' " +
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
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Invalid data!\n" + exc.StackTrace, "Items finder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string newInPlace = newInPlaceTxtBox.Text;

            if ((newInPlace != null) && (!newInPlace.Equals("")))
            {
                Place place = new Place(newInPlace);

                MySqlDataReader reader = DBConnection.getData("select * from in_place where in_place_name='" + newInPlace + "'");

                if (reader.HasRows)
                {
                    reader.Close();
                    MessageBox.Show("This place already exists!", "Add In place", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    reader.Close();
                    Database.saveInPlace(place);

                    dataGridViewInPlace.DataSource = getInPlace();
                    fillInCmb();

                    newInPlaceTxtBox.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Please enter the new place name!", "Add In place", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void issueItemsBtn_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Excel Workbook|*.xlsx|Excel Workbook 2003|*.xls";
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            int tracker = 1;

            try
            {
                string name = openFileDialog2.SafeFileName;

                if (name.Contains(".xlsx") || name.Contains(".xls"))
                {
                    _Application excel = new _Excel.Application();
                    Workbook wb;
                    Worksheet ws;

                    string path = "D:/PackingSocks/IssueFiles/" + name;

                    wb = excel.Workbooks.Open(path);
                    ws = wb.Worksheets[1];

                    int x = 2;

                    Place inPlace = new Place();
                    string article, size, color;
                    double qty = 0;

                    if (ws.Cells[2, 6].Value2 == null)
                    {
                        MessageBox.Show("First row of the excell sheet must have a in place!", "File Reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        while (ws.Cells[x, 1].Value2 != null)
                        {
                            tracker++;

                            if (ws.Cells[x, 1].Value2 is double)
                            {
                                article = "" + (int)ws.Cells[x, 1].Value2;
                            }
                            else
                            {
                                article = ws.Cells[x, 1].Value2;
                            }

                            if (ws.Cells[x, 2].Value2 is double)
                            {
                                color = "" + (int)ws.Cells[x, 2].Value2;
                            }
                            else
                            {
                                color = ws.Cells[x, 2].Value2;
                            }

                            if (ws.Cells[x, 3].Value2 is double)
                            {
                                size = "" + (int)ws.Cells[x, 3].Value2;
                            }
                            else
                            {
                                size = ws.Cells[x, 3].Value2;
                            }

                            qty = ws.Cells[x, 5].Value2;

                            if (ws.Cells[x, 6].Value2 != null)
                            {
                                if (ws.Cells[x, 6].Value2 is double)
                                    inPlace.SetPlace("" + (int)ws.Cells[x, 6].Value2);
                                else
                                    inPlace.SetPlace(ws.Cells[x, 6].Value2);
                            }

                            try
                            {
                                int stat = Database.issueFromExcel(inPlace.GetPlace(), color, size, article, (int)qty);

                                if (stat == 1)
                                {
                                    MessageBox.Show("The combination of article, color and size doesn't exists!\nExcel sheet line no " + tracker, "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                                else if (stat == 2)
                                {
                                    MessageBox.Show("The issueing place doesn't exists!\nExcel sheet line no " + tracker + " " + inPlace.GetPlace(), "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                                else if (stat == 2)
                                {
                                    MessageBox.Show("The issueing quantity is not available!\nExcel sheet line no " + tracker, "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }

                                setProgress();

                                x++;
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show("Something wrong with the qty cell in excel file line no " + tracker + "!\n" + exc, "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        itemDataGridView.DataSource = getItems();
                        setProgress();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something wrong with the excel file line no " + tracker + "!\n" + exception, "File reader", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
