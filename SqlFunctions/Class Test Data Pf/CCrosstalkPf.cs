using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CCrosstalkPf : ITestDetailPf
    {
        #region Property

        public bool PfRet { get; set; }
        public int Class_Id { get; set; }
        public int Ret_id
        {
            set;
            get;
        }
        public bool Ax_n
        {
            set;
            get;
        }
        public bool Ax_p
        {
            set;
            get;
        }
        public bool Ax
        {
            set;
            get;
        }
        public bool Nx
        {
            set;
            get;
        }
        public bool Tx
        {
            set;
            get;
        }
        public bool Tnx
        {
            set;
            get;
        }
        public bool Tax
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
                    ParameterName = "Ax_n",
                    DbType = DbType.Boolean,
                    Value = this.Ax_n
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ax_p",
                    DbType = DbType.Boolean,
                    Value = this.Ax_p
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ax",
                    DbType = DbType.Boolean,
                    Value = this.Ax
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Nx",
                    DbType = DbType.Boolean,
                    Value = this.Nx
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tx",
                    DbType = DbType.Boolean,
                    Value = this.Tx
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tnx",
                    DbType = DbType.Boolean,
                    Value = this.Tnx
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tax",
                    DbType = DbType.Boolean,
                    Value = this.Tax
                });

                cmd.CommandText = "dbo.insert_test_ret_crosstalk_pf";
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
