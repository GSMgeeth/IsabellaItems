using IsabellaItems.Role;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsabellaItems.Core
{
    class Database
    {
        public static void issue(string place, string color, string size, string article, int qty)
        {
            int batch_id = 1;
            int place_id = 1;

            MySqlDataReader reader = DBConnection.getData("select batch_id from batch where color='" + color + "' and size='" + size + "' and article='" + article + "'");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    batch_id = reader.GetInt32("batch_id");
                }

                reader.Close();

                MySqlDataReader readerPlace = DBConnection.getData("select place_id from place where place='" + place + "'");

                while (readerPlace.Read())
                {
                    place_id = readerPlace.GetInt32("place_id");
                }

                readerPlace.Close();

                DBConnection.updateDB("insert into issued (place_id, batch_id, date, issuedQty) values (" + place_id + ", " + batch_id + ", '" + DateTime.Now.ToString("yyyy/M/d") + "', " + qty + ")");
            }
        }

        public static void issueAll(Role.Issued [] issued)
        {
            foreach (Role.Issued issue in issued)
            {
                int batch_id = 1;
                int place_id = 1;

                string color = issue.Batch.Color;
                string size = issue.Batch.Size;
                string article = issue.Batch.Article;
                string place = issue.Place.GetPlace();
                int qty = issue.IssuedQty;

                MySqlDataReader reader = DBConnection.getData("select batch_id from batch where color='" + color + "' and size='" + size + "' and article='" + article + "'");

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        batch_id = reader.GetInt32("batch_id");
                    }

                    reader.Close();

                    MySqlDataReader readerPlace = DBConnection.getData("select place_id from place where place='" + place + "'");

                    while (readerPlace.Read())
                    {
                        place_id = readerPlace.GetInt32("place_id");
                    }

                    readerPlace.Close();

                    DBConnection.updateDB("insert into issued (place_id, batch_id, date, issuedQty) values (" + place_id + ", " + batch_id + ", '" + DateTime.Now.ToString("yyyy/M/d") + "', " + qty + ")");
                }
            }
        }

        public static void receive(Received rcv)
        {
            try
            {
                MySqlDataReader reader = DBConnection.getData("select * from received where batch_id=" + rcv.Batch.Batch_id + " and date='" + rcv.Date.ToString("yyyy/M/d") + "'");

                if (reader.HasRows)
                {
                    int currentQty = 0;

                    while (reader.Read())
                    {
                        currentQty = reader.GetInt32("receivedQty");
                    }
                    
                    reader.Close();

                    int totalQty = currentQty + rcv.ReceivedQty;

                    DBConnection.updateDB("update received set receivedQty=" + totalQty + " where batch_id=" + rcv.Batch.Batch_id + " and date='" + rcv.Date.ToString("yyyy/M/d") + "'");
                }
                else
                {
                    reader.Close();

                    DBConnection.updateDB("insert into received (batch_id, date, receivedQty) values (" + rcv.Batch.Batch_id + ", '" + rcv.Date.ToString("yyyy/M/d") + "', " + rcv.ReceivedQty + ")");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Something went wrong!\n" + exc, "DB Uploader", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Batch getBatch(string color, string size, string article)
        {
            int batch_id = 1;

            MySqlDataReader reader = DBConnection.getData("select batch_id from batch where color='" + color + "' and size='" + size + "' and article='" + article + "'");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    batch_id = reader.GetInt32("batch_id");
                }

                reader.Close();

                return new Batch(batch_id, color, size, article);
            }
            else
            {
                reader.Close();

                DBConnection.updateDB("insert into batch (color, size, article) values ('" + color + "', '" + size + "', '" + article + "')");

                MySqlDataReader readerNew = DBConnection.getData("select batch_id from batch where color='" + color + "' and size='" + size + "' and article='" + article + "'");

                while (readerNew.Read())
                {
                    batch_id = readerNew.GetInt32("batch_id");
                }

                readerNew.Close();

                return new Batch(batch_id, color, size, article);
            }
        }

        public static void savePlace(Place place)
        {
            string placeName = place.GetPlace();

            DBConnection.updateDB("insert into place (place) values ('" + placeName + "')");
        }
    }
}
