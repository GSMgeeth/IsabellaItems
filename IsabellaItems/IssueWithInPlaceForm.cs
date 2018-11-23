using IsabellaItems.Core;
using IsabellaItems.Role;
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
    public partial class IssueWithInPlaceForm : Form
    {
        private string color;
        private string size;
        private string article;
        private int balance;
        private int qty;

        public IssueWithInPlaceForm()
        {
            InitializeComponent();
        }

        public IssueWithInPlaceForm(string color, string size, string article, int balance)
        {
            this.color = color;
            this.size = size;
            this.article = article;
            this.balance = balance;
            this.qty = 0;

            InitializeComponent();
        }

        private void IssueWithInPlaceForm_Load(object sender, EventArgs e)
        {
            articleTxtBox.Text = article;
            colorTxtBox.Text = color;
            sizeTxtBox.Text = size;
            totBalTxtBox.Text = "" + balance;
            totQtyTxtBox.Text = "" + qty;

            fillPlaceCmb();
            fillDataGrid();
        }

        private void fillDataGrid()
        {
            DataTable table = new DataTable();

            try
            {
                Batch batch = Database.getBatch(color, size, article);

                MySqlDataReader reader = DBConnection.getData("SELECT p.in_place_name as In_Place, SUM(r.receivedQty) as Received, IFNULL(i.issued, 0) as Issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as Balance " +
                                                              "FROM received r LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued " +
                                                              "WHERE batch_id=" + batch.Batch_id + " GROUP BY batch_id) i on i.batch_id=r.batch_id INNER JOIN in_place p ON p.in_place_id=r.in_place_id WHERE r.batch_id=" + batch.Batch_id + " " +
                                                              "GROUP BY r.batch_id;");

                if (reader.HasRows)
                {
                    table.Load(reader);
                }
                else
                {
                    reader.Close();
                }

                DataColumn col = new DataColumn("Issue Qty", typeof(Int32));
                
                table.Columns.Add(col);
                //table.Columns[table.Columns.Count - 1].ReadOnly = false;

                issueingItemsDataGrid.DataSource = table;
                issueingItemsDataGrid.Columns[issueingItemsDataGrid.Columns.Count - 1].ReadOnly = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show("" + exc);
            }
        }

        private void fillPlaceCmb()
        {
            MySqlDataReader reader = DBConnection.getData("select * from place");

            while (reader.Read())
            {
                IssueingPlaceCmb.Items.Add(reader.GetString("place"));
            }

            reader.Close();
        }
    }
}
