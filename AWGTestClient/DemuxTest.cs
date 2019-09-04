using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSystem.TestLibrary.Utilities;

namespace AWGTestClient
{
    class DemuxTest : CAWGTestBase
    {
        /// <summary>
        /// 读取测试数据，Demux只接入一个功率计
        /// </summary>
        public override void ReadTestPower()
        {
            try
            {
                lstMeasureResult = new List<double[]>();

                //获取到的是一个通道下4个偏正态的数据，所以需要用SplitArray将powers分成4个数组
                lstPowermeter[0].GetPowermeterData(out double[] powers, this.SamplingPoint * 4);

                lstMeasureResult.AddRange(SplitArray(powers, 4));
            }
            catch (Exception ex)
            {
                throw new Exception($"读取Demux测试功率值出错！{ex.Message}");
            }
        }
        public override void GetILMinMax(ref tagAutoWaveform pstAutoWaveform)
        {
            double m11;
            double m12;
            double m13;
            double m14;
            double TMax;
            double TMin;
            try
            {
                //功率由dBm转换成mW 10^(P/10)
                
                for (int point = 0; point < SamplingPoint; point++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (lstCaliResult[i][point] < lstMeasureResult[i][point])
                        {
                            throw new Exception("校准功率小于产品功率，请确认校准时接线是否准确！");
                        }
                    }
                    m11 = (Math.Pow(10, lstMeasureResult[0][point] / 10) / Math.Pow(10, lstCaliResult[0][point] / 10) + Math.Pow(10, lstMeasureResult[1][point] / 10) / Math.Pow(10, lstCaliResult[1][point] / 10)) / 2;
                    m12 = (Math.Pow(10, lstMeasureResult[0][point] / 10) / Math.Pow(10, lstCaliResult[0][point] / 10) - Math.Pow(10, lstMeasureResult[1][point] / 10) / Math.Pow(10, lstCaliResult[1][point] / 10)) / 2;
                    m13 = Math.Pow(10, lstMeasureResult[2][point] / 10) / Math.Pow(10, lstCaliResult[2][point] / 10) - m11;
                    m14 = Math.Pow(10, lstMeasureResult[3][point] / 10) / Math.Pow(10, lstCaliResult[3][point] / 10) - m11;
                    TMax = m11 + Math.Sqrt(m12 * m12 + m13 * m13 + m14 * m14);
                    if ((m12 * m12 - m13 * m13 - m14 * m14) < 0)
                    {
                        TMin = m11;
                    }
                    else
                    {
                        TMin = m11 - Math.Sqrt(m12 * m12 - m13 * m13 - m14 * m14);
                    }
                    for (int ch = 0; ch < MaxChannel; ch++)
                    {
                        pstAutoWaveform.m_pdwILMinArray[ch, point] = -10 * Math.Log10(TMax);
                        pstAutoWaveform.m_pdwILMaxArray[ch, point] = -10 * Math.Log10(TMin);
                    }
                    pstAutoWaveform.m_pdwWavelengthArray[point] = wavelengthArray[point];
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"计算ILMin和ILMax出错，{ex.Message}");
            }
        }

        public override void ReadDatILMaxMinData(string strFilePathName, ref tagAutoWaveform pstAutoWaveform)
        {
            double[] m_pdwWaveLengt = new double[SamplingPoint];
            try
            {
                using (CsvReader reader = new CsvReader())
                {
                    reader.OpenFile(strFilePathName);

                    String[] line;
                    int iCount = 0;
                    line = reader.GetLine();
                    while ((line = reader.GetLine()) != null)
                    {
                        m_pdwWaveLengt[iCount] = double.Parse(line[0]);
                        pstAutoWaveform.m_pdwWavelengthArray[iCount] = m_pdwWaveLengt[iCount];
                        for (int i = 0; i < this.MaxChannel; i++)
                        {
                            pstAutoWaveform.m_pdwILMinArray[i, iCount] = double.Parse(line[1]);
                            pstAutoWaveform.m_pdwILMaxArray[i, iCount] = double.Parse(line[2]);
                        }
                        iCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
