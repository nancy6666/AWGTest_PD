using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    /// <summary>
    /// 测试条件
    /// </summary>
    public class CCond
    {
        #region Property

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
        public string Class_name
        {
            set;
            get;
        }
        public double Il_loss_win
        {
            set;
            get;
        }
        public double Ripple_loss_win
        {
            set;
            get;
        }
        public double Xtalk_loss_win
        {
            set;
            get;
        }
        public double Align_il
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetSpec(SqlCommand cmd, int commonSpecId, out List<CCond> lstCondSpec)
        {
            lstCondSpec = new List<CCond>();
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
                cmd.CommandText = "dbo.select_common_cond_spec";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CCond cond = new CCond()
                        {
                            Class_id = (int)dr["class_id"],
                            Class_name = dr["class_name"].ToString(),
                            Il_loss_win = Convert.ToDouble(dr["il_loss_win"]),
                            Xtalk_loss_win = Convert.ToDouble(dr["xtalk_loss_win"]),
                            Align_il = Convert.ToDouble(dr["align_il"]),
                            Ripple_loss_win = Convert.ToDouble(dr["ripple_loss_win"])
                        };

                        lstClassId.Add(cond.Class_id);
                        lstCondSpec.Add(cond);
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
