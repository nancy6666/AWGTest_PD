using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace SqlFunctions
{
    public class CPassband : ITestDetail
    {
        #region Property

        public CPassbandSpec Spec = new CPassbandSpec();
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
        public double At_05db
        {
            set;
            get;
        }
        public double At_1db
        {
            set;
            get;
        }
        public double At_3db
        {
            set;
            get;
        }
        public double At_20db
        {
            set;
            get;
        }
        public double At_25db
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

                CPassbandPf pf = new CPassbandPf();

                pf.Ret_id = this.RawID;

                if (this.At_05db >= this.Spec.At_05db_min & this.At_05db <= this.Spec.At_05db_max)
                {
                    pf.At_05db = true;
                }
                else
                {
                    pf.At_05db = false;
                }
                if (this.At_1db >= this.Spec.At_1db_min & this.At_1db <= this.Spec.At_1db_max)
                {
                    pf.At_1db = true;
                }
                else
                {
                    pf.At_1db = false;
                }
                if (this.At_3db >= this.Spec.At_3db_min & this.At_3db <= this.Spec.At_3db_max)
                {
                    pf.At_3db = true;
                }
                else
                {
                    pf.At_3db = false;
                }
                if (this.At_20db >= this.Spec.At_20db_min & this.At_20db <= this.Spec.At_20db_max)
                {
                    pf.At_20db = true;
                }
                else
                {
                    pf.At_20db = false;
                }
                if (this.At_25db >= this.Spec.At_25db_min & this.At_25db <= this.Spec.At_25db_max)
                {
                    pf.At_25db = true;
                }
                else
                {
                    pf.At_25db = false;
                }

                pf.PfRet = pf.At_05db & pf.At_1db & pf.At_20db & pf.At_25db & pf.At_3db;
                pf.Class_Id = this.Spec.Class_id;
                testDetailPf = pf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveTestDetail(SqlCommand cmd, int commonrowid)
        {
            try
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Test_id",
                    DbType = DbType.Int32,
                    Value = commonrowid

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
                    ParameterName = "At_05db",
                    DbType = DbType.Double,
                    Value = this.At_05db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_1db",
                    DbType = DbType.Double,
                    Value = this.At_1db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_3db",
                    DbType = DbType.Double,
                    Value = this.At_3db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_20db",
                    DbType = DbType.Double,
                    Value = this.At_20db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "At_25db",
                    DbType = DbType.Double,
                    Value = this.At_25db
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "ID",
                    DbType = DbType.Double
                }).Direction = ParameterDirection.Output;

                cmd.CommandText = "dbo.insert_test_ret_passband";
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
