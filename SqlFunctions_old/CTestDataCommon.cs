using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SqlFunctions
{
    public class CTestDataCommon
    {

        #region Property
     
        public List<ITestDetail> lstTestDetail = new List<ITestDetail>();
      
        public List<ITestDetailPf> lstTestDetailPf = new List<ITestDetailPf>();

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
        public string Temperature
        {
             set;
            get;
        }
        public float Predicted_temp
        {
             set;
            get;
        }
        public int Input_channel
        {
             set;
            get;
        }
        public float Sys_loss
        {
             set;
            get;
        }
        public float Uniformity
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
        /// <param name="lstPf">不同客户，不同测试类下的测试数据判断结果的集合</param>
        /// <param name="lstFinalPf">不同客户（class_id）情况下，所有测试数据的一个总的判断结果的集合</param>
        public void GetFinalPf(List<int> lstClassId, List<ITestDetailPf> lstPf, out List<CFinalPf> lstFinalPf)
        {
            lstFinalPf = new List<CFinalPf>();
            bool rt = true;
            foreach(var id in lstClassId)
            {
                CFinalPf pf = new CFinalPf();
                foreach (var item in lstPf)
                {
                   if(id==item.Class_Id)
                    rt = rt & item.PfRet;
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
        public void SaveAllData(SqlCommand cmd, List<int> lstClassId)
        {
            try
            {
                //#region Save Baseinfo
                //baseInfo.Save(cmd);
                //#endregion

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
                    ParameterName = "Temperature",
                    DbType = DbType.String,
                    Value = this.Temperature
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Predicted_temperature",
                    DbType = DbType.Double,
                    Value = this.Predicted_temp
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
                    ParameterName = "Uniformity",
                    DbType = DbType.Double,
                    Value = this.Uniformity
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
                //cmd.Parameters.Add(new SqlParameter()
                //{
                //    ParameterName = "Trimming_cnt",
                //    DbType = DbType.Int32,
                //    Value = this.Trimming_cnt
                //});
                //cmd.Parameters.Add(new SqlParameter()
                //{
                //    ParameterName = "Last_trimming",
                //    DbType = DbType.Int32,
                //    Value = this.Last_trimming
                //});
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Identity",
                    DbType = DbType.Int32
                }).Direction = ParameterDirection.Output;
                cmd.CommandText = "dbo.insert_test_ret_common";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                this.CommonRowID = (int)cmd.Parameters["Identity"].Value;
                #endregion

                #region Save All Test Detail Data
                //lsttestdetail里的子项数目为 MaxChannel*6*class_id
                foreach (var item in lstTestDetail)
                {
                    item.SaveTestDetail(cmd, CommonRowID);
                }
                #endregion

                #region Get Test Data Pf Result

                GetDetailPf(lstTestDetail, out lstTestDetailPf);
                #endregion

                #region Save Test Data Pf Result

                foreach (var item in lstTestDetailPf)
                {
                    item.SaveTestDetailPf(cmd);
                }
                #endregion

                #region Get Final Pf

                GetFinalPf(lstClassId, lstTestDetailPf, out lstFinalPf);
                #endregion

                #region Save Final Pf 
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
        #endregion

    }
}












