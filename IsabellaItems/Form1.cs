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
            dataGridViewDept.DataSource = getDept();
            dataGridViewIssuedPlace.DataSource = getIssuedPlace();
            fillDeptCmb();
        }

        private object getIssuedPlace()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            MySqlDataReader reader = DBConnection.getData("select * from issuedTo");

            table.Load(reader);

            return table;
        }

        private System.Data.DataTable getDept()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            MySqlDataReader reader = DBConnection.getData("select * from department");

            table.Load(reader);

            return table;
        }

        private System.Data.DataTable getItems()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            MySqlDataReader reader = DBConnection.getData("select i.item_id, i.color, i.size, i.article, d.deptName, i.date, " +
                                                          "i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id inner join" +
                                                          " department d on i.deptNo=d.deptNo");

            table.Load(reader);

            return table;
        }

        private void fillDeptCmb()
        {
            MySqlDataReader reader = DBConnection.getData("select * from department");

            while (reader.Read())
            {
                deptCmb.Items.Add(reader.GetString("deptName"));
            }

            reader.Close();
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
            string deptName = "Die";
            DateTime date = datePicker.Value;
            string qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.date='" + date.ToString("yyyy/M/d") + "'";

            Object tmpDeptNameObj = deptCmb.SelectedItem;
            string color = searchColortxt.Text;
            string size = searchSizeTxt.Text;
            string article = searchArticleTxt.Text;

            if ((tmpDeptNameObj == null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj == null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.color='" + color + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj == null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.size='" + size + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj == null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.article='" + article + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj == null) && (!color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.size='" + size + "' and i.color='" + color + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj == null) && (!color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.article='" + article + "' and i.color='" + color + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj == null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on i.deptNo=d.deptNo " +
                                                          "where i.size='" + size + "' and i.article='" + article + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (!color.Equals("")) && (size.Equals("")) && (article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where i.color='" + color + "' and d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where i.size='" + size + "' and d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where i.article='" + article + "' and d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (!color.Equals("")) && (!size.Equals("")) && (article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where i.color='" + color + "' and i.size='" + size + "' and " +
                                                          "d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (!color.Equals("")) && (size.Equals("")) && (!article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where i.color='" + color + "' and i.article='" + article + "' and " +
                                                          "d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
            }
            else if ((tmpDeptNameObj != null) && (color.Equals("")) && (!size.Equals("")) && (!article.Equals("")))
            {
                deptName = deptCmb.SelectedItem.ToString();

                qry = "select i.item_id, i.color, i.size, i.article, d.deptName, i.date, i.issued, p.place " +
                                                          "from item i left join issuedTo p on p.place_id=i.place_id " +
                                                          "inner join department d on d.deptNo=i.deptNo " +
                                                          "where i.article='" + article + "' and i.size='" + size + "' and " +
                                                          "d.deptName='" + deptName + "' and " +
                                                          "i.date='" + date.ToString("yyyy/M/d") + "'";
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
            catch (Exception)
            {
                MessageBox.Show("Invalid data!", "Items finder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showAllBtn_Click(object sender, EventArgs e)
        {
            deptCmb.SelectedIndex = -1;
            searchColortxt.Clear();
            searchSizeTxt.Clear();
            searchArticleTxt.Clear();
            itemDataGridView.DataSource = getItems();
        }
    }
}
