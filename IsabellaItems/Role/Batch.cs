using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsabellaItems.Role
{
    public class Batch
    {
        private int batch_id;
        private string color;
        private string size;
        private string article;

        public int Batch_id { get => batch_id; set => batch_id = value; }
        public string Color { get => color; set => color = value; }
        public string Size { get => size; set => size = value; }
        public string Article { get => article; set => article = value; }

        public Batch()
        {

        }

        public Batch(int batch_id, string color, string size, string article)
        {
            this.batch_id = batch_id;
            this.color = color;
            this.size = size;
            this.article = article;
        }

        public Batch(string color, string size, string article)
        {
            this.color = color;
            this.size = size;
            this.article = article;
        }
    }
}
