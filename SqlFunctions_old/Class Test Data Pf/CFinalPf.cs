using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CFinalPf
    {
        #region Property

        public int Test_id
        {
            set;
            get;
        }
        public int Spec_class_id
        {
            set;
            get;
        }
        public bool Pf
        {
            set;
            get;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// upload final pf result to database
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="commonrowid"></param>
        public void SaveFinalPf(SqlCommand cmd, int commonRowId)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Test_id",
                DbType = DbType.Int32,
                Value = commonRowId
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Spec_class_id",
                DbType = DbType.Int32,
                Value = this.Spec_class_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Pf",
                DbType = DbType.Boolean,
                Value = this.Pf
            });
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.insert_test_ret_pf";
            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
