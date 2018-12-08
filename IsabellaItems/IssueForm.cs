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
    public partial class IssueForm : Form
    {
        private string color;
        private string size;
        private string article;
        private int balance;

        public IssueForm(string color, string size, string article, int balance)
        {
            this.color = color;
            this.size = size;
            this.article = article;
            this.balance = balance;

            InitializeComponent();
        }

        private void IssueForm_Load(object sender, EventArgs e)
        {
            fillPlaceCmb();
        }

        private void fillPlaceCmb()
        {
            availableQtyLbl.Text = "Balance : " + balance;

            MySqlDataReader reader = DBConnection.getData("select * from place");

            while (reader.Read())
            {
                IssueingPlaceCmb.Items.Add(reader.GetString("place"));
            }

            reader.Close();
        }

        private void IssueBtn_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    int issuingQty = Int32.Parse(issueingQtyTxt.Text);
            //    Object placeObj = IssueingPlaceCmb.SelectedItem;

            //    if (issuingQty > balance)
            //    {
            //        MessageBox.Show("Not enough Balance!", "Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    else
            //    {
            //        if (placeObj == null)
            //        {
            //            MessageBox.Show("Please select a destination to issue!", "Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            string place = placeObj.ToString();

            //            Database.issue(place, color, size, article, issuingQty);

            //            this.Close();
            //        }
            //    }
            //}
            //catch (Exception exc)
            //{
            //    MessageBox.Show("Please check quantity value!\n" + exc, "Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
    }
}
