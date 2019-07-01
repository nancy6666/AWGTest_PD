using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CWavelengthSpec
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
        //public float Wavelength_min
        //{
        //    private set;
        //    get;
        //}
        //public float Wavelength_max
        //{
        //    private set;
        //    get;
        //}
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
        public float Pdw_min
        {
            private set;
            get;
        }
        public float Pdw_max
        {
            private set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<CWavelengthSpec> lstWavelengthSpec)
        {
            lstWavelengthSpec = new List<CWavelengthSpec>();
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
                cmd.CommandText = "dbo.select_wavelength_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CWavelengthSpec spec = new CWavelengthSpec()
                        {
                            RawID = (int)dr["id"],
                            Common_id = (int)dr["common_id"],
                            Class_id = (int)dr["class_id"],
                            Channel = (int)dr["channel"],
                            //Wavelength_max = (float)Convert.ToDouble(dr["wavelength_max"]),
                            //Wavelength_min = (float)Convert.ToDouble(dr["wavelength_min"]),
                            Offset_max = (float)Convert.ToDouble(dr["offset_max"]),
                            Offset_min = (float)Convert.ToDouble(dr["offset_min"]),
                            Pdw_max = (float)Convert.ToDouble(dr["pdw_max"]),
                            Pdw_min = (float)Convert.ToDouble(dr["pdw_min"])
                        };

                        if (!lstClassId.Where(n => n == spec.Class_id).Any())
                        {
                            lstClassId.Add(spec.Class_id);
                        }
                        lstWavelengthSpec.Add(spec);
                    }
                }

                else
                {
                    throw new Exception("读取WavelengthSpec为null！");
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
