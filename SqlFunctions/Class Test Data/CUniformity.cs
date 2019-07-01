using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlFunctions.Class_Test_Data_Pf;

namespace SqlFunctions.Class_Test_Data
{
  public class CUniformity
    {
        #region Properties

        public int RowID { get; set; }
        public int CommonID { get; set; }
        public int SpecClassID { get; set; }
        public double PredictedTemp { get; set; }
        public double Uniformity { get; set; }
        public SpecPerClass specUniformity { get; set; }
        #endregion

        #region Public Methods

        public void SaveUniformity(SqlCommand cmd,int commonID)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Common_id",
                    Value = commonID,
                    DbType = DbType.Int32
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Spec_class_id",
                    Value = this.SpecClassID,
                    DbType = DbType.Int32
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Predicted_temperature",
                    Value = this.PredictedTemp,
                    DbType = DbType.Double
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Uniformity",
                    Value = this.Uniformity,
                    DbType = DbType.Double
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Int32
                }).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insert_test_ret_uniformity";
                cmd.ExecuteNonQuery();
                this.RowID = Convert.ToInt32(cmd.Parameters["ID"].Value);
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Get Uniformity pf reuslt based on certain spec class
        /// </summary>
        /// <param name="uniformityPf"></param>
        public void GetUniformityPf(out CUniformityPf uniformityPf)
        {
            uniformityPf = new CUniformityPf();
            uniformityPf.Spec_class_id = this.SpecClassID;
            if (this.Uniformity >= specUniformity.Uniformity_min & this.Uniformity <= specUniformity.Uniformity_max)
            {
                uniformityPf.Uniformity = true;
            }
            else
            {
                uniformityPf.Uniformity = false;
            }
            if (this.PredictedTemp >= specUniformity.Predicted_temp_min & this.PredictedTemp <= specUniformity.Predicted_temp_max)
            {
                uniformityPf.Predicted_temp = true;
            }
            else
            {
                uniformityPf.Predicted_temp = false;
            }
           
        }
       
        #endregion

    }
}
