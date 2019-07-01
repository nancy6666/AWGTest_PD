using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CWavelength : ITestDetail
    {
        #region Property

       public CWavelengthSpec Spec = new CWavelengthSpec();
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
        public double Wavelength
        {
            set;
            get;
        }
        public double Offset
        {
            set;
            get;
        }
        public double Pdw
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
                CWavelengthPf pf = new CWavelengthPf();

                pf.Ret_id = this.RawID;

                //if (this.Wavelength > this.Spec.Wavelength_min & this.Wavelength < this.Spec.Wavelength_max)
                //{
                //    pf.Wavelength = true;
                //}
                //else
                //{
                //    pf.Wavelength = false;
                //}
                if (this.Offset >= this.Spec.Offset_min & this.Offset <= this.Spec.Offset_max)
                {
                    pf.Offset = true;
                }
                else
                {
                    pf.Offset = false;
                }
                if (this.Pdw >= this.Spec.Pdw_min & this.Pdw <= this.Spec.Pdw_max)
                {
                    pf.Pdw = true;
                }
                else
                {
                    pf.Pdw = false;
                }

                pf.PfRet = pf.Offset & pf.Pdw ;
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
                    ParameterName = "Pdw",
                    DbType = DbType.Double,
                    Value = this.Pdw
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Wavelength",
                    DbType = DbType.Double,
                    Value = this.Wavelength
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Double
                }).Direction = ParameterDirection.Output;

                cmd.CommandText = "dbo.insert_test_ret_wavelength";
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
