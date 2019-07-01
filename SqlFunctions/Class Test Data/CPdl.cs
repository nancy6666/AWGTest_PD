using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CPdl : ITestDetail
    {
        #region Property

      public  CPdlSpec Spec = new CPdlSpec();
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
        public double Pdl_at_itu
        {
            set;
            get;
        }
        public double Pdl_at_ctr
        {
            set;
            get;
        }
        public double Pdl_max
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

                CPdlPf pf = new CPdlPf();

                pf.Ret_id = this.RawID;

                if (this.Pdl_at_ctr>= this.Spec.Pdl_at_ctr_min & this.Pdl_at_ctr <= this.Spec.Pdl_at_ctr_max)
                {
                    pf.Pdl_at_ctr = true;
                }
                else
                {
                    pf.Pdl_at_ctr = false;
                }
                if (this.Pdl_at_itu>= this.Spec.Pdl_at_itu_min & this.Pdl_at_itu <= this.Spec.Pdl_at_itu_max)
                {
                    pf.Pdl_at_itu = true;
                }
                else
                {
                    pf.Pdl_at_itu = false;
                }
                if (this.Pdl_max>= this.Spec.Pdl_max_min & this.Pdl_max <= this.Spec.Pdl_max_max)
                {
                    pf.Pdl_max = true;
                }
                else
                {
                    pf.Pdl_max = false;
                }
                pf.PfRet = pf.Pdl_at_ctr & pf.Pdl_at_itu & pf.Pdl_max;
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
                    ParameterName = "Pdl_at_ctr",
                    DbType = DbType.Double,
                    Value = this.Pdl_at_ctr
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pdl_at_itu",
                    DbType = DbType.Double,
                    Value = this.Pdl_at_itu
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pdl_max",
                    DbType = DbType.Double,
                    Value = this.Pdl_max
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Double
                }).Direction = ParameterDirection.Output;

                cmd.CommandText = "dbo.insert_test_ret_pdl";
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
