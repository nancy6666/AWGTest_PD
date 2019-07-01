using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CPassbandSpec
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
        public float At_05db_min
        {
            private set;
            get;
        }
        public float At_05db_max
        {
            private set;
            get;
        }
        public float At_1db_min
        {
            private set;
            get;
        }
        public float At_1db_max
        {
            private set;
            get;
        }
        public float At_3db_min
        {
            private set;
            get;
        }
        public float At_3db_max
        {
            private set;
            get;
        }
        public float At_20db_min
        {
            private set;
            get;
        }
        public float At_20db_max
        {
            private set;
            get;
        }
        public float At_25db_min
        {
            private set;
            get;
        }
        public float At_25db_max
        {
            private set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<CPassbandSpec> lstpassbandspec)
        {
            lstpassbandspec = new List<CPassbandSpec>();
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
                cmd.CommandText = "dbo.select_passband_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CPassbandSpec spec = new CPassbandSpec()
                        {
                            RawID = (int)dr["id"],
                            Common_id = (int)dr["common_id"],
                            Class_id = (int)dr["class_id"],
                            Channel = (int)dr["channel"],
                            At_05db_max = (float)Convert.ToDouble(dr["at_05db_max"]),
                            At_05db_min = (float)Convert.ToDouble(dr["at_05db_min"]),
                            At_1db_max = (float)Convert.ToDouble(dr["at_1db_max"]),
                            At_1db_min = (float)Convert.ToDouble(dr["at_1db_min"]),
                            At_3db_max = (float)Convert.ToDouble(dr["at_3db_max"]),
                            At_3db_min = (float)Convert.ToDouble(dr["at_3db_min"]),
                            At_20db_max = (float)Convert.ToDouble(dr["at_20db_max"]),
                            At_20db_min = (float)Convert.ToDouble(dr["at_20db_min"]),
                            At_25db_max = (float)Convert.ToDouble(dr["at_25db_max"]),
                            At_25db_min = (float)Convert.ToDouble(dr["at_25db_min"])

                        };

                        if (!lstClassId.Where(n => n == spec.Class_id).Any())
                        {
                            lstClassId.Add(spec.Class_id);
                        }
                        lstpassbandspec.Add(spec);
                    }
                }
                else
                {
                    throw new Exception("读取PassbandSpec为null！");
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
