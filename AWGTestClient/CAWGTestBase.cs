using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWGTestClient.Instruments;
using TestSystem.TestLibrary.Utilities;

namespace AWGTestClient
{
    class CAWGTestBase : IAWGTest
    {
        public CAWGTestBase ()
        {
            lstPowermeterComs = new List<string>();
            lstPowermeterComs.Add(cfg.PM1906Com1);
            //等功率计out trigger ok以后再enable
            //lstPowermeterComs.Add(cfg.PM1906Com2);
            //lstPowermeterComs.Add(cfg.PM1906Com3);
            //lstPowermeterComs.Add(cfg.PM1906Com4);

            if (cfg.PowerMeterType.Contains("PM1906A"))
            {
                foreach (var com in lstPowermeterComs)
                {
                    PowerMeter = new MyPM1906A(com, cfg.PM1906Rate);
                    lstPowermeter.Add(PowerMeter);
                }
            }
        }

        public void OpenPowermeter()
        {
            try
            {
                foreach (var powermeter in lstPowermeter)
                {
                    powermeter.Open();
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"打开串口失败{ex.Message}");
            }
        }

        public void ClosePowermeter()
        {
            try
            {
                foreach (var powermeter in lstPowermeter)
                {
                    powermeter.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"关闭串口失败{ex.Message}");
            }
        }
        ConfigurationInstruments cfg = new ConfigurationInstruments();
        public List<IPowermeter> lstPowermeter = new List<IPowermeter>();
        public List<string> lstPowermeterComs;
       
        public IPowermeter PowerMeter;
      
        public List<double[]> lstCaliResult;

        public List<double[]> lstMeasureResult;

        public double[] wavelengthArray;
        private Frm_AWGTestClient frmAWGClient=new Frm_AWGTestClient() ;

        public double StepWavelength { get;set;}

        public double StartWavelength {get;set;}

        public double StopWavelength { get;set;}

        public int SamplingPoint { get;set;}

        public double MaxChannel { get; set; }

        #region Public Methods

        public void InitPowermeter()
        {
           
                foreach (var pm in lstPowermeter)
                {
                    PowerMeter.SetParameters((int)(StopWavelength + StartWavelength) / 2);
                }
        }
        /// <summary>
        /// 读取所有功率计的校准数据并保存
        /// </summary>
        /// <param name="strFileName">保存数据的路径</param>
        public void ReadSaveCaliData(string strFileName)
        {
            try
            {
                lstCaliResult = new List<double[]>();
                foreach (var pm in lstPowermeter)
                {
                    //获取到的是一个通道下4个偏正态的数据，所以需要用SplitArray将powers分成4个数组
                    pm.GetPowermeterData(out double[] powers, this.SamplingPoint*4);
                    
                  lstCaliResult.AddRange(SplitArray(powers, 4));
                }
                SaveAutoRawData(strFileName,lstCaliResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"读取校准功率值出错！{ex.Message}");
            }
        }

        public virtual void ReadTestPower()
        {
            
        }
        public void ReadSaveTestPower(string strFilePath)
        {
            ReadTestPower();
            SaveAutoRawData(strFilePath, lstMeasureResult);
        }
        
        /// <summary>
        /// 功率计启动扫描状态
        /// </summary>
        public void StartSweep()
        {
            GetGraphWavelength();
            foreach (var pm in lstPowermeter)
            {
                pm.StartSweep();
            }
          
        }

        /// <summary>
        /// 读取校准的RawData
        /// </summary>
        /// <param name="strFilePathName"></param>
        /// <returns></returns>
        public void ReadCaliRawData(string strFilePathName)
        {
         
            int dataRowNum = 0;

            string[] lineElems;
            int columnCount = 0;
            lstCaliResult = new List<double[]>();
            double[][] caliData;

            try
            {
                using (CsvReader reader = new CsvReader())
                {
                   
                    reader.OpenFile(strFilePathName);

                    // read the 1st line
                    lineElems = reader.GetLine();

                    lineElems = reader.GetLine();
                    if (lineElems.Length < 1)
                    {
                        frmAWGClient.ShowMsg($"There is no cali data in {strFilePathName}", false);
                        return;
                    }
                    //计算数据的列数columnNum，不包含wavelength那一列
                    for (int col = 1; col < lineElems.Length; col++)
                    {
                        if (lineElems[col] != null & lineElems[col] != "")
                        {
                            columnCount++;
                        }
                    }
                    caliData = new double[columnCount][];
                    for (int col = 0; col < columnCount; col++)
                    {
                        caliData[col] = new double[SamplingPoint];
                    }

                    do
                    {
                        try
                        {
                            for (int col = 0; col < columnCount; col++)
                            {
                                //从第二列开始读，第一列为波长
                                caliData[col][dataRowNum] = double.Parse(lineElems[col + 1]);
                            }
                            dataRowNum++;
                            if (dataRowNum >= this.SamplingPoint)
                                break;
                        }
                        catch (Exception ex)
                        {
                            string errMsg = String.Format("Invalid line in  file: '{0}', line {1}",
                                strFilePathName, dataRowNum);
                            throw new Exception(errMsg, ex);
                        }
                    }
                    while ((lineElems = reader.GetLine()) != null);

                    lstCaliResult.AddRange(caliData);
                   
                }
            }
            catch (Exception ex)
            {
                string errMsg = String.Format("Invalid line in  file: '{0}', line {1}", strFilePathName, dataRowNum + 1);
                //throw new Exception(errMsg, er);
                frmAWGClient.ShowMsg(errMsg + "  " + ex.ToString(), false);
            }
        }
        /// <summary>
        /// 根据测试值和校准值计算ILMinArray和ILMaxArray
        /// </summary>
        /// <param name="pstAutoWaveform"></param>
        public virtual void GetILMinMax(ref tagAutoWaveform pstAutoWaveform)
        {
           
        }

        /// <summary>
        /// 从csv文件读取IL数组
        /// </summary>
        /// <param name="strFilePathName"></param>
        /// <param name="pstAutoWaveform"></param>
        public virtual void ReadDatILMaxMinData(string strFilePathName, ref tagAutoWaveform pstAutoWaveform)
        {

        }
        #endregion

        #region private Methods

        public List<double[]> SplitArray(double[] array, int num)
        {
            
            List<double[]> lstArray = new List<double[]>(num);
            int perCount = array.Count() / num;
            double[] perArray = new double[perCount];
            for (int arrayNum=0; arrayNum < num; arrayNum++)
            {
                for(int j=0;j<perCount;j++)
                {
                    perArray[j] = array[j + arrayNum * perCount];
                }
                lstArray.Add(perArray);
            }
            return lstArray;
        }
        /// <summary>
        /// 保存指定数据列表到指定文件夹
        /// </summary>
        /// <param name="strFileName">目标路径</param>
        /// <param name="lstData">需要保存的数据源</param>
        /// <returns></returns>
        private bool SaveAutoRawData(string strFileName, List<double[]> lstData)
        {
            StringBuilder strNew = new StringBuilder();
            string str1 = "";
            int columnIndex, rowIndex;

            if (File.Exists(strFileName))
                File.Delete(strFileName);

            strNew.Append("WL,");
            try
            {
                //表头加上偏正态索引polIndex和通道索引ch
                for (int ch = 0; ch < lstPowermeter.Count; ch++)
                {
                    for (int polIndex = 0; polIndex < 4; polIndex++)
                    {
                        str1 = $"Power{polIndex}_ch{ch},";
                        strNew.Append(str1);
                    }
                }
                using (StreamWriter writer = new StreamWriter(strFileName, true))
                    writer.WriteLine(strNew);
                strNew.Clear();
                for (rowIndex = 0; rowIndex < this.SamplingPoint; rowIndex++)
                {
                    strNew.Append(Math.Round(wavelengthArray[rowIndex], 3).ToString() + ",");
                    for (columnIndex = 0; columnIndex < lstData.Count(); columnIndex++)
                    {
                        double power;
                        power = lstData[columnIndex][rowIndex];

                        if (double.IsNaN(power))
                        {
                            power = 65000;
                        }
                        str1 = Math.Round(power, 2).ToString() + ",";
                        strNew.Append(str1);
                    }
                    strNew.Append("\r\n");
                }
                using (StreamWriter writer = new StreamWriter(strFileName, true))
                    writer.WriteLine(strNew);
            }
            catch (Exception ex)
            {
                throw new Exception($"保存测试数据到CSV文件出错，{ex.Message}");
            }
            return true;
        }
        /// <summary>
        /// 获取采样点的波长值列表
        /// </summary>
        private void GetGraphWavelength()
        {
            wavelengthArray = new double[SamplingPoint];
            int point;
            for (point = 0x00; point < SamplingPoint; point++)
            {
                wavelengthArray[point] = this.StartWavelength + point * this.StepWavelength;
            }
        }

      
    }
    #endregion
}
