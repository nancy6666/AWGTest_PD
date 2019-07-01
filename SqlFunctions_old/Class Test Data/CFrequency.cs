using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CFrequency : ITestDetail
    {
        #region Property

       public CFrequencySpec Spec = new CFrequencySpec();
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
        public float Freq
        {
            set;
            get;
        }
        public float Offset
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

        public void GetPfRet( out ITestDetailPf testDetailPf)
        {
            testDetailPf = null;
            try
            {
                CFrequencyPf pf = new CFrequencyPf();

                pf.Ret_id = this.RawID;

                if (this.Freq > this.Spec.Freq_min & this.Freq < this.Spec.Freq_max)
                {
                    pf.Freq = true;
                }
                else
                {
                    pf.Freq = false;
                }
                if (this.Offset > this.Spec.Offset_min & this.Freq < this.Spec.Offset_max)
                {
                    pf.Offset = true;
                }
                else
                {
                    pf.Offset = false;
                }

                pf.PfRet = pf.Freq & pf.Offset;
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
                    ParameterName = "Offset",
                    DbType = DbType.Double,
                    Value = this.Offset
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Freq",
                    DbType = DbType.Double,
                    Value = this.Freq
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Double
                }).Direction = ParameterDirection.Output;

                cmd.CommandText = "dbo.insert_test_ret_frequency";
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
