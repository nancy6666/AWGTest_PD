using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

namespace TestSystem.DataBase
{
    class clsDataBaseInfo
    {
        public static OleDbConnection oleDbComCon;
        public static OleDbDataAdapter oleDbComAdp;
    }
}
