using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsabellaItems.Role
{
    class Department
    {
        int deptNo;
        string deptName;

        public Department(int deptNo)
        {
            this.deptNo = deptNo;
        }

        public Department(int deptNo, string deptName)
        {
            this.deptNo = deptNo;
            this.deptName = deptName;
        }

        public Department(string deptName)
        {
            this.deptName = deptName;
        }

        public int getDeptNo()
        {
            return deptNo;
        }

        public string getDeptName()
        {
            return deptName;
        }
    }
}
