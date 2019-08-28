using AWGTestServer.Instruments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestSystem.TestLibrary.INI;
using PM1906AHelper;

namespace AWGTestServer
{
    class Inst_Ag_ILPDL_PD
    {
        #region Properties

        private StringBuilder logging = new StringBuilder();
        private StringBuilder data = new StringBuilder();
        ConfigurationManagement cfg;
        private Frm_AWGTestServer frmAWGTest;

        private K8164B k8164;
        private N7786B n7786;
        public const int PolarizerCount = 4;
        public int[] m_dblSamplePoint_OneCircle = new int[8];

        public double[] m_dblAWGStartWavelength = new double[8];
        public double[] m_dblAWGStopWavelength = new double[8];
        public double[] m_dblAWGStepWavelength = new double[8];
        public double m_dblAWGSweepRate = 10;

        public double[] m_dblAWGTLSPower = new double[8];
        public int[] m_bAWGTLSOutoputPort = new int[8];
        public int[] m_bAWGLambdaLog = new int[8];
        public bool[] m_bAWGRefDone = new bool[8];

        public string[] m_strAWGRefDescription = new string[8];
        public long[] m_lAWGRefFinishedEvent = new long[8];
        public long[] m_lAWGMeasureFinishedEvent = new long[8];
        private double[] Result = new double[8];
        #endregion

        double[] g_dblTemperature = new double[8];
        public Inst_Ag_ILPDL_PD(Frm_AWGTestServer frmawg)
        {
            frmAWGTest = frmawg;
        }
        public void InitI()//done
        {
            try
            {
                cfg = new ConfigurationManagement();
                k8164 = new K8164B(Convert.ToInt16(cfg.K8164BGPIB));//need to set the GPIB address
                n7786 = new N7786B(Convert.ToInt16(cfg.N7786BGPIB));//need to set the GPIB address

                //设置光源K8164B
                k8164.SetSweepMode(K8164B.SweepMode.CONT);
                k8164.SetTriggerMode(K8164B.TriggerMode.STF);
                k8164.SetInputTrigIgn();
             
                k8164.SetOutputActive(true);
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool DoReference(string sStationSettingFilePath, int bWSIndex)
        {
            bool bSuccess = true;
            try
            {
                SystemCalibration(bWSIndex);//StartReference

                SetTLSetting(sStationSettingFilePath, bWSIndex);
            }

            catch (Exception e)
            {
                frmAWGTest.ShowMsg(e.ToString(), true);
                bSuccess = false;
            }

            return bSuccess;
        }
        public void GetTLSetting(string sStationSettingFilePath, int bWSIndex)//done
        {
            try
            {
                m_dblAWGStartWavelength[bWSIndex] = double.Parse(INIOperationClass.INIGetStringValue(sStationSettingFilePath, "TLS Setting", "Start WL", ""));
                m_dblAWGStopWavelength[bWSIndex] = double.Parse(INIOperationClass.INIGetStringValue(sStationSettingFilePath, "TLS Setting", "Stop WL", ""));
                m_dblAWGStepWavelength[bWSIndex] = double.Parse(INIOperationClass.INIGetStringValue(sStationSettingFilePath, "TLS Setting", "Step Size", ""));
                //     m_dblAWGSweepRate[bWSIndex] = int.Parse(INIOperationClass.INIGetStringValue(sStationSettingFilePath, "TLS Setting", "Step Size", ""));

                m_dblAWGTLSPower[bWSIndex] = double.Parse(INIOperationClass.INIGetStringValue(sStationSettingFilePath, "TLS Setting", "Output Power", ""));
                m_bAWGLambdaLog[bWSIndex] = int.Parse(INIOperationClass.INIGetStringValue(sStationSettingFilePath, "TLS Setting", "Lambda Mode", ""));
                //采样点数为一个circle的点数*4（4个偏正态）
                m_dblSamplePoint_OneCircle[bWSIndex] = Convert.ToInt32((m_dblAWGStopWavelength[bWSIndex] - m_dblAWGStartWavelength[bWSIndex]) / m_dblAWGStepWavelength[bWSIndex] + 1);
            }
            catch
            {
                throw new Exception("Read TLS Config file Error !!!");
            }
        }
        private void SetTLSetting(string sStationSettingFilePath, int bWSIndex)
        {
            try
            {
                INIOperationClass.INIWriteValue(sStationSettingFilePath, "TLS Setting", "Start WL", m_dblAWGStartWavelength[bWSIndex].ToString());
                INIOperationClass.INIWriteValue(sStationSettingFilePath, "TLS Setting", "Stop WL", m_dblAWGStopWavelength[bWSIndex].ToString());
                //INIOperationClass.INIWriteValue(sStationSettingFilePath, "TLS Setting", "Channel Count", MaxChannel.ToString());
                INIOperationClass.INIWriteValue(sStationSettingFilePath, "TLS Setting", "Step Size", m_dblAWGStepWavelength[bWSIndex].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"保存配置文件到{sStationSettingFilePath}出错！{ex.Message}");
            }
        }
        /// <summary>
        /// 获取未放置产品时，4个偏振态下的光功率数组
        /// </summary>
        /// <param name="bWSIndex"></param>
        public void SystemCalibration(int bWSIndex)
        {
            try
            {
                StartReference(bWSIndex);
            }
            catch
            {
                throw new Exception("系统校准操作错误 !!!");
            }
        }

        public bool DoTest(int bWSIndex, ref string ErrorMsg)
        {
            bool bSuccess = false;
            try
            {
                StartMeasurement(bWSIndex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bSuccess;
        }

      
        public bool GetGraphWavelength(ref double[] pdwWLData, int bWSIndex)
        {
            int point;
            for (point = 0x00; point < m_dblSamplePoint_OneCircle[bWSIndex]; point++)
            {
                pdwWLData[point] = m_dblAWGStartWavelength[bWSIndex] + point * m_dblAWGStepWavelength[bWSIndex];
            }
            return true;
        }
        private void StartMeasurement(int bWSIndex)
        {
      
            //四个偏振态

            //启动光源扫描(4次)

            List<double[]> lstSopArray = new List<double[]>();
            double[] sop1 = { 1, 1, 0, 0 };
            double[] sop2 = { 1, -1, 0, 0 };
            double[] sop3 = { 1, 0, 1, 0 };
            double[] sop4 = { 1, 0, 0, 1 };
            lstSopArray.Add(sop1);
            lstSopArray.Add(sop2);
            lstSopArray.Add(sop3);
            lstSopArray.Add(sop4);
            try
            {
                //      System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                //         {
                for (int i = 0; i < PolarizerCount; i++)
                {

                    n7786.SetSOP(lstSopArray[i]);
                    // 开始扫描
                    if (!StartSweep())
                    {
                        throw new Exception("扫描失败");
                    }
                    // 正在扫描
                    while (CheckIsSweeping())
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"启动扫描循环出错，{ex.Message}");
            }
        }

      //  public bool IsLogging { set; get; } = false;

        /// <summary>
        /// 开始扫描
        /// </summary>
        /// <returns></returns>
        public bool StartSweep()
        {
            data.Clear();
            try
            {
                //光源启动扫描
                k8164.SetSweepState(K8164B.SweepState.STAR);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception($"启动扫描出错，{ex.Message}");
            }
        }

        private const int channelCount = 1;
       
        private void StartReference(int bWSIndex)
        {
            try
            {
                StartMeasurement(bWSIndex);
            }
            catch(Exception ex)
            {
                frmAWGTest.ShowMsg(ex.ToString(), true);
                throw new Exception($"校准出错，{ex.Message}");
            }
        }

        /// <summary>
        /// 检查是否正在扫描
        /// </summary>
        /// <returns>true:正在扫描；false:扫描终止</returns>
        public bool CheckIsSweeping()
        {
          var  data= k8164.ReadSweepStatus();

            if (data.IndexOf("+1") > -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置仪器参数
        /// </summary>
        /// <param name="bWSIndex">机台号</param>
        public void SetDevicesParameters(int bWSIndex)
        {
            try
            {
                //设置光源扫描设置
                k8164.SetOutputPower(this.m_dblAWGTLSPower[bWSIndex], K8164B.PowerUnit.DBM);
                k8164.SetSweepRep();
                k8164.SetSweepCycle(1);
                k8164.SetStartWave(m_dblAWGStartWavelength[bWSIndex], K8164B.WaveUnit.NM);
                k8164.SetStopWave(m_dblAWGStopWavelength[bWSIndex], K8164B.WaveUnit.NM);
                k8164.SetSweepSpeed(m_dblAWGSweepRate);
                k8164.SetSweepStep(m_dblAWGStepWavelength[bWSIndex]);

                var err = k8164.GetConfigurationErr();
                if (!err.Contains("OK"))
                {
                    throw new Exception($"Tunable laser setting error,{err}");
                }

                k8164.SetOutputActive(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"设备设置出错，{ex.Message}");
            }
        }
    }
}
