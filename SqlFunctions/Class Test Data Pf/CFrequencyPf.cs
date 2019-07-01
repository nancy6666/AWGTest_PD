using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CFrequencyPf : ITestDetailPf
    {
        #region Property

        public int Class_Id { get; set; }
        public bool PfRet { get; set; }
        public int Ret_id
        {
            set;
            get;
        }
        public bool Freq
        {
            set;
            get;
        }
        public bool Offset
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
                    ParameterName = "Offset",
                    DbType = DbType.Boolean,
                    Value = this.Offset
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Freq",
                    DbType = DbType.Boolean,
                    Value = this.Freq
                });

                cmd.CommandText = "dbo.insert_test_ret_frequency_pf";
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
