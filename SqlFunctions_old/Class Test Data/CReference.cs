using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CReference
    {
        #region Property

        public int Common_id
        {
            set;
            get;
        }
        public int Channel
        {
            set;
            get;
        }
        public float Reference
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void Save(SqlCommand cmd, int commonRowId)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Common_id",
                DbType = DbType.Int32,
                Value = commonRowId
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Channel",
                DbType = DbType.Int32,
                Value = this.Channel
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Reference",
                DbType = DbType.Double,
                Value = this.Reference
            });
            cmd.CommandText = "dbo.insert_test_ret_extention";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
