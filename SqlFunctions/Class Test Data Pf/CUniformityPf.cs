using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlFunctions.Class_Test_Data_Pf
{
    public class CUniformityPf
    {
        #region  Property
     public int Spec_class_id { get; set; }
        public int Uniformity_per_class_id
        {
            set;
            get;
        }
        public bool Predicted_temp
        {
            set;
            get;
        }
        public bool Uniformity
        {
            set;
            get;
        }
        #endregion

        #region Pulbic Methods

        public void SaveUniformityPf(SqlCommand cmd,int commonID)
        {
            cmd.Parameters.Clear();
          
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Uniformity_per_class_id",
                DbType = DbType.Int32,
                Value = this.Uniformity_per_class_id
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Predicted_temperature",
                DbType = DbType.Boolean,
                Value = this.Predicted_temp
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                ParameterName = "Uniformity",
                DbType = DbType.Boolean,
                Value = this.Uniformity
            });
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "insert_test_uniformity_pf";
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}