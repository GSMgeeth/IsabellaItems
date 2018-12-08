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
        public static void issue(string inPlace, string place, string color, string size, string article, int qty)
        {
            int batch_id = 1;
            int place_id = 1;
            int in_place_id = 1;

            MySqlDataReader reader = DBConnection.getData("select batch_id from batch where color='" + color + "' and size='" + size + "' and article='" + article + "'");

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    batch_id = reader.GetInt32(0);
                }

                reader.Close();

                MySqlDataReader readerPlace = DBConnection.getData("select place_id from place where place='" + place + "'");

                while (readerPlace.Read())
                {
                    place_id = readerPlace.GetInt32(0);
                }

                readerPlace.Close();

                readerPlace = DBConnection.getData("select in_place_id from in_place where in_place_name='" + inPlace + "'");

                while (readerPlace.Read())
                {
                    in_place_id = readerPlace.GetInt32(0);
                }

                readerPlace.Close();

                reader = DBConnection.getData("select * from issued where place_id=" + place_id + " and batch_id=" + batch_id + " and in_place_id=" + in_place_id + " and date='" + DateTime.Now.ToString("yyyy/M/d") + "'");

                if (reader.HasRows)
                {
                    int currentQty = 0;

                    while (reader.Read())
                    {
                        currentQty = reader.GetInt32("issuedQty");
                    }

                    int newQty = currentQty + qty;

                    reader.Close();

                    DBConnection.updateDB("update issued set issuedQty=" + newQty + " where place_id=" + place_id + " and in_place_id=" + in_place_id + " and batch_id=" + batch_id + " and date='" + DateTime.Now.ToString("yyyy/M/d") + "'");
                }
                else
                {
                    reader.Close();

                    DBConnection.updateDB("insert into issued (place_id, in_place_id, batch_id, date, issuedQty) values (" + place_id + ", " + in_place_id + ", " + batch_id + ", '" + DateTime.Now.ToString("yyyy/M/d") + "', " + qty + ")");
                }
            }
        }

        public static int issueFromExcel(string inPlace, string place, string color, string size, string article, int qty)
        {
            int batch_id = 1;
            int place_id = 1;
            int in_place_id = 1;
            int stat = 0;

            MySqlDataReader reader = DBConnection.getData("select batch_id from batch where color='" + color + "' and size='" + size + "' and article='" + article + "'");

            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    batch_id = reader.GetInt32(0);
                }

                reader.Close();

                MySqlDataReader readerPlace = DBConnection.getData("select place_id from place where place='" + place + "'");

                if (readerPlace.Read())
                {
                    place_id = readerPlace.GetInt32(0);

                    readerPlace.Close();

                    readerPlace = DBConnection.getData("select in_place_id from in_place where in_place_name='" + inPlace + "'");

                    if (readerPlace.Read())
                    {
                        in_place_id = readerPlace.GetInt32(0);

                        readerPlace.Close();

                        MySqlDataReader readerCheck = DBConnection.getData("SELECT IFNULL((SUM(r.receivedQty) - IFNULL(i.issued, 0)), 0) as balance " +
                                                                          "FROM received r LEFT JOIN (SELECT batch_id, SUM(issuedQty) as issued FROM issued " +
                                                                          "WHERE batch_id=" + batch_id + " and in_place_id=" + in_place_id + " GROUP BY batch_id) i on i.batch_id=r.batch_id INNER JOIN in_place p ON p.in_place_id=r.in_place_id " +
                                                                          "WHERE r.batch_id=" + batch_id + " and r.in_place_id=" + in_place_id + " GROUP BY r.batch_id, p.in_place_name;");

                        if (readerCheck.HasRows)
                        {
                            while (readerCheck.Read())
                            {
                                if (readerCheck.GetInt32("balance") > 0)
                                {
                                    reader = DBConnection.getData("select * from issued where place_id=" + place_id + " and in_place_id=" + in_place_id + " and batch_id=" + batch_id + " and date='" + DateTime.Now.ToString("yyyy/M/d") + "'");

                                    if (reader.HasRows)
                                    {
                                        int currentQty = 0;

                                        while (reader.Read())
                                        {
                                            currentQty = reader.GetInt32("issuedQty");
                                        }

                                        int newQty = currentQty + qty;

                                        reader.Close();

                                        DBConnection.updateDB("update issued set issuedQty=" + newQty + " where place_id=" + place_id + " and in_place_id=" + in_place_id + " and batch_id=" + batch_id + " and date='" + DateTime.Now.ToString("yyyy/M/d") + "'");
                                    }
                                    else
                                    {
                                        reader.Close();

                                        DBConnection.updateDB("insert into issued (place_id, in_place_id, batch_id, date, issuedQty) values (" + place_id + ", " + in_place_id + ", " + batch_id + ", '" + DateTime.Now.ToString("yyyy/M/d") + "', " + qty + ")");
                                    }
                                }
                                else
                                {
                                    readerCheck.Close();
                                    stat = 3;
                                }
                            }
                        }
                    }
                    else
                    {
                        readerPlace.Close();
                        stat = 4;
                    }
                }
                else
                {
                    readerPlace.Close();
                    stat = 2;
                }
            }
            else
            {
                reader.Close();
                stat = 1;
            }

            return stat;
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
                    
                    reader = DBConnection.getData("select * from issued where place_id=" + place_id + " and batch_id=" + batch_id + " and date='" + DateTime.Now.ToString("yyyy/M/d") + "'");

                    if (reader.HasRows)
                    {
                        int currentQty = 0;

                        while (reader.Read())
                        {
                            currentQty = reader.GetInt32("issuedQty");
                        }

                        int newQty = currentQty + qty;

                        reader.Close();

                        DBConnection.updateDB("update issued set issuedQty=" + newQty + " where place_id=" + place_id + " and batch_id=" + batch_id + " and date='" + DateTime.Now.ToString("yyyy/M/d") + "'");
                    }
                    else
                    {
                        reader.Close();

                        DBConnection.updateDB("insert into issued (place_id, batch_id, date, issuedQty) values (" + place_id + ", " + batch_id + ", '" + DateTime.Now.ToString("yyyy/M/d") + "', " + qty + ")");
                    }
                }
            }
        }

        public static void receive(Received rcv)
        {
            try
            {
                int place_id = 0;

                MySqlDataReader readerPlace = DBConnection.getData("select in_place_id from in_place where in_place_name='" + rcv.Place.GetPlace() + "'");

                if (readerPlace.Read())
                {
                    place_id = readerPlace.GetInt32(0);

                    readerPlace.Close();

                    MySqlDataReader reader = DBConnection.getData("select * from received where batch_id=" + rcv.Batch.Batch_id + " and in_place_id=" + place_id + " and date='" + rcv.Date.ToString("yyyy/M/d") + "'");

                    if (reader.HasRows)
                    {
                        int currentQty = 0;

                        while (reader.Read())
                        {
                            currentQty = reader.GetInt32("receivedQty");
                        }

                        reader.Close();

                        int totalQty = currentQty + rcv.ReceivedQty;

                        DBConnection.updateDB("update received set receivedQty=" + totalQty + " where batch_id=" + rcv.Batch.Batch_id + " and in_place_id=" + place_id + " and date='" + rcv.Date.ToString("yyyy/M/d") + "'");
                    }
                    else
                    {
                        reader.Close();

                        DBConnection.updateDB("insert into received (batch_id, in_place_id, date, receivedQty) values (" + rcv.Batch.Batch_id + ", " + place_id + ", '" + rcv.Date.ToString("yyyy/M/d") + "', " + rcv.ReceivedQty + ")");
                    }
                }
                else
                {
                    MessageBox.Show("In Place doesn't exist!", "DB Uploader", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public static void saveInPlace(Place place)
        {
            string placeName = place.GetPlace();

            DBConnection.updateDB("insert into in_place (in_place_name) values ('" + placeName + "')");
        }
    }
}
