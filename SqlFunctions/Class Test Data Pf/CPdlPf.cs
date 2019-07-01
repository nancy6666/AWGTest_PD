using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CPdlPf : ITestDetailPf
    {
        #region Property

        public int Class_Id { get; set; }
        public bool PfRet { get; set; }
        public int Ret_id
        {
            set;
            get;
        }
        public bool Pdl_at_itu
        {
            set;
            get;
        }
        public bool Pdl_at_ctr
        {
            set;
            get;
        }
        public bool Pdl_max
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
                    ParameterName = "Pdl_at_ctr",
                    DbType = DbType.Boolean,
                    Value = this.Pdl_at_ctr
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pdl_at_itu",
                    DbType = DbType.Boolean,
                    Value = this.Pdl_at_itu
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pdl_max",
                    DbType = DbType.Boolean,
                    Value = this.Pdl_max
                });

                cmd.CommandText = "dbo.insert_test_ret_pdl_pf";
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
