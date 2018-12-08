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

            foreach (DataGridViewColumn col in issueingItemsDataGrid.Columns)
            {
                col.ReadOnly = true;
            }

            issueingItemsDataGrid.Columns["Issue Qty"].ReadOnly = false;
        }

        private void fillDataGrid()
        {
            DataTable table = new DataTable();

            try
            {
                Batch batch = Database.getBatch(color, size, article);

                MySqlDataReader reader = DBConnection.getData("SELECT p.in_place_name as In_Place, SUM(r.receivedQty) as Received, IFNULL(i.issued, 0) as Issued, IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as Balance " +
                                                              "FROM received r LEFT JOIN (SELECT batch_id, in_place_id, SUM(issuedQty) as issued FROM issued " +
                                                              "WHERE batch_id=" + batch.Batch_id + " GROUP BY batch_id, in_place_id) i on i.batch_id=r.batch_id and i.in_place_id=r.in_place_id INNER JOIN in_place p ON p.in_place_id=r.in_place_id WHERE r.batch_id=" + batch.Batch_id + " " +
                                                              "GROUP BY r.batch_id, p.in_place_name;");

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

                issueingItemsDataGrid.DataSource = table;
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

        private void issueingItemsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == (issueingItemsDataGrid.Columns.Count - 1))
            {
                if (e.RowIndex >= 0)
                {
                    int tmpQty = 0;

                    foreach (DataGridViewRow dgvr in issueingItemsDataGrid.Rows)
                    {
                        int q = 0;

                        if (Int32.TryParse(dgvr.Cells[e.ColumnIndex].Value.ToString(), out q))
                        {
                            if (q > Int32.Parse(dgvr.Cells[3].Value.ToString()))
                            {
                                MessageBox.Show("Quantity must be less than or equal to the balance!", "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                dgvr.Cells[e.ColumnIndex].Value = 0;
                            }
                            else
                            {
                                tmpQty += q;
                            }
                        }
                    }

                    if (tmpQty <= balance)
                    {
                        qty = tmpQty;

                        totQtyTxtBox.Text = "" + qty;
                    }
                    else
                    {
                        MessageBox.Show("Quantity must be less than or equal to the balance!", "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private void issueItemsBtn_Click(object sender, EventArgs e)
        {
            Object tmp = IssueingPlaceCmb.SelectedItem;

            if (tmp != null)
            {
                string issuePlace = tmp.ToString();

                if (Int32.Parse(totQtyTxtBox.Text) <= Int32.Parse(totBalTxtBox.Text))
                {
                    foreach (DataGridViewRow dgvr in issueingItemsDataGrid.Rows)
                    {
                        int q = 0;

                        if (Int32.TryParse(dgvr.Cells[4].Value.ToString(), out q))
                        {
                            Database.issue(dgvr.Cells[0].Value.ToString(), issuePlace, color, size, article, q);
                        }
                    }

                    MessageBox.Show("Items successfully issued!", "Issue Items", MessageBoxButtons.OK);

                    closeForm();
                }
                else
                {
                    MessageBox.Show("Quantity must be less than or equal to the balance!", "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                MessageBox.Show("Set a place to issue!", "Issue Items", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void closeForm()
        {
            this.Close();
        }
    }
}
