using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SqlFunctions.Class_Test_Data_Pf;
using SqlFunctions.Class_Test_Data;

namespace SqlFunctions
{
    public class CTestDataCommon
    {

        #region Property
     
        public List<ITestDetail> lstTestDetail = new List<ITestDetail>();
        public List<CUniformity> lstUniformity = new List<CUniformity>();
      
        public List<ITestDetailPf> lstTestDetailPf = new List<ITestDetailPf>();
        public List<CUniformityPf> lstUniformityPf = new List<CUniformityPf>();

        public List<CFinalPf> lstFinalPf = new List<CFinalPf>();

        public List<CReference> lstReference = new List<CReference>();

        public CBaseInfo baseInfo = new CBaseInfo();
        public CRawData rawData = new CRawData();

        string strLastError = string.Empty;

        #region Common Property

        public int MaxChannel { get; set; }
        public int CommonRowID { get; set; }
        public int Baseinfo_id
        {
            set;
            get;
        }
        public int Spec_common_id
        {
            set;
            get;
        }
        
        public string Temperature
        {
             set;
            get;
        }
        public double Predicted_temp
        {
             set;
            get;
        }
        public int Input_channel
        {
             set;
            get;
        }
        public double Sys_loss
        {
             set;
            get;
        }
        public double Uniformity
        {
             set;
            get;
        }
      
        public int Tested_by
        {
             set;
            get;
        }
        public double Test_costs
        {
             set;
            get;
        }
        public string Station
        {
             set;
            get;
        }
        public string Comment
        {
             set;
            get;
        }
        public int Trimming_cnt
        {
             set;
            get;
        }
        public int Last_trimming
        {
             set;
            get;
        }

        public bool Pf
        {
            get;
            set;
        }
       
        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        ///获取不同客户，不同测试类下的测试数据判断结果的集合
        /// </summary>
        /// <param name="lstTestDetail">不同测试类，不同channel下的所有测试数据集</param>
        /// <param name="lstPf">不同客户，不同测试类下的测试数据判断结果的集合</param>
        public void GetDetailPf(List<ITestDetail> lstTestDetail, out List<ITestDetailPf> lstPf)
        {
            lstPf = new List<ITestDetailPf>();
            ITestDetailPf pf;
            foreach (var item in lstTestDetail)
            {
                item.GetPfRet(out pf);
                lstPf.Add(pf);
            }
        }
        /// <summary>
        /// 获取不同客户（class_id）情况下，所有测试数据的一个总的判断结果（pf）
        /// </summary>
        /// <param name="lstClassId">客户编号集合</param>
        /// <param name="lstPf">不同客户，不同测试表格下的测试数据判断结果的集合</param>
        /// <param name="lstFinalPf">不同客户（class_id）情况下，所有测试数据的一个总的判断结果的集合</param>
        public void GetFinalPfPerClass(List<int> lstClassId, List<ITestDetailPf> lstPf, List<CUniformityPf> lstUniformityPf,out List<CFinalPf> lstFinalPf)
        {
            lstFinalPf = new List<CFinalPf>();
            bool rt;
            foreach(var id in lstClassId)
            {
                rt = true;
                CFinalPf pf = new CFinalPf();
                foreach (var item in lstPf)
                {                    
                   if(id==item.Class_Id)
                    rt = rt & item.PfRet;
                }
                foreach(var uniformityPf in lstUniformityPf)
                {
                    if(id== uniformityPf.Spec_class_id)
                    {
                        rt = rt & uniformityPf.Uniformity & uniformityPf.Predicted_temp;
                    }
                }
                pf.Spec_class_id = id;
                pf.Pf = rt;          
                lstFinalPf.Add(pf);
            }
            
            
        }
        /// <summary>
        /// 保存基础数据、测试数据、RawData、测试数据的判断结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="lstClassId">客户编号集合</param>
        public void SaveAllData(SqlCommand cmd, CTestSpecCommon specCommon)
        {
            try
            {
                List<int> lstClassId = specCommon.lstSpecClassId;

                #region Get Uniformity Pf Result

                GetUniformityPf(lstUniformity, out lstUniformityPf);
                #endregion

                #region Get Test Data Pf Result

                GetDetailPf(lstTestDetail, out lstTestDetailPf);
                #endregion

                #region Get Final Pf for Per class

                GetFinalPfPerClass(lstClassId, lstTestDetailPf, lstUniformityPf, out lstFinalPf);
                #endregion

                #region Get Final Pf 
                ///one spec class pass, the pf is true
                GetFinalPf(lstFinalPf);
                #endregion

                #region Save Common

                cmd.Parameters.Clear();

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Baseinfo_id",
                    DbType = DbType.Int32,
                    Value = baseInfo.RowID
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Spec_common_id",
                    DbType = DbType.Int32,
                    Value = specCommon.SpecID
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Temperature",
                    DbType = DbType.String,
                    Value = this.Temperature
                });

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Input_channel",
                    DbType = DbType.Int32,
                    Value = this.Input_channel
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "System_loss",
                    DbType = DbType.Double,
                    Value = this.Sys_loss
                });


                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Tested_by",
                    DbType = DbType.Int32,
                    Value = this.Tested_by
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Test_costs",
                    DbType = DbType.Double,
                    Value = this.Test_costs
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Station",
                    DbType = DbType.String,
                    Value = this.Station
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Comment",
                    DbType = DbType.String,
                    Value = this.Comment
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pf",
                    DbType = DbType.Boolean,
                    Value = this.Pf
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Identity",
                    DbType = DbType.Int32
                }).Direction = ParameterDirection.Output;
                cmd.CommandText = "dbo.insert_test_ret_common_2";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                this.CommonRowID = (int)cmd.Parameters["Identity"].Value;
                #endregion

                #region Save All Test Detail Data
                //Uniformity
                foreach (var item in lstUniformity)
                {
                    item.SaveUniformity(cmd, CommonRowID);
                }
                //lsttestdetail里的子项数目为 MaxChannel*6*class_id
                foreach (var item in lstTestDetail)
                {
                    item.SaveTestDetail(cmd, CommonRowID);
                }
                #endregion

                #region Save Test Data Pf Result   
                for (int i = 0; i < lstTestDetailPf.Count; i++)
                {
                    lstTestDetailPf[i].Ret_id = lstTestDetail[i].RawID;
                    lstTestDetailPf[i].SaveTestDetailPf(cmd);
                }
                //foreach (var item in lstTestDetailPf)
                //{
                //    item.SaveTestDetailPf(cmd);
                //}
                #endregion

                #region Save uniformity Pf & Final pf for per class
                for (int i = 0; i < lstUniformityPf.Count; i++)
                {
                    lstUniformityPf[i].Uniformity_per_class_id = lstUniformity[i].RowID;
                    lstUniformityPf[i].SaveUniformityPf(cmd, this.CommonRowID);
                }

                foreach (var item in lstFinalPf)
                {
                    item.SaveFinalPf(cmd, this.CommonRowID);
                }

                #endregion

                #region Save RawData

                rawData.Save(cmd, this.CommonRowID);

                #endregion

                #region Save Reference

                foreach (var item in lstReference)
                {
                    item.Save(cmd, this.CommonRowID);
                }
                #endregion

                cmd.Transaction.Commit();

            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                throw ex;
            }

        }

/// <summary>
/// Get final pf for all spec class,one class pass,then pf is true
/// </summary>
/// <param name="lstFinalPf"></param>
/// <param name="lstCommonPf"></param>
        private void GetFinalPf(List<CFinalPf> lstFinalPf)
        {
            bool ret = false;
            for (int i = 0; i < lstFinalPf.Count; i++)
            {
                ret = ret ||  lstFinalPf[i].Pf;
            }
         
            this.Pf = ret;
        }

        public void GetUniformityPf(List<CUniformity> lstUniformity, out List<CUniformityPf> lstUniformityPf)
        {
            lstUniformityPf = new List<CUniformityPf>();
            foreach (var uniformity in lstUniformity)
            {
                CUniformityPf pf = new CUniformityPf();
                uniformity.GetUniformityPf(out pf);
                lstUniformityPf.Add(pf);
            }
        }
        #endregion

    }
}












