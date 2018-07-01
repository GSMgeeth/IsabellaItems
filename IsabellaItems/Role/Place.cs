using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsabellaItems.Role
{
    public class Place
    {
        private int place_id;
        private string place;

        public Place()
        {

        }

        public Place(string place)
        {
            this.place = place;
        }

        public Place(int place_id, string place)
        {
            this.place_id = place_id;
            this.place = place;
        }

        public int Place_id { get => place_id; set => place_id = value; }

        public string GetPlace()
        {
            return place;
        }

        public void SetPlace(string value)
        {
            place = value;
        }
    }
}
