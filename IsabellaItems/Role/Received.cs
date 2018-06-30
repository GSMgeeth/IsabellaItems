using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsabellaItems.Role
{
    class Received
    {
        private Batch batch;
        private DateTime date;
        private int receivedQty;

        public Received(Batch batch, DateTime date, int receivedQty)
        {
            this.batch = batch;
            this.date = date;
            this.receivedQty = receivedQty;
        }

        public DateTime Date { get => date; set => date = value; }
        public int ReceivedQty { get => receivedQty; set => receivedQty = value; }
        internal Batch Batch { get => batch; set => batch = value; }
    }
}
