using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CLossPf : ITestDetailPf
    {
        #region Property

        public int Class_Id { get; set; }
        public bool PfRet { get; set; }
        public int Ret_id
        {
            set;
            get;
        }
        public bool Min_at_lw
        {
            set;
            get;
        }
        public bool Max_at_lw
        {
            set;
            get;
        }
        public bool Min_at_itu
        {
            set;
            get;
        }
        public bool Max_at_itu
        {
            set;
            get;
        }
        public bool Min_at_cw
        {
            set;
            get;
        }
        public bool Max_at_cw
        {
            set;
            get;
        }
        public bool Ripple
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
                    ParameterName = "Max_at_cw",
                    DbType = DbType.Boolean,
                    Value = this.Max_at_cw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Max_at_itu",
                    DbType = DbType.Boolean,
                    Value = this.Max_at_itu
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Max_at_lw",
                    DbType = DbType.Boolean,
                    Value = this.Min_at_cw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Min_at_lw",
                    DbType = DbType.Boolean,
                    Value = this.Min_at_lw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ripple",
                    DbType = DbType.Boolean,
                    Value = this.Ripple
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Min_at_cw",
                    DbType = DbType.Boolean,
                    Value = this.Max_at_cw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Min_at_itu",
                    DbType = DbType.Boolean,
                    Value = this.Max_at_itu
                });

                cmd.CommandText = "dbo.insert_test_ret_loss_pf";
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
