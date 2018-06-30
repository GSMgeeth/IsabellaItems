using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }

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
