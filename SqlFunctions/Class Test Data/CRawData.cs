using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CRawData
    {
        #region Property

        public byte[] RawData
        {
            set;
            get;
        }
        public string File_ext
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
                ParameterName = "Id",
                DbType = DbType.Int32,
                Value = commonRowId
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Rawdata",
                DbType = DbType.Binary,
                Value = this.RawData
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "File_ext",
                DbType = DbType.String,
                Value = this.File_ext
            });
            cmd.CommandText = "dbo.insert_test_ret_rawdata";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
