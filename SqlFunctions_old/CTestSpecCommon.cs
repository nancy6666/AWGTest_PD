using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SqlFunctions
{
    public class CTestSpecCommon
    {
        #region Property
       public  string strLastError = string.Empty;
        public List<CLossSpec> lstLossSpec = new List<CLossSpec>();
        public List<CPdlSpec> lstPdlSpec = new List<CPdlSpec>();
        public List<CFrequencySpec> lstFrequencySpec = new List<CFrequencySpec>();
        public List<CPassbandSpec> lstPassbandSpec = new List<CPassbandSpec>();
        public List<CWavelengthSpec> lstWavelengthSpec = new List<CWavelengthSpec>();
        public List<CCrosstalkSpec> lstCrosstalkSpec = new List<CCrosstalkSpec>();
        
        /// <summary>
        /// 测试结果的计算条件，按客户（Spec Class）分类
        /// </summary>
        public List<CCond> lstCondSpec = new List<CCond>();
        public List<int> lstSpecClassId = new List<int>();

     
        public int MaxChannel
        {
            set;
            get;
        }
        public int SpecID
        {
            set;
            get;
        }
        public int Version
        {
            set;
            get;
        }
        public int Category_id
        {
            set;
            get;
        }
        public string Temperature
        {
            set;
            get;
        }
        public float Sweep_start
        {
            set;
            get;
        }
        public float Sweep_end
        {
            set;
            get;
        }
        public float Sweep_step
        {
            set;
            get;
        }
        public float Laser_output_pwr
        {
            set;
            get;
        }
        public float Predicted_temp_min
        {
            set;
            get;
        }
        public float Predicted_temp_max
        {
            set;
            get;
        }
        public float Uniformity_min
        {
            set;
            get;
        }
        public float Uniformity_max
        {
            set;
            get;
        }
        public int Created_by
        {
            set;
            get;
        }
       
        public string Comment { get; set; }
        public double ITU_Start { set; get; }
       
        public double ITU_Step { set; get; }
        public bool Llog { set; get; }
        public string CategoryName { set; get; }

        #endregion

        #region Constructors

        public CTestSpecCommon(SqlCommand cmd,string pn,string temp)
        {
          //GetMaxChannel(cmd, pn, temp);          
            GetSpec(cmd, pn, temp);

        }
        #endregion

        #region Public Method

        ///// <summary>
        ///// 获取当前产品的最大通道数
        ///// </summary>
        ///// <param name="cmd"></param>
        ///// <param name="pn">PN号</param>
        ///// <param name="temp">温度</param>
        //public void GetMaxChannel(SqlCommand cmd,string pn, string temp)
        //{
        //    cmd.Parameters.Clear();
        //    try
        //    {
        //            cmd.Parameters.Add(new SqlParameter()
        //            {
        //                ParameterName = "Temp",
        //                DbType = DbType.String,
        //                Value = temp
        //            });
        //            cmd.Parameters.Add(new SqlParameter()
        //            {
        //                ParameterName = "Pn",
        //                DbType = DbType.String,
        //                Value = pn
        //            });
        //            cmd.Parameters.Add(new SqlParameter()
        //            {
        //                ParameterName = "MaxChannel",
        //                DbType = DbType.Int32
        //            }).Direction = ParameterDirection.Output;

        //            cmd.CommandText = "dbo.select_maxchannel";
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.ExecuteNonQuery();
        //            this.MaxChannel = (int)cmd.Parameters["MaxChannel"].Value;                   
        //        }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }          
           
        //}

        /// <summary>
        /// 获取当前产品的所有Spec
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="pn">PN号</param>
        /// <param name="temp">温度</param>
        /// <returns></returns>
        public bool GetSpec(SqlCommand cmd, string pn, string temp)
        {
            bool ret = false;

            #region Get Common Spec & Product Category

            cmd.Parameters.Clear();
            try
            {
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Temp",
                    DbType = DbType.String,
                    Value = temp
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pn",
                    DbType = DbType.String,
                    Value = pn
                });

                cmd.CommandText = "dbo.select_common_spec";
                cmd.CommandType = CommandType.StoredProcedure;             
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];

                this.SpecID = (int)dt.Rows[0]["id"];
                this.Category_id = (int)dt.Rows[0]["category_id"];
                this.Temperature = dt.Rows[0]["temperature"].ToString();
               
                this.Sweep_start = (float)Convert.ToDouble(dt.Rows[0]["sweep_start"]);
                this.Sweep_end = (float)Convert.ToDouble(dt.Rows[0]["sweep_end"]);
                this.Sweep_step = (float)Convert.ToDouble(dt.Rows[0]["sweep_step"]);
                this.Laser_output_pwr = (float)Convert.ToDouble(dt.Rows[0]["laser_output_pwr"]);
                this.Predicted_temp_max = (float)Convert.ToDouble(dt.Rows[0]["predicted_temp_max"]);
                this.Predicted_temp_min = (float)Convert.ToDouble(dt.Rows[0]["predicted_temp_min"]);
                this.Uniformity_max = (float)Convert.ToDouble(dt.Rows[0]["uniformity_max"]);
                this.Uniformity_min = (float)Convert.ToDouble(dt.Rows[0]["uniformity_min"]);
                this.MaxChannel = (int)dt.Rows[0]["max_channel"];
                this.ITU_Start = Convert.ToDouble(dt.Rows[0]["itu_start"]);
                this.ITU_Step = Convert.ToDouble(dt.Rows[0]["itu_step"]);
               
                this.Llog = (dt.Rows[0]["llog"]).ToString() == "1" ? true:false ;
                this.CategoryName = dt.Rows[0]["category_name"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion

//            #region Get Product Categoty

//            productCategory.ReadTable(cmd, this.Category_id);
//            this.MaxChannel = productCategory.Max_Channel;
//#endregion

            #region Get Common Condition Spec

            CCond condspec = new CCond();
            condspec.GetSpec(cmd, this.SpecID, out lstCondSpec);
            #endregion

            #region Get Test Detail Spec

            CCrosstalkSpec crosstalkSpec = new CCrosstalkSpec();
            crosstalkSpec.GetSpec(cmd, this.SpecID, out lstCrosstalkSpec);

            CFrequencySpec frequencySpec = new CFrequencySpec();
            frequencySpec.GetSpec(cmd, this.SpecID, out lstFrequencySpec);

            CLossSpec lossSpec = new CLossSpec();
            lossSpec.GetSpec(cmd, this.SpecID, out lstLossSpec);

            CPassbandSpec passbandSpec = new CPassbandSpec();
            passbandSpec.GetSpec(cmd, this.SpecID, out lstPassbandSpec);

            CWavelengthSpec wavelengthSpec = new CWavelengthSpec();
            wavelengthSpec.GetSpec(cmd, this.SpecID, out lstWavelengthSpec);

            CPdlSpec pdlSpec = new CPdlSpec();
            pdlSpec.GetSpec(cmd, this.SpecID, out lstPdlSpec);

            #endregion

            if (lstCrosstalkSpec.Count == lstLossSpec.Count && lstCrosstalkSpec.Count == lstPassbandSpec.Count && lstCrosstalkSpec.Count == lstPdlSpec.Count && lstCrosstalkSpec.Count == lstWavelengthSpec.Count && lstCrosstalkSpec.Count == lstFrequencySpec.Count)
            {
                if (crosstalkSpec.lstClassId.Except(frequencySpec.lstClassId).ToList().Count == 0)
                {
                    if (crosstalkSpec.lstClassId.Except(lossSpec.lstClassId).ToList().Count == 0)
                    {
                        if (crosstalkSpec.lstClassId.Except(passbandSpec.lstClassId).ToList().Count == 0)
                        {
                            if (crosstalkSpec.lstClassId.Except(wavelengthSpec.lstClassId).ToList().Count == 0)
                            {
                                if (crosstalkSpec.lstClassId.Except(pdlSpec.lstClassId).ToList().Count == 0)
                                {
                                    if (crosstalkSpec.lstClassId.Except(condspec.lstClassId).ToList().Count == 0)
                                    {
                                        ret = true;
                                        this.lstSpecClassId = crosstalkSpec.lstClassId;
                                    }
                                    else
                                    {
                                        ret = false;
                                        throw new Exception("the class_id should be the same in 7 spec tables!");
                                    }
                                }
                                else
                                {
                                    ret = false;
                                    throw new Exception("the class_id should be the same in 7 spec tables!");
                                }
                            }
                            else
                            {
                                ret = false;
                                throw new Exception("the class_id should be the same in 7 spec tables!");
                            }
                        }
                        else
                        {
                            ret = false;
                            throw new Exception("the class_id should be the same in 7 spec tables!");
                        }
                    }
                    else
                    {
                        ret = false;
                        throw new Exception("the class_id should be the same in 7 spec tables!");
                    }
                }
                else
                {
                    ret = false;
                    throw new Exception("the class_id should be the same in 7 spec tables!");
                }
                return ret;
            }
            else
            {
                throw new Exception("error!测试参数spec的行数不同!");
            }
        }

        /// <summary>
        /// 获取某一个客户编号及某一个通道下的测试数据的spec
        /// </summary>
        /// <param name="SpecClassId">客户编号</param>
        /// <param name="Channel">通道</param>
        /// <returns></returns>
        public CLossSpec GetLossSpecBySpecClassAndChannel(int SpecClassId, int Channel)
        {
            var list = lstLossSpec.Where(a => a.Class_id == SpecClassId && a.Channel == Channel);
            if(list.Count() > 1)
            {
                throw new Exception("There are more than 1 loss spec.");
            }
            else
            {
                return list.First();
            }
        }

        /// <summary>
        /// 获取某一个客户编号及某一个通道下的测试数据的spec
        /// </summary>
        /// <param name="SpecClassId">客户编号</param>
        /// <param name="Channel">通道</param>
        /// <returns></returns>
        public CCrosstalkSpec GetCrosstalkSpecBySpecClassAndChannel(int SpecClassId, int Channel)
        {
            var list = lstCrosstalkSpec.Where(a => a.Class_id == SpecClassId && a.Channel == Channel);
            if (list.Count() > 1)
            {
                throw new Exception("There are more than 1 crosstalk spec.");
            }
            else
            {
                return list.First();
            }
        }

        /// <summary>
        /// 获取某一个客户编号及某一个通道下的测试数据的spec
        /// </summary>
        /// <param name="SpecClassId">客户编号</param>
        /// <param name="Channel">通道</param>
        /// <returns></returns>
        public CPassbandSpec GetPassbandSpecBySpecClassAndChannel(int SpecClassId, int Channel)
        {
            var list = lstPassbandSpec.Where(a => a.Class_id == SpecClassId && a.Channel == Channel);
            if (list.Count() > 1)
            {
                throw new Exception("There are more than 1 Passband spec.");
            }
            else
            {
                return list.First();
            }
        }

        /// <summary>
        /// 获取某一个客户编号及某一个通道下的测试数据的spec
        /// </summary>
        /// <param name="SpecClassId">客户编号</param>
        /// <param name="Channel">通道</param>
        /// <returns></returns>
        public CPdlSpec GetPdlSpecBySpecClassAndChannel(int SpecClassId, int Channel)
        {
            var list = lstPdlSpec.Where(a => a.Class_id == SpecClassId && a.Channel == Channel);
            if (list.Count() > 1)
            {
                throw new Exception("There are more than 1 pdl spec.");
            }
            else
            {
                return list.First();
            }
        }

        /// <summary>
        /// 获取某一个客户编号及某一个通道下的测试数据的spec
        /// </summary>
        /// <param name="SpecClassId">客户编号</param>
        /// <param name="Channel">通道</param>
        /// <returns></returns>
        public CWavelengthSpec GetWavelengthSpecBySpecClassAndChannel(int SpecClassId, int Channel)
        {
            var list = lstWavelengthSpec.Where(a => a.Class_id == SpecClassId && a.Channel == Channel);
            if (list.Count() > 1)
            {
                throw new Exception("There are more than 1 wavelength spec.");
            }
            else
            {
                return list.First();
            }
        }

        /// <summary>
        /// 获取某一个客户编号及某一个通道下的测试数据的spec
        /// </summary>
        /// <param name="SpecClassId">客户编号</param>
        /// <param name="Channel">通道</param>
        /// <returns></returns>
        public CFrequencySpec GetFrequencySpecBySpecClassAndChannel(int SpecClassId, int Channel)
        {
            var list = lstFrequencySpec.Where(a => a.Class_id == SpecClassId && a.Channel == Channel);
            if (list.Count() > 1)
            {
                throw new Exception("There are more than 1 frequency spec.");
            }
            else
            {
                return list.First();
            }
        }
        #endregion

    } 
}
