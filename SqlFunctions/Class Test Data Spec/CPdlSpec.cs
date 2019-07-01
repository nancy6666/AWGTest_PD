using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CPdlSpec
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
            set;
            get;
        }
        public int Class_id
        {
            private set;
            get;
        }
        public int Channel
        {
            set;
            get;
        }
        public float Pdl_at_itu_min
        {
            set;
            get;
        }
        public float Pdl_at_itu_max
        {
            set;
            get;
        }
        public float Pdl_at_ctr_min
        {
            set;
            get;
        }
        public float Pdl_at_ctr_max
        {
            set;
            get;
        }
        public float Pdl_max_min
        {
            set;
            get;
        }
        public float Pdl_max_max
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<CPdlSpec> lstpdlspec)
        {
            lstpdlspec = new List<CPdlSpec>();
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
                cmd.CommandText = "dbo.select_pdl_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CPdlSpec spec = new CPdlSpec()
                        {
                            RawID = (int)dr["id"],
                            Common_id = (int)dr["common_id"],
                            Class_id = (int)dr["class_id"],
                            Channel = (int)dr["channel"],
                            Pdl_at_ctr_max = (float)Convert.ToDouble(dr["pdl_at_ctr_max"]),
                            Pdl_at_ctr_min = (float)Convert.ToDouble(dr["pdl_at_ctr_min"]),
                            Pdl_at_itu_min = (float)Convert.ToDouble(dr["pdl_at_itu_min"]),
                            Pdl_at_itu_max = (float)Convert.ToDouble(dr["pdl_at_itu_max"]),
                            Pdl_max_max = (float)Convert.ToDouble(dr["pdl_max_max"]),
                            Pdl_max_min = (float)Convert.ToDouble(dr["pdl_max_min"]),

                        };
                        if (!lstClassId.Where(n => n == spec.Class_id).Any())
                        {
                            lstClassId.Add(spec.Class_id);
                        }
                        lstpdlspec.Add(spec);
                    }
                }
                else
                {
                    throw new Exception("读取PdlSpec为null！");
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
