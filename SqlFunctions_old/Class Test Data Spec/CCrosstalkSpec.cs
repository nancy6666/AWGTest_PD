using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CCrosstalkSpec
    {
        #region Property

        public List<int> lstClassId = new List<int>();
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
        public float Ax_n_min
        {
            private set;
            get;
        }
        public float Ax_n_max
        {
            private set;
            get;
        }
        public float Ax_p_min
        {
            private set;
            get;
        }
        public float Ax_p_max
        {
            private set;
            get;
        }
        public float Ax_min
        {
            private set;
            get;
        }
        public float Ax_max
        {
            private set;
            get;
        }
        public float Nx_min
        {
            private set;
            get;
        }
        public float Nx_max
        {
            private set;
            get;
        }
        public float Tx_min
        {
            private set;
            get;
        }
        public float Tx_max
        {
            private set;
            get;
        }
        public float Tnx_min
        {
            private set;
            get;
        }
        public float Tnx_max
        {
            private set;
            get;
        }
        public float Tax_min
        {
            private set;
            get;
        }
        public float Tax_max
        {
            private set;
            get;
        }
        public int RawID
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<CCrosstalkSpec> lstCrosstalkSpec)
        {
            lstCrosstalkSpec = new List<CCrosstalkSpec>();
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
                cmd.CommandText = "dbo.select_crosstalk_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CCrosstalkSpec spec = new CCrosstalkSpec()
                        {
                            RawID = (int)dr["id"],
                            Common_id = (int)dr["common_id"],
                            Class_id = (int)dr["class_id"],
                            Channel = (int)dr["channel"],
                            Nx_min = (float)Convert.ToDouble(dr["nx_min"]),
                            Nx_max = (float)Convert.ToDouble(dr["nx_max"]),
                            Tnx_min = (float)Convert.ToDouble(dr["tnx_min"]),
                            Tnx_max = (float)Convert.ToDouble(dr["tnx_max"]),
                            Tx_min = (float)Convert.ToDouble(dr["tx_min"]),
                            Tx_max = (float)Convert.ToDouble(dr["tx_max"]),
                            Ax_min = (float)Convert.ToDouble(dr["ax_min"]),
                            Ax_max = (float)Convert.ToDouble(dr["ax_max"]),
                            Ax_p_min = (float)Convert.ToDouble(dr["ax_p_min"]),
                            Ax_p_max = (float)Convert.ToDouble(dr["ax_p_max"]),
                            Ax_n_min = (float)Convert.ToDouble(dr["ax_n_min"]),
                            Ax_n_max = (float)Convert.ToDouble(dr["ax_n_max"]),
                            Tax_min = (float)Convert.ToDouble(dr["tax_min"]),
                            Tax_max = (float)Convert.ToDouble(dr["tax_max"])
                        };

                        if (!lstClassId.Where(n => n == spec.Class_id).Any())
                        {
                            lstClassId.Add(spec.Class_id);
                        }
                        lstCrosstalkSpec.Add(spec);
                    }
                }
                else
                {
                    throw new Exception("读取CrosstalkSpec为null！");
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
