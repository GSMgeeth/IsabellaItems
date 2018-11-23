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
        private Place place;
        private DateTime date;
        private int receivedQty;

        public Received(DateTime date, int receivedQty, Batch batch, Place place)
        {
            Date = date;
            ReceivedQty = receivedQty;
            Batch = batch;
            Place = place;
        }

        public Received(Batch batch)
        {
            this.Batch = batch;
        }
        
        public DateTime Date { get => date; set => date = value; }
        public int ReceivedQty { get => receivedQty; set => receivedQty = value; }
        public Batch Batch { get => batch; set => batch = value; }
        public Place Place { get => place; set => place = value; }
    }
}
