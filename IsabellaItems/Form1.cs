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
    }
}
