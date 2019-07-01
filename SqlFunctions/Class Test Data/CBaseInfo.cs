using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SqlFunctions
{
  public  class CBaseInfo
    {
        #region Property

        public int RowID
        {
            set;
            get;
        }
        public string Chip_id
        {
            set;
            get;
        }
        public string Wafer_id
        {
            set;
            get;
        }
        public string Bar_id
        {
            set;
            get;
        }
        public int Bar_order
        {
            set;
            get;
        }
        public int Spec_id
        {
            set;
            get;
        }
        public string Mask
        {
            set;
            get;
        }
        public string Travel_card
        {
            set;
            get;
        }
        public int Created_by
        {
            set;
            get;
        }
        #endregion

        #region Public Method

        public void Save(SqlCommand cmd)
        {          
            cmd.Parameters.Clear();

            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Chip_id",
                DbType = DbType.String,
                Value = this.Chip_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Wafer_id",
                DbType = DbType.String,
                Value = this.Wafer_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Bar_id",
                DbType = DbType.String,
                Value = this.Bar_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Bar_order",
                DbType = DbType.Int32,
                Value = this.Bar_order
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Spec_id",
                DbType = DbType.Int32,
                Value = this.Spec_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Mask",
                DbType = DbType.String,
                Value = this.Mask
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Travel_card",
                DbType = DbType.String,
                Value = this.Travel_card
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Created_by",
                DbType = DbType.Int32,
                Value = this.Created_by
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Identity",
                DbType = DbType.Int32
            }).Direction = ParameterDirection.Output;
            cmd.CommandText = "dbo.insert_baseinfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            this.RowID = (int)cmd.Parameters["Identity"].Value;


        }
        public void GetRowID(SqlCommand cmd,string chip_id)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName="Chip_id",
                DbType=DbType.String,
                Value=chip_id
            });          
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "ID",
                DbType = DbType.Int32
            }).Direction = ParameterDirection.Output;
            cmd.CommandText = "dbo.select_baseinfo_id";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            this.RowID=(int)cmd.Parameters["ID"].Value;
        }
        #endregion
    }
}
