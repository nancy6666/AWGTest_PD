using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlFunctions
{
   public class SpecPerClass
    {
        #region Properties
        public List<int> lstClassId = new List<int>();
        public int Common_id
        {
            set;
            get;
        }
        public int Class_id
        {
            set;
            get;
        }     
        public double Predicted_temp_min
        {
            set;
            get;
        }
       
        public double Predicted_temp_max
        {
            set;
            get;
        }
        public double Uniformity_min
        {
            set;
            get;
        }

        public double Uniformity_max
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<SpecPerClass> lstPerClassSpec)
        {
            lstPerClassSpec = new List<SpecPerClass>();
            cmd.Parameters.Clear();
            string sql = $"select * from plc_production_test_spec_per_class where common_id={commonSpecId}";
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SpecPerClass cond = new SpecPerClass()
                        {
                            Class_id = (int)dr["class_id"],
                            Predicted_temp_min = (double)Convert.ToDouble(dr["predicted_temp_min"]),
                            Predicted_temp_max = (double)Convert.ToDouble(dr["predicted_temp_max"]),
                            Uniformity_max = (double)Convert.ToDouble(dr["uniformity_max"]),
                            Uniformity_min = (double)Convert.ToDouble(dr["uniformity_min"])
                        };

                        lstClassId.Add(cond.Class_id);
                        lstPerClassSpec.Add(cond);
                    }
                }
                else
                {
                    throw new Exception("读取CondSpec为null！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion
    }
}
