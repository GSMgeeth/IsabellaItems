using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsabellaItems.Role
{
    public class Issued
    {
        private Batch batch;
        private Place place;
        private DateTime date;
        private int issuedQty;

        public Issued(Batch batch, Place place, DateTime date, int issuedQty)
        {
            this.batch = batch;
            this.place = place;
            this.date = date;
            this.issuedQty = issuedQty;
        }

        public DateTime Date { get => date; set => date = value; }
        public int IssuedQty { get => issuedQty; set => issuedQty = value; }
        public Batch Batch { get => batch; set => batch = value; }
        public Place Place { get => place; set => place = value; }
    }
}
