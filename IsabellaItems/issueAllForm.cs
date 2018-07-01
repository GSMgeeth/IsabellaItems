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
    public partial class issueAllForm : Form
    {
        private Role.Issued[] issues;

        public issueAllForm()
        {
            InitializeComponent();
        }

        public void getIssues(Role.Issued[] issues)
        {
            this.issues = issues;
        }

        private void issueAllForm_Load(object sender, EventArgs e)
        {
            fillPlaceCmb();
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

        private void IssueBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Object placeObj = IssueingPlaceCmb.SelectedItem;

                if (placeObj == null)
                {
                    MessageBox.Show("Please select a destination to issue!", "Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string place = placeObj.ToString();

                    foreach (Role.Issued issue in issues)
                    {
                        issue.Place.SetPlace(place);
                    }

                    Database.issueAll(issues);

                    this.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something not right!", "Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
