using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace SqlFunctions
{
    public class CDatabase : IDisposable
    {
        SqlConnection conn;

        #region Constructor

        public CDatabase()
        {
            CConfigurationManagement cfg = new CConfigurationManagement();
            conn = new SqlConnection();
            conn.ConnectionString = string.Format("Server={0}; Database={1} ;User id={2}; PWD={3}", cfg.DBIP, cfg.DBName, cfg.DBUser, cfg.DBPassword);
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
        public void Open()
        {
            try
            {
                conn.Open();

            }
            catch (Exception ex)
            {
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

        private void ExecuteRawSQL(string Expression)
        {
            // try to open the connection
            //try
            //{
            //    conn.Open();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"Unable to open SQL Server connection, {ex.Message}");
            //}

            // try to execute query 
            try
            {
                using (var cmd = new SqlCommand(Expression, conn))
                {

                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region Public Methods

        public void SaveMesData(string post_json, string created_by)
        {
            string sql = $"insert into dbo.MES_TEST_DATA (post_json,create_by) values('{post_json}','{created_by}')";
            ExecuteRawSQL(sql);
        }

        public void Dispose()
        {
            this.Close();
        }
        #endregion
    }
}