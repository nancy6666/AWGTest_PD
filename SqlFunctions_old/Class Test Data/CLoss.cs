using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CLoss : ITestDetail
    {
        #region Property

        public CLossSpec Spec = new CLossSpec();
        public int Spec_class_id
        {
            set;
            get;
        }
        public int Channel
        {
            set;
            get;
        }
        public float Min_at_lw
        {
            set;
            get;
        }
        public float Max_at_lw
        {
            set;
            get;
        }
        public float Min_at_itu
        {
            set;
            get;
        }
        public float Max_at_itu
        {
            set;
            get;
        }
        public float Min_at_cw
        {
            set;
            get;
        }
        public float Max_at_cw
        {
            set;
            get;
        }
        public float Ripple
        {
            set;
            get;
        }
        public int RawID
        {
            set;
            get;
        }
        #endregion

        #region Public Methods

        public void GetPfRet(out ITestDetailPf testDetailPf)
        {
            testDetailPf = null;
            try
            {
                CLossPf pf = new CLossPf();

                pf.Ret_id = this.RawID;

                if (this.Max_at_cw > this.Spec.Max_at_cw_min & this.Max_at_cw < this.Spec.Max_at_cw_max)
                {
                    pf.Max_at_cw = true;
                }
                else
                {
                    pf.Max_at_cw = false;
                }
                if (this.Max_at_itu > this.Spec.Max_at_itu_min & this.Max_at_itu < this.Spec.Max_at_itu_max)
                {
                    pf.Max_at_itu = true;
                }
                else
                {
                    pf.Max_at_itu = false;
                }
                if (this.Max_at_lw > this.Spec.Max_at_lw_min & this.Max_at_lw < this.Spec.Max_at_lw_max)
                {
                    pf.Max_at_lw = true;
                }
                else
                {
                    pf.Max_at_lw = false;
                }
                if (this.Min_at_cw > this.Spec.Min_at_cw_min & this.Min_at_cw < this.Spec.Min_at_cw_max)
                {
                    pf.Min_at_cw = true;
                }
                else
                {
                    pf.Min_at_cw = false;
                }
                if (this.Min_at_itu > this.Spec.Min_at_itu_min & this.Min_at_itu < this.Spec.Min_at_itu_max)
                {
                    pf.Min_at_itu = true;
                }
                else
                {
                    pf.Min_at_itu = false;
                }
                if (this.Min_at_lw > this.Spec.Min_at_lw_min & this.Min_at_lw < this.Spec.Min_at_lw_max)
                {
                    pf.Min_at_lw = true;
                }
                else
                {
                    pf.Min_at_lw = false;
                }
                if (this.Ripple > this.Spec.Ripple_min & this.Ripple < this.Spec.Ripple_max)
                {
                    pf.Ripple = true;
                }
                else
                {
                    pf.Ripple = false;
                }

                pf.PfRet = pf.Max_at_cw & pf.Max_at_itu & pf.Max_at_lw & pf.Min_at_cw & pf.Min_at_itu & pf.Min_at_lw & pf.Ripple;
                pf.Class_Id = this.Spec.Class_id;
                testDetailPf = pf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveTestDetail(SqlCommand cmd, int commonRowId)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Test_id",
                    DbType = DbType.Int32,
                    Value = commonRowId

                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Spec_class_id",
                    DbType = DbType.Int32,
                    Value = this.Spec_class_id

                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Channel",
                    DbType = DbType.Int32,
                    Value = this.Channel

                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Max_at_cw",
                    DbType = DbType.Double,
                    Value = this.Max_at_cw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Max_at_itu",
                    DbType = DbType.Double,
                    Value = this.Max_at_itu
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Max_at_lw",
                    DbType = DbType.Double,
                    Value = this.Max_at_lw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Min_at_cw",
                    DbType = DbType.Double,
                    Value = this.Min_at_cw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Min_at_itu",
                    DbType = DbType.Double,
                    Value = this.Min_at_itu
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Min_at_lw",
                    DbType = DbType.Double,
                    Value = this.Min_at_lw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ripple",
                    DbType = DbType.Double,
                    Value = this.Ripple
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Double
                }).Direction = ParameterDirection.Output;

                cmd.CommandText = "dbo.insert_test_ret_loss";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                this.RawID = Convert.ToInt32(cmd.Parameters["ID"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
