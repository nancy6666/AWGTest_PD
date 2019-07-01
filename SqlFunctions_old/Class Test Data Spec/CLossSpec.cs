using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CLossSpec
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
        public float Min_at_lw_min
        {
            private set;
            get;
        }
        public float Min_at_lw_max
        {
            private set;
            get;
        }
        public float Max_at_lw_min
        {
            private set;
            get;
        }
        public float Max_at_lw_max
        {
            private set;
            get;
        }
        public float Min_at_itu_min
        {
            private set;
            get;
        }
        public float Min_at_itu_max
        {
            private set;
            get;
        }
        public float Max_at_itu_min
        {
            private set;
            get;
        }
        public float Max_at_itu_max
        {
            private set;
            get;
        }
        public float Min_at_cw_min
        {
            private set;
            get;
        }
        public float Min_at_cw_max
        {
            private set;
            get;
        }
        public float Max_at_cw_min
        {
            private set;
            get;
        }
        public float Max_at_cw_max
        {
            private set;
            get;
        }
        public float Ripple_min
        {
            private set;
            get;
        }
        public float Ripple_max
        {
            private set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int common_spec_id, out List<CLossSpec> lstlossspec)
        {
            lstlossspec = new List<CLossSpec>();
            cmd.Parameters.Clear();
            try
            {
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Common_id",
                    DbType = DbType.Int32,
                    Value = common_spec_id
                });

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.select_loss_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CLossSpec spec = new CLossSpec()
                        {
                            RawID = (int)dr["id"],
                            Common_id = (int)dr["common_id"],
                            Class_id = (int)dr["class_id"],
                            Channel = (int)dr["channel"],
                            Max_at_cw_min = (float)Convert.ToDouble(dr["max_at_cw_min"]),
                            Max_at_cw_max = (float)Convert.ToDouble(dr["max_at_cw_max"]),
                            Max_at_itu_max = (float)Convert.ToDouble(dr["max_at_itu_max"]),
                            Max_at_itu_min = (float)Convert.ToDouble(dr["max_at_itu_min"]),
                            Max_at_lw_max = (float)Convert.ToDouble(dr["max_at_lw_max"]),
                            Max_at_lw_min = (float)Convert.ToDouble(dr["max_at_lw_min"]),
                            Min_at_cw_max = (float)Convert.ToDouble(dr["min_at_cw_max"]),
                            Min_at_cw_min = (float)Convert.ToDouble(dr["min_at_cw_min"]),
                            Min_at_itu_max = (float)Convert.ToDouble(dr["min_at_itu_max"]),
                            Min_at_itu_min = (float)Convert.ToDouble(dr["min_at_itu_min"]),
                            Min_at_lw_max = (float)Convert.ToDouble(dr["min_at_lw_max"]),
                            Min_at_lw_min = (float)Convert.ToDouble(dr["min_at_lw_min"]),
                            Ripple_max = (float)Convert.ToDouble(dr["ripple_max"]),
                            Ripple_min = (float)Convert.ToDouble(dr["ripple_min"])
                        };
                        if (!lstClassId.Where(n => n == spec.Class_id).Any())
                        {
                            lstClassId.Add(spec.Class_id);
                        }
                        lstlossspec.Add(spec);
                    }
                }
                else
                {
                    throw new Exception("读取LossSpec为null！");
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
