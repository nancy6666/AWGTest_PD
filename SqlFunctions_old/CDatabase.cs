using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace SqlFunctions
{
    public class CDatabase
    {
        SqlConnection conn;
      
        #region Constructor

        public CDatabase()
        {
            CConfigurationManagement cfg = new CConfigurationManagement();
            conn = new SqlConnection();
            conn.ConnectionString = string.Format("Server={0}; Database={1} ;User id={2}; PWD={3}",cfg.DBIP,cfg.DBName, cfg.DBUser,cfg.DBPassword);
        }
        #endregion

        #region Private Method

        public bool Open(out SqlCommand cmd)
        {
            cmd = null;
            try
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                cmd = new SqlCommand() { Transaction = trans, Connection = conn };
                return true;
            }
            catch (Exception ex)
            {               
                return false;
                throw ex;
            }
        }

        public bool Close()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (Exception ex)
            { 
                return false;
                throw ex;
            }
        }
        #endregion

    }
}