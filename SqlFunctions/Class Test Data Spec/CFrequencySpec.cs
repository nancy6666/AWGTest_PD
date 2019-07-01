using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{

    public class CFrequencySpec
    {
        #region Property
        public List<int> lstClassId = new List<int>();
        public int RawID
        {
            set;
            get;
        }
        public int Common_id
        {
            private set;
            get;
        }
        public int Class_id
        {
            private set;
            get;
        }
        public int Channel
        {
            private set;
            get;
        }
        public float Freq_min
        {
            private set;
            get;
        }
        public float Freq_max
        {
            private set;
            get;
        }
        public float Offset_min
        {
            private set;
            get;
        }
        public float Offset_max
        {
            private set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<CFrequencySpec> lstFrequencySpec)
        {
            lstFrequencySpec = new List<CFrequencySpec>();
            cmd.Parameters.Clear();
            try
            {
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Common_id",
                    DbType = DbType.Int32,
                    Value = commonSpecId
                });

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.select_frequency_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CFrequencySpec spec = new CFrequencySpec()
                        {
                            RawID = (int)dr["id"],
                            Common_id = (int)dr["common_id"],
                            Class_id = (int)dr["class_id"],
                            Channel = (int)dr["channel"],
                            Freq_max = (float)Convert.ToDouble(dr["freq_max"]),
                            Freq_min = (float)Convert.ToDouble(dr["freq_min"]),
                            Offset_max = (float)Convert.ToDouble(dr["offset_max"]),
                            Offset_min = (float)Convert.ToDouble(dr["offset_min"])

                        };
                        if (!lstClassId.Where(n => n == spec.Class_id).Any())
                        {
                            lstClassId.Add(spec.Class_id);
                        }
                        lstFrequencySpec.Add(spec);
                    }
                }
                else
                {
                    throw new Exception("读取FrequnecySpec为null！");
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
