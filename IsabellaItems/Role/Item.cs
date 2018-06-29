using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsabellaItems.Role
{
    class Item
    {
        int item_id;
        private DateTime date;
        string color;
        string size;
        string article;
        private bool issued;
        private IssuedPlace place;
        private Department dept;

        public Item(int item_id, bool issued, IssuedPlace place)
        {
            this.item_id = item_id;
            this.issued = issued;
            this.place = place;
        }

        public Item(DateTime date, string color, string size, string article, Department dept)
        {
            this.date = date;
            this.color = color;
            this.size = size;
            this.article = article;
            this.dept = dept;
        }

        public Item(string color, string size, string article)
        {
            this.color = color;
            this.size = size;
            this.article = article;
        }

        public void setColor(string color)
        {
            this.color = color;
        }

        public string getColor()
        {
            return color;
        }

        public void setSize(string size)
        {
            this.size = size;
        }

        public string getSize()
        {
            return size;
        }

        public void setArticle(string article)
        {
            this.article = article;
        }

        public string getArticle()
        {
            return article;
        }

        public void issue(IssuedPlace place)
        {
            issued = true;
            this.place = place;
        }

        public bool isIssued()
        {
            return issued;
        }

        public IssuedPlace getPlace_id()
        {
            return place;
        }

        public void setDept(Department dept)
        {
            this.dept = dept;
        }

        public Department getDept()
        {
            return dept;
        }

        public void setDate(DateTime date)
        {
            this.date = date;
        }

        public DateTime getDate()
        {
            return date;
        }
    }
}
