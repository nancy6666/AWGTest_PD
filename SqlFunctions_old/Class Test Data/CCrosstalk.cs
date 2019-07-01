using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SqlFunctions
{
    public class CCrosstalk : ITestDetail
    {
        #region Property

        public  CCrosstalkSpec Spec=new CCrosstalkSpec();
        public int RawID
        {
            set;
            get;
        }
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
        public float Ax_n
        {
            set;
            get;
        }
        public float Ax_p
        {
            set;
            get;
        }
        public float Ax
        {
            set;
            get;
        }
        public float Nx
        {
            set;
            get;
        }
        public float Tx
        {
            set;
            get;
        }
        public float Tnx
        {
            set;
            get;
        }
        public float Tax
        {
            set;
            get;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// 判断一组测试数据是否在客户要的spec内
        /// </summary>
        /// <param name="TestDetailPf">测试数据的判断结果</param>
        public void GetPfRet( out ITestDetailPf TestDetailPf)
        {
            TestDetailPf = null;
            try
            {
                CCrosstalkPf pf = new CCrosstalkPf();

                pf.Ret_id = this.RawID;

                if (this.Ax > this.Spec.Ax_min & this.Ax < this.Spec.Ax_max)
                {
                    pf.Ax = true;
                }
                else
                {
                    pf.Ax = false;
                }
                if (this.Ax_n > this.Spec.Ax_n_min & this.Ax_n < this.Spec.Ax_n_max)
                {
                    pf.Ax_n = true;
                }
                else
                {
                    pf.Ax_n = false;
                }
                if (this.Ax_p > this.Spec.Ax_p_min & this.Ax_p < this.Spec.Ax_p_max)
                {
                    pf.Ax_p = true;
                }
                else
                {
                    pf.Ax_p = false;
                }
                if (this.Tax > this.Spec.Tax_min & this.Tax < this.Spec.Tax_max)
                {
                    pf.Tax = true;
                }
                else
                {
                    pf.Tax = false;
                }
                if (this.Nx > this.Spec.Nx_min & this.Nx < this.Spec.Nx_max)
                {
                    pf.Nx = true;
                }
                else
                {
                    pf.Nx = false;
                }
                if (this.Tnx > this.Spec.Tnx_min & this.Tnx < this.Spec.Tnx_max)
                {
                    pf.Tnx = true;
                }
                else
                {
                    pf.Tnx = false;
                }
                if (this.Tx > this.Spec.Tx_min & this.Tx < this.Spec.Tx_max)
                {
                    pf.Tx = true;
                }
                else
                {
                    pf.Tx = false;
                }

                pf.PfRet = pf.Ax & pf.Ax & pf.Ax_p & pf.Nx & pf.Tax & pf.Tnx & pf.Tx;
                pf.Class_Id = this.Spec.Class_id;
                TestDetailPf = pf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存一条测试数据到数据库
        /// </summary>
        /// <param name="cmd">数据库实例化对象</param>
        /// <param name="commonRowId">基础数据（Common）上传后，在数据库的ID号</param>
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
                    ParameterName = "Ax_n",
                    DbType = DbType.Double,
                    Value = this.Ax_n
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ax_p",
                    DbType = DbType.Double,
                    Value = this.Ax_p
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Ax",
                    DbType = DbType.Double,
                    Value = this.Ax
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Nx",
                    DbType = DbType.Double,
                    Value = this.Nx
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tx",
                    DbType = DbType.Double,
                    Value = this.Tx
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tnx",
                    DbType = DbType.Double,
                    Value = this.Tax
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tax",
                    DbType = DbType.Double,
                    Value = this.Tax
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Double
                }).Direction = ParameterDirection.Output;

                cmd.CommandText = "dbo.insert_test_ret_crosstalk";
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
