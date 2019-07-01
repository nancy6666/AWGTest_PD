using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CUsers
    {

        public string Name
        {
            private set;
            get;
        }
        public string Work_id
        {
            private set;
            get;
        }
        public string Passwd
        {
            private set;
            get;
        }
        public int Role
        {
            private set;
            get;
        }
        public int Created_by
        {
            private set;
            get;
        }
        public bool Is_admin
        {
            private set;
            get;
        }
        public bool Is_active
        {
            private set;
            get;
        }
    }
}
