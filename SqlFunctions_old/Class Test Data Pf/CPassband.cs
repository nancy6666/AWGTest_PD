using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CPassbandPf : ITestDetailPf
    {
        #region Property

        public int Class_Id { get; set; }
        public bool PfRet { get; set; }
        public int Ret_id
        {
            set;
            get;
        }
        public bool At_05db
        {
            set;
            get;
        }
        public bool At_1db
        {
            set;
            get;
        }
        public bool At_3db
        {
            set;
            get;
        }
        public bool At_20db
        {
            set;
            get;
        }
        public bool At_25db
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void SaveTestDetailPf(SqlCommand cmd)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ret_id",
                    DbType = DbType.Int32,
                    Value = this.Ret_id


                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_05db",
                    DbType = DbType.Boolean,
                    Value = this.At_05db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_1db",
                    DbType = DbType.Boolean,
                    Value = this.At_1db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_3db",
                    DbType = DbType.Boolean,
                    Value = this.At_3db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_20db",
                    DbType = DbType.Boolean,
                    Value = this.At_20db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_25db",
                    DbType = DbType.Boolean,
                    Value = this.At_25db
                });

                cmd.CommandText = "dbo.insert_test_ret_passband_pf";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
