using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestSystem.TestLibrary.Utilities;
using TestSystem.TestLibrary.INI;
using TestSystem.InternalData;
using TestSystem.TestLibrary.Algorithms;

namespace AWGTestClient
{
    class AWGTestClient
    {
        public int m_dwSamplePoint;
        static double[] _gpdblSweepRate = new double[13] { 0.5, 1.0, 2.0, 5.0, 10.0, 20.0, 40.0, 50.0, 80.0, 100.0, 150.0, 160.0, 200.0 };
        static double[] _gpdblWindowIL = new double[7] { 1270.0, 1310.0, 1490.0, 1550.0, 1577.0, 1625.0, 1650.0 };
        //double m_dblStep;
        //double m_dwStartWavelength;
        //double m_dwStopWavelength;
        //int m_dwInputPortCounts;
        //int m_dwOutputPortCounts;
        //int m_dwChannelCounts;
        //double m_dblPower;
        //int m_dwOutput;
        //bool m_bLLog;
        //int m_dwTmplChannelCount;

        //int m_dwStepIndex = 0;
        public int CHANNEL_COUNT = 10;
        tagAutoWaveform m_stPLCData;
        tagPLCData m_stPLCTestResultData;
        tagPLCCriteria m_stCriteria;
        double[] m_pdwWave;
        double[,] m_pdwPwr;
        double[,] m_pdwPDL;
        double[] m_pdwRef;
        private Frm_AWGTestClient frmAWGClient;
        public AWGTestClient()
        {
        }

        public AWGTestClient(Frm_AWGTestClient frmClient,tagPLCCriteria PLCCriteria,tagAutoWaveform PLCData,tagPLCData testResult)
        {
            frmAWGClient = frmClient;
            m_stCriteria = PLCCriteria;
            m_stPLCData = PLCData;
            m_stPLCTestResultData = testResult;
        }
        public bool CalculateILPDL_New(ref tagPLCData pstResultData, ref tagAutoWaveform pstAutoWaveform, ref tagPLCCriteria pstCriteria, int dwStartChannel, int dwEndChannel,bool bUseITU)
        {
            bool bSuccess = false;
            int dwSamplePoint = pstAutoWaveform.m_dwSampleCount;
            dwSamplePoint = 7501;
            double[] pdwWaveArray = pstAutoWaveform.m_pdwWavelengthArray;
            if (pdwWaveArray == null)
                return false;
            double[] pdwMinLossArray = new double[dwSamplePoint];
            double[] pdwMaxLossArray = new double[dwSamplePoint];
            double[] pdwAverageArray = new double[dwSamplePoint];

            double[] dwMinLossMin = new double[CHANNEL_COUNT];
            double[] dwMinLossMax = new double[CHANNEL_COUNT];
            double[] dwMaxLossMin = new double[CHANNEL_COUNT];
            double[] dwMaxLossMax = new double[CHANNEL_COUNT];

            double[] pdwLossArray = new double[dwSamplePoint];
            double[] pdwPDLArray = new double[dwSamplePoint];
            double[,] CrossTalk = new double[CHANNEL_COUNT, CHANNEL_COUNT];

            int iPoint1 = 0;
            int iPoint2 = 0;
            int iStartLossWindow = 0;
            int iStopLossWindow = 0;
            int iStartLossWindowRipple = 0;
            int iStopLossWindowRipple = 0;
            int iStartLossWindowCrossTalk = 0;
            int iStopLossWindowCrossTalk = 0;
            int indexCW = 0;
            double deltaWave = 0;//
                //double deltaWaveRipple = 0;//
            double C = 299792458;
            int[] iPointLossWindow = new int[2];
            for (int iChannel = dwStartChannel; iChannel < dwEndChannel; iChannel++)
            {
                #region calc ILMax ILMin ILAve
                for (int i = 0; i < dwSamplePoint; i++)
                {
                    pdwLossArray[i] = pstAutoWaveform.m_pdwLossArray[iChannel, i];
                    pdwPDLArray[i] = pstAutoWaveform.m_pdwPDLArray[iChannel, i];
                }
                if (pdwLossArray == null)
                    return false;
                if (pdwPDLArray == null)
                    return false;

                string strChannelName = pstCriteria.m_strChannelName[iChannel];
                double dblCW = ConvertITUNameToWL(strChannelName);
                pstCriteria.m_dblItuWL[iChannel] = Math.Round(dblCW, 3);
                double dwCW = dblCW;
                for (int dwIndex = 0x00; dwIndex < dwSamplePoint; dwIndex++)
                {
                    double dblILave = pdwLossArray[dwIndex];

                    double dblPDL = pdwPDLArray[dwIndex];

                    double dblb = Math.Pow(10, dblILave / (-10.0));

                    double dblk = Math.Pow(10, dblPDL / 10.0);

                    double dblTemp = 2.0 * dblb / (1 + dblk);
                    dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                    pdwMaxLossArray[dwIndex] = dblTemp;

                    dblTemp = 2.0 * dblk * dblb / (1 + dblk);
                    dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                    pdwMinLossArray[dwIndex] = dblTemp;

                    pdwAverageArray[dwIndex] = (pdwMinLossArray[dwIndex] + pdwMaxLossArray[dwIndex]) / 2.0;
                }

                //string strFileRawData = Directory.GetCurrentDirectory() + string.Format("\\Data\\RawData_Station{0}_{1}_ILMaxMin.csv", 0,System.DateTime .Now .ToString ("yyyy-MM-dd-hh-mm"));
                //for (int i = 0; i < m_dwSamplePoint; i++)
                //{
                //    using (StreamWriter writer = new StreamWriter(strFileRawData, true))
                //        writer.WriteLine(pdwWaveArray[i] + "," + pdwMinLossArray[i] + "," + pdwMaxLossArray[i]);
                //}

                #endregion

                // CW
                #region Calc CW

                double MaxILMin = Alg_PointSearch.FindMinValueInArray(pdwMaxLossArray);
                int MaxILIndex = Alg_PointSearch.FindFirstIndexOfMinValueInArray(pdwMaxLossArray);
                double MaxILWavelength = pdwWaveArray[MaxILIndex];
                double[] CrossPoint1 = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);

                double[] CrossPoint = GetCrossPoint(pdwMinLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);

                pstResultData.m_dblCW[iChannel] = Math.Round((CrossPoint[0] + CrossPoint[1] + CrossPoint1[0] + CrossPoint1[1]) / 4, 3);
                indexCW = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, pstResultData.m_dblCW[iChannel], 0, dwSamplePoint-1);
                #endregion

                //Get IL Loss Window Point
                #region Get Loss Window point
                if (pstCriteria.m_dblILLossWindow > 100)
                {
                    if (bUseITU)
                        deltaWave = C * pstCriteria.m_dblILLossWindow / Math.Pow(C / pstCriteria.m_dblItuWL[iChannel], 2);
                    else
                        deltaWave = C * pstCriteria.m_dblILLossWindow / Math.Pow(C / pstResultData.m_dblCW[iChannel], 2);
                }
                else
                    deltaWave = pstCriteria.m_dblILLossWindow;
                iPointLossWindow = CalcLossWindowsPoint(pdwWaveArray, pstCriteria.m_dblILLossWindow, pstCriteria.m_dblItuWL[iChannel], pstResultData.m_dblCW[iChannel], dwSamplePoint, bUseITU);
                iStartLossWindow = iPointLossWindow[0];
                iStopLossWindow = iPointLossWindow[1];
                #endregion

                //Get Ripple Loss Window Point
                #region Get Ripple Loss Window point
                iPointLossWindow = CalcLossWindowsPoint(pdwWaveArray, pstCriteria.m_dblRippleLossWindow, pstCriteria.m_dblItuWL[iChannel], pstResultData.m_dblCW[iChannel], dwSamplePoint, bUseITU);
                iStartLossWindowRipple = iPointLossWindow[0];
                iStopLossWindowRipple = iPointLossWindow[1];
                #endregion

                //PDW
                #region PDW
                double[] ILMaxCrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, MaxILMin + 3,MaxILIndex);
                double[] ILMinCrossPoint = GetCrossPoint(pdwMinLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);
                pstResultData.m_dblPDW[iChannel] =Math.Round ( ((ILMinCrossPoint[1]+ ILMaxCrossPoint[0])-(ILMaxCrossPoint[1]+ ILMinCrossPoint[0])) /2,3);
                #endregion

                //Shift
                #region Shift
                pstResultData.m_dblShift[iChannel] = Math.Round(pstResultData.m_dblCW[iChannel] - dblCW, 3);
                #endregion

                //IL Min Max
                #region IL Min Max
                double ILMin =  Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iStartLossWindow, iStopLossWindow);
                double ILMax = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindow, iStopLossWindow);
                dwMinLossMin[iChannel] = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iStartLossWindow, iStopLossWindow);
                dwMinLossMax[iChannel] = Alg_PointSearch.FindMaxValueInArray(pdwMinLossArray, iStartLossWindow, iStopLossWindow);

                dwMaxLossMax[iChannel] = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindow, iStopLossWindow);
                dwMaxLossMin[iChannel] = Alg_PointSearch.FindMinValueInArray(pdwMaxLossArray, iStartLossWindow, iStopLossWindow);

                if (pstCriteria.m_bAlignIL)
                    pstResultData.m_dblILMin[iChannel] = Math.Round(ILMin - pstCriteria.m_dblAlignIL, 2);
                else
                    pstResultData.m_dblILMin[iChannel] = Math.Round(ILMin, 2);

                if (pstCriteria.m_bAlignIL)
                    pstResultData.m_dblILMax[iChannel] = Math.Round(ILMax - pstCriteria.m_dblAlignIL, 2);
                else
                    pstResultData.m_dblILMax[iChannel] = Math.Round(ILMax, 2);

                #endregion

                // for IL@ITU, Ripple, PDL ITU, PDL CRT, PDL Max
                #region for IL@ITU , Ripple, PDL ITU, PDL CRT, PDL Max
                double dwTargetData = 0;
                //if (bUseITU)
                    dwTargetData = pstCriteria.m_dblItuWL[iChannel];
                //else
                //    dwTargetData = pstResultData.m_dblCW[iChannel] * 1000.0;
                iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                if (pstCriteria.m_bAlignIL)
                    pstResultData.m_dblILITU[iChannel] = Math.Round(pdwMaxLossArray[iPoint1] - pstCriteria.m_dblAlignIL, 2);
                else
                    pstResultData.m_dblILITU[iChannel] = Math.Round(pdwMaxLossArray[iPoint1], 2);

                // for Ripple
                double ILMinRipple = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iStartLossWindowRipple, iStopLossWindowRipple);
               double ILMaxRipple = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindowRipple, iStopLossWindowRipple);
                pstResultData.m_dblRipple[iChannel] = Math.Round((ILMaxRipple - ILMinRipple), 2);

                // for PDL ITU
                dwTargetData = Math.Truncate(dwCW - 1.402);
                iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                pstResultData.m_dblPDLITU[iChannel] = Math.Round((pdwMaxLossArray[iPoint1] - pdwMinLossArray[iPoint1]), 2);

                // for PDL CRT
                dwTargetData = Math.Truncate(pstResultData.m_dblCW[iChannel]);
                iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                pstResultData.m_dblPDLCRT[iChannel] = Math.Round((pdwMaxLossArray[iPoint1]- pdwMinLossArray[iPoint1] ), 2);

                // for PDL Max
                //dwTempData = ((0.8 * pstCriteria.m_dblILLossWindow) / 100.0) * 1000.0;
                dwTargetData = Math.Round(pstResultData.m_dblCW[iChannel] - deltaWave,3);
                iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);

                dwTargetData = Math.Round(pstResultData.m_dblCW[iChannel] + deltaWave,3);
                iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                double  lTemp = 0;
                for (int dwIndex = iPoint1-1; dwIndex < iPoint2; dwIndex++)
                {
                    double dwILMinLeft = pdwMinLossArray[dwIndex];
                    double dwILMaxLeft = pdwMaxLossArray[dwIndex];

                    if ((dwILMaxLeft - dwILMinLeft) > lTemp)
                        lTemp = dwILMaxLeft - dwILMinLeft;
                }
                pstResultData.m_dblPDLMax[iChannel] = Math.Round(lTemp, 2);
                #endregion 

                // for BW 0.5dB 1dB 3dBm 20dB 25dB 30dB
                #region for BW 0.5dB 1dB 3dBm 20dB 25dB 30dB

                CrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, dwMaxLossMin[iChannel] + 0.5, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                pstResultData.m_dblBW05dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                CrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, dwMaxLossMin[iChannel] + 1, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                pstResultData.m_dblBW1dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                CrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, dwMaxLossMin[iChannel] + 3, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                pstResultData.m_dblBW3dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                CrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, dwMaxLossMin[iChannel] + 20, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                pstResultData.m_dblBW20dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                CrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, dwMaxLossMin[iChannel] + 25, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                pstResultData.m_dblBW25dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                CrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, dwMaxLossMin[iChannel] + 30, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                pstResultData.m_dblBW30dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);
                #endregion

                #region  CrossTalk
                double deltaWaveCrossTalk = 0;
                if (pstCriteria.m_dblCrossTalkLossWindow > 100)
                {
                    if (bUseITU)
                        deltaWaveCrossTalk = C * pstCriteria.m_dblCrossTalkLossWindow / Math.Pow(C / pstCriteria.m_dblItuWL[iChannel], 2);
                    else
                        deltaWaveCrossTalk = C * pstCriteria.m_dblCrossTalkLossWindow / Math.Pow(C / pstResultData.m_dblCW[iChannel], 2);
                }
                else
                    deltaWaveCrossTalk = pstCriteria.m_dblCrossTalkLossWindow;
                for (int iCrossChannel = 0; iCrossChannel < dwEndChannel; iCrossChannel++)
                {
                    if (iChannel == iCrossChannel)
                    {
                        CrossTalk[iChannel, iCrossChannel] = 0;
                        continue;
                    }
                    strChannelName = pstCriteria.m_strChannelName[iCrossChannel];
                    dblCW = ConvertITUNameToWL(strChannelName);
                    dwCW = pstResultData.m_dblCW[iCrossChannel];

                    double dwTargetDataCrossTalk = dblCW - deltaWaveCrossTalk;
                    iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetDataCrossTalk);

                    dwTargetDataCrossTalk = dblCW + deltaWaveCrossTalk;
                    iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetDataCrossTalk);

                    double cross = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iPoint1 - 1, iPoint2);
                    CrossTalk[iChannel, iCrossChannel] = Math.Round(cross, 3);
                }
                #endregion

                #region for AX-
                if (iChannel == 0x00)
                    pstResultData.m_dblAXLeft[iChannel] = 0.0;
                else
                {
                    //if (pstCriteria.m_bAlignIL)
                    //    pstResultData.m_dblAXLeft[iChannel] = Math.Round(CrossTalk[iChannel, iChannel - 1] -
                    //                     dwMaxLossMax[iChannel], 2);
                    //else
                    pstResultData.m_dblAXLeft[iChannel] = Math.Round(CrossTalk[iChannel, iChannel - 1] -
                                     dwMaxLossMax[iChannel], 2);
                }
                #endregion

                #region  for AX+
                if (iChannel == dwEndChannel - 1)
                    pstResultData.m_dblAXRight[iChannel] = 0.0;
                else
                {
                    //if (pstCriteria.m_bAlignIL)
                    //    pstResultData.m_dblAXRight[iChannel] = Math.Round(CrossTalk[iChannel, iChannel + 1] -
                    //                     dwMaxLossMax[iChannel] / 1000.0, 2);
                    //else
                    pstResultData.m_dblAXRight[iChannel] = Math.Round(CrossTalk[iChannel, iChannel + 1] - dwMaxLossMax[iChannel], 2);
                }
                #endregion

                #region for TAX
                double dblTemp1 = Math.Pow(10, -pstResultData.m_dblAXLeft[iChannel] / 10.0);
                double dblTemp2 = Math.Pow(10, -pstResultData.m_dblAXRight[iChannel] / 10.0);
                //double dblTemp1 = Calc10Power(pstResultData.m_dblAXLeft[iChannel]);
                //double dblTemp2 = Calc10Power(pstResultData.m_dblAXRight[iChannel]);
                double Temp = 0;
                if (iChannel == 0)
                    Temp = dblTemp2;
                else if (iChannel == dwEndChannel - 1)
                    Temp = dblTemp1;
                else
                    Temp = dblTemp1 + dblTemp2;
                pstResultData.m_dblTAX[iChannel] = CalcTotal(Temp);
                #endregion

                #region  for NX
                double dblNX = 10000000.0;
                for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                {
                    if (dwITUIndex < iChannel && (iChannel - dwITUIndex) < 2)
                        continue;

                    if (dwITUIndex >= iChannel && (dwITUIndex - iChannel) < 2)
                        continue;

                    if ((CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]) < dblNX)
                        dblNX = CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel];
                }

                //if (pstCriteria.m_bAlignIL)
                //    pstResultData.m_dblNX[iChannel] = Math.Round(dblNX, 2);
                //else
                pstResultData.m_dblNX[iChannel] = Math.Round(dblNX, 2);
                #endregion

                #region  for TX
                double dblTX = 0.0;
                for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                {
                    if (dwITUIndex == iChannel)
                        continue;
                    //dblTX += Math.Pow(10, ((-1) * (CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel] / 1000.0)) / 10.0);
                    dblTX += Calc10Power(CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]);
                }
                //dblTX = Math.Abs(10.0 * Math.Log10(dblTX));
                pstResultData.m_dblTX[iChannel] = CalcTotal(dblTX);
                #endregion

                #region  for TX - AX
                double dblTXAX = 0.0;
                for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                {
                    if (dwITUIndex < iChannel && (iChannel - dwITUIndex) < 2)
                        continue;

                    if (dwITUIndex >= iChannel && (dwITUIndex - iChannel) < 2)
                        continue;

                    //dblTXAX += Math.Pow(10, ((-1) * (CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel] / 1000.0)) / 10.0);
                    dblTXAX += Calc10Power(CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]);
                }
                //dblTXAX = Math.Abs(10.0 * Math.Log10(dblTXAX));
                pstResultData.m_dblTXAX[iChannel] = CalcTotal(dblTXAX);
                #endregion
                //Cust1 Cust2
                //MaxIL
            }

            for (int iChannel = 0; iChannel < dwEndChannel; iChannel++)
            {
                #region calc ILMax ILMin ILAve
                for (int i = 0; i < dwSamplePoint; i++)
                {
                    pdwLossArray[i] = pstAutoWaveform.m_pdwLossArray[iChannel, i];
                    pdwPDLArray[i] = pstAutoWaveform.m_pdwPDLArray[iChannel, i];
                }
                if (pdwLossArray == null)
                    return false;
                if (pdwPDLArray == null)
                    return false;

                string strChannelName = pstCriteria.m_strChannelName[iChannel];
                double dblCW = ConvertITUNameToWL(strChannelName);
                pstCriteria.m_dblItuWL[iChannel] = Math.Round(dblCW, 3);
                double dwCW = dblCW ;
                for (int dwIndex = 0x00; dwIndex < dwSamplePoint; dwIndex++)
                {
                    double dblILave = pdwLossArray[dwIndex];

                    double dblPDL = pdwPDLArray[dwIndex];

                    double dblb = Math.Pow(10, dblILave / (-10.0));

                    double dblk = Math.Pow(10, dblPDL / 10.0);

                    double dblTemp = 2.0 * dblb / (1 + dblk);
                    dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                    pdwMaxLossArray[dwIndex] = dblTemp;

                    dblTemp = 2.0 * dblk * dblb / (1 + dblk);
                    dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                    pdwMinLossArray[dwIndex] = dblTemp;

                    pdwAverageArray[dwIndex] = (pdwMinLossArray[dwIndex] + pdwMaxLossArray[dwIndex]) / 2.0;
                }
                //string strFileRawData = Directory.GetCurrentDirectory() + string.Format("\\Data\\RawData_Station{0}_{1}_ILMaxMin.csv", 0,System.DateTime .Now .ToString ("yyyy-MM-dd-hh-mm"));
                //for (int i = 0; i < m_dwSamplePoint; i++)
                //{
                //    using (StreamWriter writer = new StreamWriter(strFileRawData, true))
                //        writer.WriteLine(pdwWaveArray[i] + "," + pdwMinLossArray[i] + "," + pdwMaxLossArray[i]);
                //}
                #endregion

                #region  CrossTalk
                double deltaWaveCrossTalk = 0;
                if (pstCriteria.m_dblCrossTalkLossWindow > 100)
                {
                    if (bUseITU)
                        deltaWaveCrossTalk = C * pstCriteria.m_dblCrossTalkLossWindow / Math.Pow(C / pstCriteria.m_dblItuWL[iChannel], 2);
                    else
                        deltaWaveCrossTalk = C * pstCriteria.m_dblCrossTalkLossWindow / Math.Pow(C / pstResultData.m_dblCW[iChannel], 2);
                }
                else
                    deltaWaveCrossTalk = pstCriteria.m_dblCrossTalkLossWindow;
                for (int iCrossChannel = 0; iCrossChannel < dwEndChannel; iCrossChannel++)
                {
                    if (iChannel == iCrossChannel)
                    {
                        CrossTalk[iChannel, iCrossChannel] = 0;
                        continue;
                    }
                    strChannelName = pstCriteria.m_strChannelName[iCrossChannel];
                    dblCW = ConvertITUNameToWL(strChannelName);
                    dwCW = pstResultData.m_dblCW[iCrossChannel];

                    double dwTargetData = dblCW - deltaWaveCrossTalk;
                    iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);

                    dwTargetData = dblCW + deltaWaveCrossTalk;
                    iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);

                    double cross = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iPoint1 - 1, iPoint2);
                    CrossTalk[iChannel, iCrossChannel] = Math.Round(cross, 3);
                }
                #endregion

                #region Get CrossTalk Loss Window point
                //iPointLossWindow = CalcLossWindowsPoint(pdwWaveArray, deltaWave, pstCriteria.m_dblItuWL[iChannel], pstResultData.m_dblCW[iChannel], dwSamplePoint, bUseITU);
                //iStartLossWindowCrossTalk = iPointLossWindow[0];
                //iStopLossWindowCrossTalk = iPointLossWindow[1];

                //dwMaxLossMin = new double[CHANNEL_COUNT];
                //dwMaxLossMax = new double[CHANNEL_COUNT];
                //dwMaxLossMax[iChannel] = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindowCrossTalk, iStopLossWindowCrossTalk);
                //dwMaxLossMin[iChannel] = Alg_PointSearch.FindMinValueInArray(pdwMaxLossArray, iStartLossWindowCrossTalk, iStopLossWindowCrossTalk);
                #endregion

                #region for AX-
                if (iChannel == 0x00)
                    pstResultData.m_dblAXLeft[iChannel] = 0.0;
                else
                {
                    //if (pstCriteria.m_bAlignIL)
                    //    pstResultData.m_dblAXLeft[iChannel] = Math.Round(CrossTalk[iChannel, iChannel - 1] -
                    //                     dwMaxLossMax[iChannel], 2);
                    //else
                        pstResultData.m_dblAXLeft[iChannel] = Math.Round(CrossTalk[iChannel, iChannel - 1] -
                                         dwMaxLossMax[iChannel], 2);
                }
                #endregion

                #region  for AX+
                if (iChannel == dwEndChannel - 1)
                    pstResultData.m_dblAXRight[iChannel] = 0.0;
                else
                {
                    //if (pstCriteria.m_bAlignIL)
                    //    pstResultData.m_dblAXRight[iChannel] = Math.Round(CrossTalk[iChannel, iChannel + 1] -
                    //                     dwMaxLossMax[iChannel] / 1000.0, 2);
                    //else
                    pstResultData.m_dblAXRight[iChannel] = Math.Round(CrossTalk[iChannel, iChannel + 1] -dwMaxLossMax[iChannel], 2);
                }
                #endregion

                #region for TAX
                double dblTemp1 = Math.Pow(10, -pstResultData.m_dblAXLeft[iChannel] / 10.0);
                double dblTemp2 = Math.Pow(10, -pstResultData.m_dblAXRight[iChannel] / 10.0);
                //double dblTemp1 = Calc10Power(pstResultData.m_dblAXLeft[iChannel]);
                //double dblTemp2 = Calc10Power(pstResultData.m_dblAXRight[iChannel]);
                double Temp = 0;
                if (iChannel == 0)
                    Temp = dblTemp2;
                else if (iChannel == dwEndChannel-1)
                    Temp = dblTemp1;
                else
                    Temp = dblTemp1 + dblTemp2;
                pstResultData.m_dblTAX[iChannel] = CalcTotal(Temp);
                #endregion

                #region  for NX
                double dblNX = 10000000.0;
                for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                {
                    if (dwITUIndex < iChannel && (iChannel - dwITUIndex) < 2)
                        continue;

                    if (dwITUIndex >= iChannel && (dwITUIndex - iChannel) < 2)
                        continue;

                    if ((CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]) < dblNX)
                        dblNX = CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel];
                }

                //if (pstCriteria.m_bAlignIL)
                //    pstResultData.m_dblNX[iChannel] = Math.Round(dblNX, 2);
                //else
                pstResultData.m_dblNX[iChannel] = Math.Round(dblNX, 2);
                #endregion

                #region  for TX
                double dblTX = 0.0;
                for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                {
                    if (dwITUIndex == iChannel)
                        continue;
                    //dblTX += Math.Pow(10, ((-1) * (CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel] / 1000.0)) / 10.0);
                    dblTX += Calc10Power(CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]);
                }
                //dblTX = Math.Abs(10.0 * Math.Log10(dblTX));
                pstResultData.m_dblTX[iChannel] = CalcTotal(dblTX);
                #endregion

                #region  for TX - AX
                double dblTXAX = 0.0;
                for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                {
                    if (dwITUIndex < iChannel && (iChannel - dwITUIndex) < 2)
                        continue;

                    if (dwITUIndex >= iChannel && (dwITUIndex - iChannel) < 2)
                        continue;

                    //dblTXAX += Math.Pow(10, ((-1) * (CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel] / 1000.0)) / 10.0);
                    dblTXAX += Calc10Power(CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]);
                }
                //dblTXAX = Math.Abs(10.0 * Math.Log10(dblTXAX));
                pstResultData.m_dblTXAX[iChannel] = CalcTotal(dblTXAX);
                #endregion

            }

            #region  for TNX
            for (int dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
            {
                double dblTemp = 0.0;
                for (int dwIndex = dwStartChannel; dwIndex < dwEndChannel; dwIndex++)
                {
                    if (dwIndex == dwChannelIndex)
                        continue;

                    //dblTemp += Math.Pow(10, pstResultData.m_dblNX[dwChannelIndex] / 10.0);
                    dblTemp += Calc10Power( pstResultData.m_dblNX[dwChannelIndex]);
                }

                pstResultData.m_dblTNX[dwChannelIndex] = CalcTotal(dblTemp);
            }
            #endregion

            //Uniformity
            double max = Alg_PointSearch.FindMaxValueInArray(pstResultData.m_dblILMax);
            double min = Alg_PointSearch.FindMinValueInArray(pstResultData.m_dblILMax);
            pstResultData.m_dblUniformity = Math.Round((max - min), 3);

            return bSuccess;
        }

        public int [] CalcLossWindowsPoint(double [] pdwWaveArray,double lossWindow, double ItuWL,double CW, int dwSamplePoint, bool bUseITU)
        {
            int iPoint1, iPoint2, iStartLossWindow, iStopLossWindow;
            double deltaWave, waveStartLossWindow, waveStopLossWindow;
            double C = 299792458;

            int[] iPoint = new int[2];
            if (lossWindow > 100)
            {
                if (bUseITU)
                    deltaWave = Math.Truncate(C * lossWindow / Math.Pow(C / ItuWL, 2));
                else
                    deltaWave = Math.Truncate(C * lossWindow / Math.Pow(C / CW, 2));
            }
            else
                deltaWave = lossWindow;

            //if (bUseITU)
            //    deltaWave = Math.Truncate( C * lossWindow / Math.Pow(C / ItuWL, 2));
            //else
            //    deltaWave = Math.Truncate(C * lossWindow / Math.Pow(C / CW, 2));

            //double dwTempData = (((0.8 * pstCriteria.m_dblILLossWindow) / 100.0) * 1000.0);
            //double dwTempData = deltaWave;

            if (bUseITU)
                waveStartLossWindow = Math.Round(ItuWL - deltaWave, 3);
            else
                waveStartLossWindow = Math.Round(CW - deltaWave, 3);
            //if (bUseITU)
            //    waveStartLossWindow = Math.Truncate(ItuWL - deltaWave);
            //else
            //    waveStartLossWindow = Math.Truncate(CW - deltaWave);
            iPoint1 = Alg_PointSearch.FindLastValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, waveStartLossWindow, 0, dwSamplePoint - 1);
            iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, waveStartLossWindow, 0, dwSamplePoint - 1);
            iStartLossWindow = iPoint1;

            if (bUseITU)
                waveStopLossWindow = Math.Round(ItuWL + deltaWave, 3);
            else
                waveStopLossWindow = Math.Round(CW + deltaWave, 3);

            //if (bUseITU)
            //    waveStopLossWindow = Math.Truncate(ItuWL + deltaWave);
            //else
            //    waveStopLossWindow = Math.Truncate(CW + deltaWave);
            iPoint1 = Alg_PointSearch.FindLastValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, waveStopLossWindow, 0, dwSamplePoint - 1);
            iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, waveStopLossWindow, 0, dwSamplePoint - 1);
            iStopLossWindow = iPoint2;

            iPoint[0] = iStartLossWindow;
            iPoint[1] = iStopLossWindow;
            return iPoint;
        }
        public double Calc10Power(double data)
        {
            double result= Math.Pow(10, -data / 10.0);
            return result;
        }
        public double CalcTotal(double data)
        {
            double result = -Math.Round(10.0 * Math.Log10(data), 2);
            return result;
        }
        public double[] GetCrossPoint(double[] RawDataY, double[] RawDataX, double targetLine, int iSplitIndex)
        {
            double WaveLeftLess, WaveLeftGreater, WaveRightLess, WaveRightGreater, IL1, IL2, IL3, IL4;
            double waveLeftILMax = 0;
            double waveRightILMax = 0;
            int indexLeftLess, indexLeftGreater, indexRigtLess, indexRigtGreater;

            int iLen = RawDataY.Length;
            double[] CrossPoint = new double[2]; //Index 0 for Left,1 for right
            indexLeftLess = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
            indexLeftGreater = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
            indexRigtLess = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);
            indexRigtGreater = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);

            WaveLeftLess = RawDataX[indexLeftLess];
            WaveLeftGreater = RawDataX[indexLeftGreater];
            WaveRightLess = RawDataX[indexRigtLess];
            WaveRightGreater = RawDataX[indexRigtGreater];
            IL1 = RawDataY[indexLeftLess];
            IL2 = RawDataY[indexLeftGreater];
            IL3 = RawDataY[indexRigtLess];
            IL4 = RawDataY[indexRigtGreater];
            //double wave3dBILMaxLeft = Math.Round((Wave3dBLeftLess + Wave3dBLeftGreater + Wave3dBRightLess + Wave3dBRightGreater) / 4 / 1000.0, 3);

            if (IL1 != IL2)
                waveLeftILMax = LinearInterpolateAlgorithm.Calculate(IL1, IL2, WaveLeftLess, WaveLeftGreater, targetLine);
            else
                waveLeftILMax = WaveLeftLess;

            if (IL3 != IL4)
                waveRightILMax = LinearInterpolateAlgorithm.Calculate(IL3, IL4, WaveRightLess, WaveRightGreater, targetLine);
            else
                waveRightILMax = WaveRightLess;

            CrossPoint[0] = waveLeftILMax;
            CrossPoint[1] = waveRightILMax;

            return CrossPoint;
        }
        public double[] GetCrossPoint(double [] RawDataY, double[] RawDataX,double targetLine,int iSplitIndex,double CW)
        {
            double WaveLeftLess, WaveLeftGreater, WaveRightLess, WaveRightGreater, IL1, IL2, IL3, IL4;
            double waveLeftILMax = 0;
            double waveRightILMax = 0;
            int indexLeftLess, indexLeftGreater, indexRigtLess, indexRigtGreater;

            int iLen = RawDataY.Length;
            double[] CrossPoint = new double[2]; //Index 0 for Left,1 for right
            indexLeftLess = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, 0, iSplitIndex-1);
            indexLeftGreater = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, 0, iSplitIndex-1);
            indexRigtLess = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, iSplitIndex+1, iLen-1);
            indexRigtGreater = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, iSplitIndex+1, iLen-1);
            if (indexLeftLess == -1 || indexLeftGreater == -1)
            {
                WaveRightLess = RawDataX[indexRigtLess];
                WaveRightGreater = RawDataX[indexRigtGreater];
                IL3 = RawDataY[indexRigtLess];
                IL4 = RawDataY[indexRigtGreater];
                if (IL3 != IL4)
                    waveRightILMax = LinearInterpolateAlgorithm.Calculate(IL3, IL4, WaveRightLess, WaveRightGreater, targetLine);
                else
                    waveRightILMax = WaveRightLess;

                CrossPoint[0] = CW- (waveRightILMax - CW);
                CrossPoint[1] = waveRightILMax;
            }
            else if (indexRigtLess == -1 || indexRigtGreater == -1)
            {
                WaveLeftLess = RawDataX[indexLeftLess];
                WaveLeftGreater = RawDataX[indexLeftGreater];
                IL1 = RawDataY[indexLeftLess];
                IL2 = RawDataY[indexLeftGreater];
                if (IL1 != IL2)
                    waveLeftILMax = LinearInterpolateAlgorithm.Calculate(IL1, IL2, WaveLeftLess, WaveLeftGreater, targetLine);
                else
                    waveLeftILMax = WaveLeftLess;
                CrossPoint[0] = waveLeftILMax;
                CrossPoint[1] = CW + (CW - waveLeftILMax);
            }
            else 
            {
                WaveLeftLess = RawDataX[indexLeftLess];
                WaveLeftGreater = RawDataX[indexLeftGreater];
                WaveRightLess = RawDataX[indexRigtLess];
                WaveRightGreater = RawDataX[indexRigtGreater];
                IL1 = RawDataY[indexLeftLess];
                IL2 = RawDataY[indexLeftGreater];
                IL3 = RawDataY[indexRigtLess];
                IL4 = RawDataY[indexRigtGreater];
                //double wave3dBILMaxLeft = Math.Round((Wave3dBLeftLess + Wave3dBLeftGreater + Wave3dBRightLess + Wave3dBRightGreater) / 4 / 1000.0, 3);

                if (IL1 != IL2)
                    waveLeftILMax = LinearInterpolateAlgorithm.Calculate(IL1, IL2, WaveLeftLess, WaveLeftGreater, targetLine);
                else
                    waveLeftILMax = WaveLeftLess;

                if (IL3 != IL4)
                    waveRightILMax = LinearInterpolateAlgorithm.Calculate(IL3, IL4, WaveRightLess, WaveRightGreater, targetLine);
                else
                    waveRightILMax = WaveRightLess;

                CrossPoint[0] = waveLeftILMax;
                CrossPoint[1] = waveRightILMax;
            }

            return CrossPoint;
        }

        public bool CalculateILPDL(ref tagPLCData pstResultData, ref tagAutoWaveform pstAutoWaveform, ref tagPLCCriteria pstCriteria, int dwStartChannel, int dwEndChannel)
        {
            int dwMaxIndex, dwMinIndex, dwDestIndex, dwStartIndex, dwEndIndex, dwLeftIndex, dwRightIndex;
            int dwIndex, dwChannelIndex, dwSamplePoint, dwWLCount, dwTargetIndex;
            double dwTargetData = 0;
            double dwTempData;
            string strChannelName = "";
            double dblCW;
            double lTemp, lTemp1, lTemp2;
            double dblTemp;
            //double dblCrossTalk[1000];
            //DWORD* pdwMinLossArray, *pdwMaxLossArray, *pdwAverageArray;
            double dwCW, dwCWRight, dwCWLeft, dwCWMin, dwCWMax;
            double dwILMinLeft, dwILMinRight, dwILMaxLeft, dwILMaxRight, dwILLeft, dwILRight;
            double dwWLMinLeft, dwWLMinRight, dwWLMaxLeft, dwWLMaxRight, dwWLLeft, dwWLRight;
            //double dwCrTalk1, dwCrTalk2, dwITUCount, dwChannelITU;
            double dwWave3dBMin1, dwWave3dBMax1, dwWave3dBMin2, dwWave3dBMax2;
            double dblWave3dBMin1, dblWave3dBMax1, dblWave3dBMin2, dblWave3dBMax2, dblWinPos, dblWinNeg;
            double dwILMin, dwILMax;
            double dblILave, dblPDL, dblb, dblk;
            int dwITUIndex;
            //DWORD dwMinLossMin[10], dwMinLossMax[10], dwMaxLossMin[10], dwMaxLossMax[10];
            //double dblY[21], dblX[21];
            double dblK1, dblK2, dblK3, dblK4, dblK5, dblK6, dblK7;
            double dblA, dblB, dblC;
            double dblIL, dblWL, dblTemp1, dblTemp2;
            bool bFunctionOK = false;
            double[] dblY = new double[21];
            double[] dblX = new double[21];
            double[] dwMinLossMin = new double[10];
            double[] dwMinLossMax = new double[10];
            double[] dwMaxLossMin = new double[10];
            double[] dwMaxLossMax = new double[10];
            double[] dblCrossTalk = new double[1000];

            dwSamplePoint = pstAutoWaveform.m_dwSampleCount;

            double[] pdwWaveArray = pstAutoWaveform.m_pdwWavelengthArray;
            if (pdwWaveArray == null)
                return false;
            double[] pdwMinLossArray = new double[dwSamplePoint];
            double[] pdwMaxLossArray = new double[dwSamplePoint];
            double[] pdwAverageArray = new double[dwSamplePoint];

            double[] pdwLossArray = new double[dwSamplePoint];
            double[] pdwPDLArray = new double[dwSamplePoint];

            try
            {
                #region cycle channel
                for (dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
                {
                    for (int i = 0; i < dwSamplePoint; i++)
                    {
                        pdwLossArray[i] = pstAutoWaveform.m_pdwLossArray[dwChannelIndex, i];
                        pdwPDLArray[i] = pstAutoWaveform.m_pdwPDLArray[dwChannelIndex, i];
                    }
                    if (pdwLossArray == null)
                        return false;
                    if (pdwPDLArray == null)
                        return false;

                    strChannelName = pstCriteria.m_strChannelName[dwChannelIndex];
                    dblCW = ConvertITUNameToWL(strChannelName);
                    pstCriteria.m_dblItuWL[dwChannelIndex] = Math.Round(dblCW, 3);
                    dwCW = dblCW * 1000.0;
                    for (dwIndex = 0x00; dwIndex < dwSamplePoint; dwIndex++)
                    {
                        dblILave = pdwLossArray[dwIndex] / 1000.0;

                        dblPDL = pdwPDLArray[dwIndex] / 1000.0;

                        dblb = Math.Pow(10, dblILave / (-10.0));

                        dblk = Math.Pow(10, dblPDL / 10.0);

                        dblTemp = 2.0 * dblb / (1 + dblk);
                        dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                        pdwMaxLossArray[dwIndex] = dblTemp * 1000.0;

                        dblTemp = 2.0 * dblk * dblb / (1 + dblk);
                        dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                        pdwMinLossArray[dwIndex] = dblTemp * 1000.0;

                        pdwAverageArray[dwIndex] = (pdwMinLossArray[dwIndex] + pdwMaxLossArray[dwIndex]) / 2.0;
                    }
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;
                    dwMinIndex = 0;
                    bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    if (dwMinIndex <= 10)
                        dwMinIndex = 10;
                    for (int i = 0x00; i < 21; i++)
                    {
                        dblIL = pdwMinLossArray[dwMinIndex - 10 + i] / 1000.0;
                        dblY[i] = -1.0 * dblIL;

                        dblWL = (pdwWaveArray[dwMinIndex - 10] + pdwWaveArray[dwMinIndex + 10]) / 2 / 1000.0;

                        if (pdwWaveArray[dwMinIndex - 10 + i] <= dblWL * 1000.0)
                            dblWL = -1.0 * (dblWL * 1000.0 - pdwWaveArray[dwMinIndex - 10 + i]) / 1000.0;
                        else
                            dblWL = (pdwWaveArray[dwMinIndex - 10 + i] - dblWL * 1000.0) / 1000.0;

                        dblX[i] = dblWL;
                    }
                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 21; i++)
                    {
                        dblK1 += dblX[i];
                        dblK2 += Math.Pow(dblX[i], 2.0);
                        dblK3 += Math.Pow(dblX[i], 3.0);
                        dblK4 += Math.Pow(dblX[i], 4.0);
                        dblK5 += dblY[i];
                        dblK6 += dblY[i] * dblX[i];
                        dblK7 += dblY[i] * Math.Pow(dblX[i], 2.0);
                    }

                    dblTemp1 = dblK6 * (dblK1 * dblK2 - 21.0 * dblK3) + dblK1 * (dblK5 * dblK3 - dblK7 * dblK1) + dblK2 * (21.0 * dblK7 - dblK5 * dblK2);
                    dblTemp2 = dblK4 * (21.0 * dblK2 - Math.Pow(dblK1, 2.0)) + dblK3 * (2.0 * dblK1 * dblK2 - 21.0 * dblK3) - Math.Pow(dblK2, 3.0);
                    dblA = dblTemp1 / dblTemp2;

                    dblTemp1 = dblA * (21.0 * dblK3 - dblK1 * dblK2) + (dblK5 * dblK1 - 21.0 * dblK6);
                    dblTemp2 = Math.Pow(dblK1, 2.0) - 21.0 * dblK2;
                    dblB = dblTemp1 / dblTemp2;

                    dblTemp1 = dblK5 - dblB * dblK1 - dblA * dblK2;
                    dblTemp2 = 21.0;
                    dblC = dblTemp1 / dblTemp2;

                    dblTemp2 = Math.Pow(dblB, 2.0);

                    dblTemp2 = dblTemp2 / 4.0;

                    dblTemp2 = dblTemp2 / dblA;

                    dblTemp1 = dblC - dblTemp2;

                    dwTargetData = Math.Truncate((-1.0) * dblTemp1 * 1000.0);
                    dwILMin = Math.Truncate((-1.0) * dblTemp1 * 1000.0);
                    dwTargetData += 3000;
                    dwTargetIndex = 0;
                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);
                    dwWave3dBMax1 = pdwWaveArray[dwTargetIndex];

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);
                    dwWave3dBMax2 = pdwWaveArray[dwTargetIndex];

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);
                    dwWave3dBMin1 = pdwWaveArray[dwTargetIndex];

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);
                    dwWave3dBMin2 = pdwWaveArray[dwTargetIndex];


                    pstResultData.m_dblCW[dwChannelIndex] =Math.Round ( (dwWave3dBMin1 + dwWave3dBMax1 + dwWave3dBMin2 + dwWave3dBMax2) / 4 / 1000.0,3);

                    dwTargetData = dwILMin + 3000;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);
                    dwLeftIndex = dwTargetIndex;

                    if (dwLeftIndex <= 4)
                        dwLeftIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;

                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwLeftIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwLeftIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }
                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 1000) / 1000.0);
                    dblTemp1 += dblB;

                    dblWave3dBMax1 = dblTemp1;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);
                    dwRightIndex = dwTargetIndex;

                    if (dwRightIndex <= 2)
                        dwRightIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;

                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwRightIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwRightIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }


                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblWave3dBMax2 = -1.0 * dblA * ((dwILMin + 1000) / 1000.0) + dblB;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwLeftIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwLeftIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblWave3dBMin1 = -1.0 * dblA * ((dwILMin + 1000) / 1000.0) + dblB;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwRightIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwRightIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblWave3dBMin2 = -1.0 * dblA * ((dwILMin + 1000) / 1000.0) + dblB;

                    dblTemp = (dblWave3dBMax2 + dblWave3dBMin1 - dblWave3dBMin2 - dblWave3dBMax1) / 2.0;
                    pstResultData.m_dblPDW[dwChannelIndex] =Math .Round ( Math.Abs(dblTemp),3);

                    // for shift
                    pstResultData.m_dblShift[dwChannelIndex] =Math.Round ( pstResultData.m_dblCW[dwChannelIndex] - dblCW,3);

                    // for IL Min
                    dwTempData = (((0.8 * pstCriteria.m_dblILLossWindow) / 100.0) * 1000.0);

                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0 - dwTempData);

                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwStartIndex, true);

                    if (dwStartIndex <= 2)
                        dwStartIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblWL = pdwWaveArray[dwStartIndex - 2 + i] / 1000.0;
                        dblX[i] = dblWL;

                        dblIL = pdwMinLossArray[dwStartIndex - 2 + i] / 1000.0;
                        dblY[i] = -1.0 * dblIL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblWinNeg = dblA * (dwTargetData / 1000.0) + dblB;
                    dblWinNeg = -1.0 * dblWinNeg;

                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0 + dwTempData);
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwEndIndex, true);

                    if (dwEndIndex <= 2)
                        dwEndIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblWL = pdwWaveArray[dwEndIndex - 2 + i] / 1000.0;
                        dblX[i] = dblWL;

                        dblIL = pdwMinLossArray[dwEndIndex - 2 + i] / 1000.0;
                        dblY[i] = -1.0 * dblIL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblWinPos = dblA * (dwTargetData / 1000.0) + dblB;
                    dblWinPos = -1.0 * dblWinPos;
                    dwMaxIndex = 0;
                    bFunctionOK = GetMaxMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex, ref dwMaxIndex);
                    dwMinLossMin[dwChannelIndex] = pdwMinLossArray[dwMinIndex];
                    if (dwMinLossMin[dwChannelIndex] > dblWinPos * 1000.0)
                        dwMinLossMin[dwChannelIndex] = dblWinPos * 1000.0;

                    if (dwMinLossMin[dwChannelIndex] > dblWinNeg * 1000.0)
                        dwMinLossMin[dwChannelIndex] = dblWinNeg * 1000.0;

                    dwMinLossMax[dwChannelIndex] = pdwMinLossArray[dwMaxIndex];
                    if (dwMinLossMax[dwChannelIndex] < dblWinPos * 1000.0)
                        dwMinLossMax[dwChannelIndex] = dblWinPos * 1000.0;

                    if (dwMinLossMax[dwChannelIndex] < dblWinNeg * 1000.0)
                        dwMinLossMax[dwChannelIndex] = dblWinNeg * 1000.0;

                    bFunctionOK = GetMaxMinIndex(pdwMaxLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex, ref dwMaxIndex);

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblWL = pdwWaveArray[dwStartIndex - 2 + i] / 1000.0;
                        dblX[i] = dblWL;

                        dblIL = pdwMaxLossArray[dwStartIndex - 2 + i] / 1000.0;
                        dblY[i] = -1.0 * dblIL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;

                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0 - dwTempData);

                    dblWinNeg = dblA * (dwTargetData / 1000.0) + dblB;
                    dblWinNeg = -1.0 * dblWinNeg;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblWL = pdwWaveArray[dwEndIndex - 2 + i] / 1000.0;
                        dblX[i] = dblWL;

                        dblIL = pdwMaxLossArray[dwEndIndex - 2 + i] / 1000.0;
                        dblY[i] = -1.0 * dblIL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;

                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0 + dwTempData);

                    dblWinPos = dblA * (dwTargetData / 1000.0) + dblB;
                    dblWinPos = -1.0 * dblWinPos;

                    dwMaxLossMin[dwChannelIndex] = pdwMaxLossArray[dwMinIndex];
                    if (dwMaxLossMin[dwChannelIndex] > dblWinPos * 1000.0)
                        dwMaxLossMin[dwChannelIndex] = dblWinPos * 1000.0;

                    if (dwMaxLossMin[dwChannelIndex] > dblWinNeg * 1000.0)
                        dwMaxLossMin[dwChannelIndex] = dblWinNeg * 1000.0;

                    dwMaxLossMax[dwChannelIndex] = pdwMaxLossArray[dwMaxIndex];
                    if (dwMaxLossMax[dwChannelIndex] < dblWinPos * 1000.0)
                        dwMaxLossMax[dwChannelIndex] = dblWinPos * 1000.0;

                    if (dwMaxLossMax[dwChannelIndex] < dblWinNeg * 1000.0)
                        dwMaxLossMax[dwChannelIndex] = dblWinNeg * 1000.0;

                    lTemp = dwMinLossMin[dwChannelIndex];

                    if (pstCriteria.m_bAlignIL)
                        pstResultData.m_dblILMin[dwChannelIndex] =Math.Round ( lTemp / 1000.0 - pstCriteria.m_dblAlignIL,2);
                    else
                        pstResultData.m_dblILMin[dwChannelIndex] =Math .Round ( lTemp / 1000.0,2);

                    // for IL Max		
                    lTemp = dwMaxLossMax[dwChannelIndex];

                    if (pstCriteria.m_bAlignIL)
                        pstResultData.m_dblILMax[dwChannelIndex] =Math.Round ( lTemp / 1000.0 - pstCriteria.m_dblAlignIL,2);
                    else
                        pstResultData.m_dblILMax[dwChannelIndex] =Math.Round ( lTemp / 1000.0,2);

                    // for IL@ITU
                    dwTargetData = pstResultData.m_dblCW[dwChannelIndex] * 1000.0;
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwTargetIndex, true);

                    dwILRight = pdwMaxLossArray[dwTargetIndex];

                    lTemp = dwILRight;

                    if (pstCriteria.m_bAlignIL)
                        pstResultData.m_dblILITU[dwChannelIndex] =Math.Round ( lTemp / 1000.0 - pstCriteria.m_dblAlignIL,2);
                    else
                        pstResultData.m_dblILITU[dwChannelIndex] =Math.Round ( lTemp / 1000.0,2);

                    // for Ripple
                    pstResultData.m_dblRipple[dwChannelIndex] =Math.Round ( (dwMaxLossMax[dwChannelIndex] - dwMinLossMin[dwChannelIndex]) / 1000.0,2);

                    // for PDL ITU
                    dwTargetData = Math.Truncate(dwCW - 1.402 * 1000.0);
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwTargetIndex, true);

                    dwILRight = pdwMaxLossArray[dwTargetIndex];
                    dwILLeft = pdwMinLossArray[dwTargetIndex];

                    lTemp = dwILRight - dwILLeft;

                    pstResultData.m_dblPDLITU[dwChannelIndex] =Math.Round ( lTemp / 1000.0,2);

                    pstResultData.m_dblPDLITU[dwChannelIndex] =Math.Round ( Math.Abs(pstResultData.m_dblPDLITU[dwChannelIndex]),2);

                    // for PDL CRT
                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0);
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwTargetIndex, true);

                    dwILRight = pdwMaxLossArray[dwTargetIndex];
                    dwILLeft = pdwMinLossArray[dwTargetIndex];

                    lTemp = dwILRight - dwILLeft;

                    pstResultData.m_dblPDLCRT[dwChannelIndex] =Math .Round ( lTemp / 1000.0,2);

                    pstResultData.m_dblPDLCRT[dwChannelIndex] =Math .Round ( Math.Abs(pstResultData.m_dblPDLCRT[dwChannelIndex]),2);

                    // for PDL Max
                    dwTempData = ((0.8 * pstCriteria.m_dblILLossWindow) / 100.0) * 1000.0;

                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0 - dwTempData);
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwStartIndex, true);

                    dwTargetData = Math.Truncate(pstResultData.m_dblCW[dwChannelIndex] * 1000.0 + dwTempData);
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwEndIndex, true);

                    lTemp = 0;
                    for (dwIndex = dwStartIndex; dwIndex < dwEndIndex + 1; dwIndex++)
                    {
                        dwILMinLeft = pdwMinLossArray[dwIndex];
                        dwILMaxLeft = pdwMaxLossArray[dwIndex];

                        if ((dwILMaxLeft - dwILMinLeft) > lTemp)
                            lTemp = dwILMaxLeft - dwILMinLeft;
                    }

                    pstResultData.m_dblPDLMax[dwChannelIndex] =Math .Round ( lTemp / 1000.0,2);

                    pstResultData.m_dblPDLMax[dwChannelIndex] =Math .Round ( Math.Abs(pstResultData.m_dblPDLMax[dwChannelIndex]),2);

                    // for BW 0.5dB
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;

                    bFunctionOK = GetMinIndex(pdwMaxLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    dwTargetData = Math.Truncate(dwILMin);
                    dwTargetData += 500;

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                    if (dwTargetIndex <= 4)
                        dwTargetIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 500) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinLeft = dblTemp1 * 1000.0;

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                    if (dwTargetIndex <= 2)
                        dwTargetIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 500) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinRight = dblTemp1 * 1000.0;

                    lTemp = dwWLMinRight - dwWLMinLeft;
                    dblTemp = lTemp / 1000.0;
                    pstResultData.m_dblBW05dB[dwChannelIndex] =Math .Round ( dblTemp,3);

                    // for BW 1dB
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;

                    bFunctionOK = GetMinIndex(pdwMaxLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    dwTargetData = dwILMin;
                    dwTargetData += 1000;

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                    if (dwTargetIndex <= 4)
                        dwTargetIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 1000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinLeft = dblTemp1 * 1000.0;

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                    if (dwTargetIndex <= 2)
                        dwTargetIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 1000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinRight = dblTemp1 * 1000.0;

                    lTemp = dwWLMinRight - dwWLMinLeft;
                    dblTemp = lTemp / 1000.0;
                    pstResultData.m_dblBW1dB[dwChannelIndex] = Math.Round(dblTemp, 3);


                    // for BW 3dB
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;

                    bFunctionOK = GetMinIndex(pdwMaxLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    dwTargetData = dwILMin;
                    dwTargetData += 3000;

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                    if (dwTargetIndex <= 4)
                        dwTargetIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 3000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinLeft = dblTemp1 * 1000.0;

                    bFunctionOK = GetDataIndex(pdwMaxLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                    if (dwTargetIndex <= 2)
                        dwTargetIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMaxLossArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 3000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinRight = dblTemp1 * 1000.0;

                    lTemp = dwWLMinRight - dwWLMinLeft;
                    dblTemp = lTemp / 1000.0;
                    pstResultData.m_dblBW3dB[dwChannelIndex] = Math.Round(dblTemp, 3);

                    // for BW 20dB
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;

                    bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    dwTargetData = dwILMin;
                    dwTargetData += 20000;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                    if (dwTargetIndex <= 4)
                        dwTargetIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 20000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinLeft = dblTemp1 * 1000.0;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                    if (dwTargetIndex <= 2)
                        dwTargetIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 20000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinRight = dblTemp1 * 1000.0;

                    lTemp = dwWLMinRight - dwWLMinLeft;
                    dblTemp = lTemp / 1000.0;
                    pstResultData.m_dblBW20dB[dwChannelIndex] = Math.Round(dblTemp, 3);

                    // for BW 25dB
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;

                    bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    dwTargetData = dwILMin;
                    dwTargetData += 25000;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                    if (dwTargetIndex <= 4)
                        dwTargetIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 25000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinLeft = dblTemp1 * 1000.0;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                    if (dwTargetIndex <= 2)
                        dwTargetIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 25000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinRight = dblTemp1 * 1000.0;

                    lTemp = dwWLMinRight - dwWLMinLeft;
                    dblTemp = lTemp / 1000.0;
                    pstResultData.m_dblBW25dB[dwChannelIndex] = Math.Round(dblTemp, 3);

                    // for BW 30dB
                    dwStartIndex = 0x00;
                    dwEndIndex = dwSamplePoint - 1;

                    bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                    dwTargetData = dwILMin;
                    dwTargetData += 30000;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                    if (dwTargetIndex <= 4)
                        dwTargetIndex = 4;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 30000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinLeft = dblTemp1 * 1000.0;

                    bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                    if (dwTargetIndex <= 2)
                        dwTargetIndex = 2;

                    dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                    for (int i = 0x00; i < 8; i++)
                    {
                        dblIL = pdwMinLossArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblX[i] = -1.0 * dblIL;

                        dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                        dblY[i] = dblWL;

                        dblK1 += dblX[i];
                        dblK2 += dblX[i] * dblX[i];
                        dblK3 += dblY[i];
                        dblK4 += dblX[i] * dblY[i];
                    }

                    dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                    dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                    dblA = dblTemp1 / dblTemp2;

                    dblB = (dblK3 - dblA * dblK1) / 8.0;
                    dblTemp1 = -1.0 * dblA * ((dwILMin + 30000) / 1000.0);
                    dblTemp1 += dblB;

                    dwWLMinRight = dblTemp1 * 1000.0;

                    lTemp = dwWLMinRight - dwWLMinLeft;
                    dblTemp = lTemp / 1000.0;
                    pstResultData.m_dblBW30dB[dwChannelIndex] = Math.Round(dblTemp, 3);


                    // for BW Cust1
                    if (pstCriteria.m_bBWCust1)
                    {
                        dwStartIndex = 0x00;
                        dwEndIndex = dwSamplePoint - 1;

                        bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                        dwTempData = pstCriteria.m_dblBWCust1Value * 1000.0;
                        dwTargetData = dwILMin;
                        dwTargetData += dwTempData;

                        bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                        if (dwTargetIndex <= 4)
                            dwTargetIndex = 4;

                        dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                        for (int i = 0x00; i < 8; i++)
                        {
                            dblIL = pdwMinLossArray[dwTargetIndex - 4 + i] / 1000.0;
                            dblX[i] = -1.0 * dblIL;

                            dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                            dblY[i] = dblWL;

                            dblK1 += dblX[i];
                            dblK2 += dblX[i] * dblX[i];
                            dblK3 += dblY[i];
                            dblK4 += dblX[i] * dblY[i];
                        }

                        dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                        dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                        dblA = dblTemp1 / dblTemp2;

                        dblB = (dblK3 - dblA * dblK1) / 8.0;
                        dblTemp1 = -1.0 * dblA * ((dwILMin + dwTempData) / 1000.0);
                        dblTemp1 += dblB;

                        dwWLMinLeft = dblTemp1 * 1000.0;

                        bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                        if (dwTargetIndex <= 2)
                            dwTargetIndex = 2;

                        dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                        for (int i = 0x00; i < 8; i++)
                        {
                            dblIL = pdwMinLossArray[dwTargetIndex - 2 + i] / 1000.0;
                            dblX[i] = -1.0 * dblIL;

                            dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                            dblY[i] = dblWL;

                            dblK1 += dblX[i];
                            dblK2 += dblX[i] * dblX[i];
                            dblK3 += dblY[i];
                            dblK4 += dblX[i] * dblY[i];
                        }

                        dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                        dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                        dblA = dblTemp1 / dblTemp2;

                        dblB = (dblK3 - dblA * dblK1) / 8.0;
                        dblTemp1 = -1.0 * dblA * ((dwILMin + dwTempData) / 1000.0);
                        dblTemp1 += dblB;

                        dwWLMinRight = dblTemp1 * 1000.0;

                        lTemp = dwWLMinRight - dwWLMinLeft;
                        dblTemp = lTemp / 1000.0;
                        pstResultData.m_dblBWCust1[dwChannelIndex] = Math.Round(dblTemp, 3);
                    }

                    if (pstCriteria.m_bBWCust2)
                    {
                        dwStartIndex = 0x00;
                        dwEndIndex = dwSamplePoint - 1;

                        bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                        dwTempData = (pstCriteria.m_dblBWCust2Value * 1000.0);
                        dwTargetData = dwILMin;
                        dwTargetData += dwTempData;

                        bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwStartIndex, dwMinIndex, ref dwTargetIndex, true);

                        if (dwTargetIndex <= 4)
                            dwTargetIndex = 4;

                        dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                        for (int i = 0x00; i < 8; i++)
                        {
                            dblIL = pdwMinLossArray[dwTargetIndex - 4 + i] / 1000.0;
                            dblX[i] = -1.0 * dblIL;

                            dblWL = pdwWaveArray[dwTargetIndex - 4 + i] / 1000.0;
                            dblY[i] = dblWL;

                            dblK1 += dblX[i];
                            dblK2 += dblX[i] * dblX[i];
                            dblK3 += dblY[i];
                            dblK4 += dblX[i] * dblY[i];
                        }

                        dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                        dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                        dblA = dblTemp1 / dblTemp2;

                        dblB = (dblK3 - dblA * dblK1) / 8.0;
                        dblTemp1 = -1.0 * dblA * ((dwILMin + dwTempData) / 1000.0);
                        dblTemp1 += dblB;

                        dwWLMinLeft = dblTemp1 * 1000.0;

                        bFunctionOK = GetDataIndex(pdwMinLossArray, dwTargetData, dwMinIndex, dwEndIndex, ref dwTargetIndex, false);

                        if (dwTargetIndex <= 2)
                            dwTargetIndex = 2;

                        dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                        for (int i = 0x00; i < 8; i++)
                        {
                            dblIL = pdwMinLossArray[dwTargetIndex - 2 + i] / 1000.0;
                            dblX[i] = -1.0 * dblIL;

                            dblWL = pdwWaveArray[dwTargetIndex - 2 + i] / 1000.0;
                            dblY[i] = dblWL;

                            dblK1 += dblX[i];
                            dblK2 += dblX[i] * dblX[i];
                            dblK3 += dblY[i];
                            dblK4 += dblX[i] * dblY[i];
                        }

                        dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                        dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                        dblA = dblTemp1 / dblTemp2;

                        dblB = (dblK3 - dblA * dblK1) / 8.0;
                        dblTemp1 = -1.0 * dblA * ((dwILMin + dwTempData) / 1000.0);
                        dblTemp1 += dblB;

                        dwWLMinRight = dblTemp1 * 1000.0;

                        lTemp = dwWLMinRight - dwWLMinLeft;
                        dblTemp = lTemp / 1000.0;
                        pstResultData.m_dblBWCust2[dwChannelIndex] = Math.Round(dblTemp,3);
                    }
                }
                #endregion

                for (dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
                {
                    // for CrossTalk calculate
                    for (int i = 0; i < dwSamplePoint; i++)
                    {
                        pdwLossArray[i] = pstAutoWaveform.m_pdwLossArray[dwChannelIndex, i];
                        pdwPDLArray[i] = pstAutoWaveform.m_pdwPDLArray[dwChannelIndex, i];
                    }
                    if (pdwLossArray == null)
                        return false;

                    if (pdwPDLArray == null)
                        return false;

                    // for IL Min and IL Max Array
                    for (dwIndex = 0x00; dwIndex < dwSamplePoint; dwIndex++)
                    {
                        dblILave = pdwLossArray[dwIndex] / 1000.0;

                        dblPDL = pdwPDLArray[dwIndex] / 1000.0;

                        dblb = Math.Pow(10, dblILave / (-10.0));

                        dblk = Math.Pow(10, dblPDL / 10.0);

                        dblTemp = 2.0 * dblb / (1 + dblk);
                        dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                        pdwMaxLossArray[dwIndex] = dblTemp * 1000.0;

                        dblTemp = 2.0 * dblk * dblb / (1 + dblk);
                        dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                        pdwMinLossArray[dwIndex] = dblTemp * 1000.0;

                        pdwAverageArray[dwIndex] = (pdwMinLossArray[dwIndex] + pdwMaxLossArray[dwIndex]) / 2.0;
                    }

                    if (dwChannelIndex == 2)
                        dwChannelIndex = 2;

                    for (dwITUIndex = dwStartChannel; dwITUIndex < dwEndChannel; dwITUIndex++)
                    {
                        if (dwITUIndex == dwChannelIndex)
                        {
                            dblCrossTalk[dwITUIndex] = 0.0;
                            continue;
                        }

                        strChannelName = pstCriteria.m_strChannelName[dwITUIndex];

                        dblCW =ConvertITUNameToWL(strChannelName);

                        dwCW = pstResultData.m_dblCW[dwITUIndex] * 1000.0;

                        dwTempData = (((0.8 * pstCriteria.m_dblILLossWindow) / 100.0) * 1000.0);
                        dwStartIndex = 0;
                        dwEndIndex = 0;
                        dwTargetData = dwCW - dwTempData;
                        bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwStartIndex, true);

                        dwTargetData = dwCW + dwTempData;
                        bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwEndIndex, true);

                        if (dwStartIndex <= 2)
                            dwStartIndex = 2;

                        dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                        for (int i = 0x00; i < 8; i++)
                        {
                            dblWL = pdwWaveArray[dwStartIndex - 2 + i] / 1000.0;
                            dblX[i] = dblWL;

                            dblIL = pdwMinLossArray[dwStartIndex - 2 + i] / 1000.0;
                            dblY[i] = -1.0 * dblIL;

                            dblK1 += dblX[i];
                            dblK2 += dblX[i] * dblX[i];
                            dblK3 += dblY[i];
                            dblK4 += dblX[i] * dblY[i];
                        }

                        dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                        dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                        dblA = dblTemp1 / dblTemp2;

                        dblB = (dblK3 - dblA * dblK1) / 8.0;
                        dblWinNeg = dblA * ((dwCW - dwTempData) / 1000.0) + dblB;
                        dblWinNeg = -1.0 * dblWinNeg;

                        if (dwEndIndex <= 2)
                            dwEndIndex = 2;

                        dblK1 = dblK2 = dblK3 = dblK4 = dblK5 = dblK6 = dblK7 = 0.0;
                        for (int i = 0x00; i < 8; i++)
                        {
                            dblWL = pdwWaveArray[dwEndIndex - 2 + i] / 1000.0;
                            dblX[i] = dblWL;

                            dblIL = pdwMinLossArray[dwEndIndex - 2 + i] / 1000.0;
                            dblY[i] = -1.0 * dblIL;

                            dblK1 += dblX[i];
                            dblK2 += dblX[i] * dblX[i];
                            dblK3 += dblY[i];
                            dblK4 += dblX[i] * dblY[i];
                        }

                        dblTemp1 = 8.0 * dblK4 - dblK1 * dblK3;
                        dblTemp2 = 8.0 * dblK2 - dblK1 * dblK1;
                        dblA = dblTemp1 / dblTemp2;

                        dblB = (dblK3 - dblA * dblK1) / 8.0;
                        dblWinPos = dblA * ((dwCW + dwTempData) / 1000.0) + dblB;
                        dblWinPos = -1.0 * dblWinPos;
                        dwMinIndex = 0;
                        bFunctionOK = GetMinIndex(pdwMinLossArray, dwStartIndex, dwEndIndex, ref dwMinIndex);

                        if (pdwMinLossArray[dwMinIndex] > dblWinNeg * 1000.0)
                            pdwMinLossArray[dwMinIndex] = dblWinNeg * 1000.0;

                        if (pdwMinLossArray[dwMinIndex] > dblWinPos * 1000.0)
                            pdwMinLossArray[dwMinIndex] = dblWinPos * 1000.0;

                        lTemp = pdwMinLossArray[dwMinIndex];

                        dblCrossTalk[dwITUIndex] = lTemp / 1000.0;
                    }

                    // for AX-
                    if (dwChannelIndex == 0x00)
                        pstResultData.m_dblAXLeft[dwChannelIndex] = 0.0;
                    else
                    {
                        if (pstCriteria.m_bAlignIL)
                            pstResultData.m_dblAXLeft[dwChannelIndex] =Math.Round ( dblCrossTalk[dwChannelIndex - 1] -
                                             dwMaxLossMax[dwChannelIndex] / 1000.0,2);
                        else
                            pstResultData.m_dblAXLeft[dwChannelIndex] = Math.Round(dblCrossTalk[dwChannelIndex - 1] -
                                             dwMaxLossMax[dwChannelIndex] / 1000.0, 2);
                    }

                    // for AX+
                    if (dwChannelIndex == dwEndChannel - 1)
                        pstResultData.m_dblAXRight[dwChannelIndex] = 0.0;
                    else
                    {
                        if (pstCriteria.m_bAlignIL)
                            pstResultData.m_dblAXRight[dwChannelIndex] = Math.Round(dblCrossTalk[dwChannelIndex + 1] -
                                             dwMaxLossMax[dwChannelIndex] / 1000.0, 2);
                        else
                            pstResultData.m_dblAXRight[dwChannelIndex] = Math.Round(dblCrossTalk[dwChannelIndex + 1] -
                                             dwMaxLossMax[dwChannelIndex] / 1000.0, 2);
                    }

                    // for TAX
                    dblTemp1 = Math.Pow(10, pstResultData.m_dblAXLeft[dwChannelIndex] / 10.0);
                    dblTemp2 = Math.Pow(10, pstResultData.m_dblAXRight[dwChannelIndex] / 10.0);

                    dblTemp = dblTemp1 + dblTemp2;

                    pstResultData.m_dblTAX[dwChannelIndex] =Math .Round ( 10.0 * Math.Log10(dblTemp),2);

                // for NX
                double dblNX = 10000000.0;

                    if (dwChannelIndex == 2)
                        dwChannelIndex = 2;

                    for (dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                    {
                        if (dwITUIndex < dwChannelIndex && (dwChannelIndex - dwITUIndex) < 2)
                            continue;

                        if (dwITUIndex >= dwChannelIndex && (dwITUIndex - dwChannelIndex) < 2)
                            continue;

                        if ((dblCrossTalk[dwITUIndex] - dwMaxLossMax[dwChannelIndex] / 1000.0) < dblNX)
                            dblNX = dblCrossTalk[dwITUIndex] - dwMaxLossMax[dwChannelIndex] / 1000.0;
                    }

                    if (pstCriteria.m_bAlignIL)
                        pstResultData.m_dblNX[dwChannelIndex] =Math .Round ( dblNX,2);
                    else
                        pstResultData.m_dblNX[dwChannelIndex] =Math .Round ( dblNX,2);

                // for TX
                double dblTX = 0.0;

                    for (dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                    {
                        if (dwITUIndex == dwChannelIndex)
                            continue;

                        dblTX += Math.Pow(10, ((-1) * (dblCrossTalk[dwITUIndex] - dwMaxLossMax[dwChannelIndex] / 1000.0)) / 10.0);
                    }

                    dblTX = Math.Abs(10.0 * Math.Log10(dblTX));

                    pstResultData.m_dblTX[dwChannelIndex] =Math.Round ( dblTX,2);

                    // for TX - AX
                    double dblTXAX = 0.0;

                    for (dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                    {
                        if (dwITUIndex < dwChannelIndex && (dwChannelIndex - dwITUIndex) < 2)
                            continue;

                        if (dwITUIndex >= dwChannelIndex && (dwITUIndex - dwChannelIndex) < 2)
                            continue;

                        dblTXAX += Math.Pow(10, ((-1) * (dblCrossTalk[dwITUIndex] - dwMaxLossMax[dwChannelIndex] / 1000.0)) / 10.0);
                    }

                    dblTXAX = Math.Abs(10.0 * Math.Log10(dblTXAX));

                    pstResultData.m_dblTXAX[dwChannelIndex] =Math.Round ( dblTXAX,2);
                }

                // for TNX
                for (dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
                {
                    dblTemp = 0.0;
                    for (dwIndex = dwStartChannel; dwIndex < dwEndChannel; dwIndex++)
                    {
                        if (dwIndex == dwChannelIndex)
                            continue;

                        dblTemp += Math.Pow(10, pstResultData.m_dblNX[dwChannelIndex] / 10.0);
                    }

                    pstResultData.m_dblTNX[dwChannelIndex] =Math.Round ( 10.0 * Math.Log10(dblTemp),2);
                }

                double dblILMaxMin = 100.0, dblILMaxMax = -100.0;
                for (dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
                {
                    for (int i = 0; i < dwSamplePoint; i++)
                    {
                        pdwLossArray[i] = pstAutoWaveform.m_pdwLossArray[dwChannelIndex, i];
                        pdwPDLArray[i] = pstAutoWaveform.m_pdwPDLArray[dwChannelIndex, i];
                    }
                    if (pdwLossArray == null)
                        return false;

                    if (pdwPDLArray == null)
                        return false;

                    // for IL Min and IL Max Array
                    for (dwIndex = 0x00; dwIndex < dwSamplePoint; dwIndex++)
                    {
                        dblILave = pdwLossArray[dwIndex] / 1000.0;

                        dblPDL = pdwPDLArray[dwIndex] / 1000.0;

                        dblb = Math.Pow(10, dblILave / (-10.0));

                        dblk = Math.Pow(10, dblPDL / 10.0);

                        dblTemp = 2.0 * dblb / (1 + dblk);
                        dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                        pdwMaxLossArray[dwIndex] = dblTemp * 1000.0;

                        dblTemp = 2.0 * dblk * dblb / (1 + dblk);
                        dblTemp = -1.0 * 10 * Math.Log10(dblTemp);
                        pdwMinLossArray[dwIndex] = dblTemp * 1000.0;

                        pdwAverageArray[dwIndex] = (pdwMinLossArray[dwIndex] + pdwMaxLossArray[dwIndex]) / 2.0;
                    }

                    // for IL@ITU
                    dwTargetIndex = 0;
                    dwTargetData = Math.Truncate((pstCriteria.m_dblItuWL[dwChannelIndex] + pstResultData.m_dblShift[5]) * 1000.0);
                    bFunctionOK = GetDataIndex(pdwWaveArray, dwTargetData, 0, dwSamplePoint - 1, ref dwTargetIndex, true);

                    dwILRight = pdwMaxLossArray[dwTargetIndex];

                    lTemp = dwILRight;

                    if (pstCriteria.m_bAlignIL)
                        pstResultData.m_dblILITU[dwChannelIndex] =Math.Round ( lTemp / 1000.0 - pstCriteria.m_dblAlignIL,2);
                    else
                        pstResultData.m_dblILITU[dwChannelIndex] =Math.Round ( lTemp / 1000.0,2);

                    // for PDL ITU
                    dwILRight = pdwMaxLossArray[dwTargetIndex];
                    dwILLeft = pdwMinLossArray[dwTargetIndex];

                    lTemp = dwILRight - dwILLeft;

                    pstResultData.m_dblPDLITU[dwChannelIndex] =Math.Round ( lTemp / 1000.0,2);

                    pstResultData.m_dblPDLITU[dwChannelIndex] =Math .Round ( Math.Abs(pstResultData.m_dblPDLITU[dwChannelIndex]),2);

                    dblTemp = pstResultData.m_dblILMax[dwChannelIndex];

                    if (dblTemp <= dblILMaxMin)
                        dblILMaxMin = dblTemp;

                    if (dblTemp > dblILMaxMax)
                        dblILMaxMax = dblTemp;
                }
                pstResultData.m_dblUniformity =Math .Round ( dblILMaxMax - dblILMaxMin,3);
            }
            catch (Exception ex)
            {
                string mm = ex.ToString();
            }

            return true;
        }

        public bool ReadRefRawData(ref tagAutoWaveform pstAutoWaveform, int dwStartChannel, int dwEndChannel)
        {
            m_pdwRef = new double[1];
            int dwIndex = 0;
            double dblTemp;
            try
            {
                for (int dwChannelIndex = 0x00; dwChannelIndex < CHANNEL_COUNT; dwChannelIndex++)
                {
                    string strFilePathName = Directory.GetCurrentDirectory() + string.Format("\\Reference\\Ref_CH{0}.csv", dwChannelIndex + 1);
                    using (CsvReader reader = new CsvReader())
                    {
                        int lineNbr = 0;

                        reader.OpenFile(strFilePathName);
                        string[] lineElems;
                        // read the 1st line
                        lineElems = reader.GetLine();
                        lineNbr++;
                        lineElems = reader.GetLine();
                        int lineElemLen = lineElems.Length;

                        do
                        {
                            lineNbr++;
                            try
                            {
                                dblTemp = double.Parse(lineElems[1]);

                                m_pdwRef[dwIndex] = dblTemp;

                                dwIndex++;

                                Array.Resize(ref m_pdwRef, dwIndex + 1);
                                //if (dwIndex >= m_dwSamplePoint)
                                //    break;

                            }
                            catch (Exception er)
                            {
                                // rethrow exception with line and file info
                                string errMsg = String.Format("Invalid line in  file: '{0}', line {1}",
                                    strFilePathName, lineNbr);
                                throw new Exception(errMsg, er);
                            }
                        }
                        while ((lineElems = reader.GetLine()) != null);
                    }
                }

                //int dwSampleCount;
                //double mm = (m_dwStopWavelength * 1.0 - m_dwStartWavelength * 1.0) / m_dblStep + 1;
                //mm = Math.Ceiling(mm);
                //dwSampleCount = 6668;// int.Parse(mm.ToString());
                //dwSampleCount =int.Parse (((m_dwStopWavelength * 1.0 - m_dwStartWavelength * 1.0) / m_dblStep + 1).ToString ());
                pstAutoWaveform.m_dwSampleCount = m_dwSamplePoint;
                //pstAutoWaveform.m_dwStep = m_dblStep;

                //double[,] pdwPwrData = new double[CHANNEL_COUNT, dwSampleCount];
                //double[,] pdwPDLData = new double[CHANNEL_COUNT, dwSampleCount];

                for (dwIndex = 0x00; dwIndex < m_dwSamplePoint; dwIndex++)
                {
                    for (int dwChannelIndex = 0x00; dwChannelIndex < dwEndChannel - dwStartChannel; dwChannelIndex++)
                    {
                        pstAutoWaveform.m_pdwLossArray[dwChannelIndex, dwIndex] = m_pdwPwr[dwChannelIndex, dwIndex] -
                            m_pdwRef[dwIndex + dwChannelIndex * m_dwSamplePoint];

                        pstAutoWaveform.m_pdwPDLArray[dwChannelIndex, dwIndex] = m_pdwPDL[dwChannelIndex, dwIndex];
                    }
                }
                for (int i = 0x00; i < m_dwSamplePoint; i++)
                {
                    pstAutoWaveform.m_pdwWavelengthArray[i] = m_pdwWave[i];
                }
            }
            catch (Exception ex)
            {
                frmAWGClient.ShowMsg(ex.ToString(),false);
            }
            return true;
        }
        //public bool RetrieveData(ref tagAutoWaveform pstAutoWaveform, int dwStartChannel, int dwEndChannel)
        //{
        //    int dwSampleCount, dwIndex, dwChannelIndex;
        //    double mm = (m_dwStopWavelength * 1.0 - m_dwStartWavelength * 1.0) / m_dblStep + 1;
        //    mm = Math.Ceiling(mm);
        //    dwSampleCount = int.Parse(mm.ToString());
        //    //dwSampleCount =int.Parse (((m_dwStopWavelength * 1.0 - m_dwStartWavelength * 1.0) / m_dblStep + 1).ToString ());
        //    pstAutoWaveform.m_dwSampleCount = dwSampleCount;
        //    pstAutoWaveform.m_dwStep = m_dblStep;

        //    //double[,] pdwPwrData = new double[CHANNEL_COUNT, dwSampleCount];
        //    //double[,] pdwPDLData = new double[CHANNEL_COUNT, dwSampleCount];

        //    for (dwIndex = 0x00; dwIndex < dwSampleCount; dwIndex++)
        //    {
        //        for (dwChannelIndex = 0x00; dwChannelIndex < dwEndChannel - dwStartChannel; dwChannelIndex++)
        //        {
        //            pstAutoWaveform.m_pdwLossArray[dwChannelIndex, dwIndex] = m_pdwPwr[dwChannelIndex,dwIndex] -
        //                m_pdwRef[dwIndex + dwChannelIndex * dwSampleCount];

        //            pstAutoWaveform.m_pdwPDLArray[dwChannelIndex, dwIndex] = m_pdwPDL[dwChannelIndex, dwIndex];
        //        }
        //    }

        //    //for (dwIndex = dwStartChannel; dwIndex < dwEndChannel; dwIndex++)
        //    //{
        //    //    for (int i = 0x00; i < dwSampleCount; i++)
        //    //    {
        //    //        pstAutoWaveform.m_pdwLossArray[dwIndex, i] = pdwPwrData[dwIndex, i];

        //    //        pstAutoWaveform.m_pdwPDLArray[dwIndex, i] = pdwPDLData[dwIndex, i];
        //    //    }
        //    //}
        //    for (int i = 0x00; i < dwSampleCount; i++)
        //    {
        //        pstAutoWaveform.m_pdwWavelengthArray[i] = m_pdwWave[i];
        //    }
        //    return false;

        //}
        public double ConvertITUNameToWL(string strChannelName)
        {
            double WL = 0;
            string sPath = Directory.GetCurrentDirectory() + "\\ITU.txt";
            string[] strTemp = ReadTxTFile(sPath);
            for (int i = 1; i < strTemp.Length; i++)
            {
                if (strTemp[i].Contains(strChannelName.ToUpper()))
                {
                    string[] str = strTemp[i].Split(',');
                    WL = double.Parse(str[2]);
                    break;
                }
            }
            return WL;
        }
        private bool GetMinIndex(double[] pdwDataArray, int dwStartIndex, int dwEndIndex, ref int pdwMinIndex)
        {
            bool bFound = false;
            int iLen = dwEndIndex - dwStartIndex + 1;
            double[] Copy = new double[iLen];
            for (int i = 0; i < iLen; i++)
                Copy[i] = pdwDataArray[dwStartIndex + i];

            double Min = Copy.Min();

            for (int i = dwStartIndex; i <= dwEndIndex; i++)
            {
                if (pdwDataArray[i] == Min)
                {
                    bFound = true;
                    pdwMinIndex = i;
                    break;
                }
            }
            if (!bFound)
                pdwMinIndex = dwStartIndex;
            return bFound;
        }
        private bool GetMaxIndex(double[] pdwDataArray, int dwStartIndex, int dwEndIndex, ref int pdwMaxIndex)
        {
            bool bFound = false;
            int iLen = dwEndIndex - dwStartIndex + 1;
            double[] Copy = new double[iLen];
            for (int i = 0; i < iLen; i++)
                Copy[i] = pdwDataArray[dwStartIndex + i];
            double Min = Copy.Max();

            for (int i = dwStartIndex; i <= dwEndIndex; i++)
            {
                if (pdwDataArray[i] == Min)
                {
                    bFound = true;
                    pdwMaxIndex = i;
                    break;
                }
            }
            if (!bFound)
                pdwMaxIndex = dwStartIndex;
            return bFound;
        }

        private bool GetMaxMinIndex(double[] pdwDataArray, int dwStartIndex, int dwEndIndex, ref int pdwMinIndex, ref int pdwMaxIndex)
        {
            bool bFoundMax = false;
            bool bFoundMin = false;
            int iLen = dwEndIndex - dwStartIndex + 1;
            double[] Copy = new double[iLen];
            for (int i = 0; i < iLen; i++)
                Copy[i] = pdwDataArray[dwStartIndex + i];

            double Max = Copy.Max();
            double Min = Copy.Min();
            for (int i = dwStartIndex; i <= dwEndIndex; i++)
            {
                if (pdwDataArray[i] == Min)
                {
                    bFoundMin = true;
                    pdwMinIndex = i;
                }
                if (pdwDataArray[i] == Max)
                {
                    bFoundMax = true;
                    pdwMaxIndex = i;
                }
                if (bFoundMax && bFoundMin)
                    break;
            }
            if (!bFoundMin)
                pdwMinIndex = dwStartIndex;
            if (!bFoundMax)
                pdwMaxIndex = dwStartIndex;

            return bFoundMax && bFoundMin;
        }
        private bool GetDataIndex(double[] pdwDataArray, double dwDestData, int dwStartIndex, int dwEndIndex, ref int pdwDestIndex, bool bIncrease)
        {
            int dwDataIndex;
            double lDiff1, lDiff2;
            int iPosibleNum = 0;
            int[] dwPosibleDataIndex = new int[2];
            double dbBetwen = 0xFFFFFFF0, dbSecond = dwDestData;
            double dbDleta;
            double dbFirst;
            bool bDataFound = false;
            double[] TempArray = new double[dwEndIndex - dwStartIndex + 1];
            for (int i = dwStartIndex; i <= dwEndIndex; i++)
            {
                TempArray[i- dwStartIndex] =Math .Abs ( pdwDataArray[i] - dwDestData);
            }
            double mm = TempArray.Min();
            try
            {
                //for (int i = dwStartIndex; i <= dwEndIndex; i++)
                //{
                //    if (TempArray[i - dwStartIndex] == mm)
                //    {
                //        dwPosibleDataIndex[iPosibleNum] = i;
                //        iPosibleNum = 1;
                //    }
                //    if (TempArray[i - dwStartIndex] < dbBetwen && TempArray[i - dwStartIndex] != mm)
                //    {
                //        bDataFound = true;
                //        dwPosibleDataIndex[iPosibleNum] = i;
                //        dbBetwen = TempArray[i - dwStartIndex];
                //    }
                //}
                for (int i = dwStartIndex; i <= dwEndIndex; i++)
                {
                    dbFirst = pdwDataArray[i];
                    dbDleta = Math.Abs(dbFirst - dbSecond);

                    if (dbDleta < dbBetwen)
                    {
                        bDataFound = true;
                        dwPosibleDataIndex[iPosibleNum] = i;
                        dbBetwen = dbDleta;
                        if (iPosibleNum == 0)
                        {
                            iPosibleNum = 1;
                        }
                        else
                        {
                            iPosibleNum = 0;
                        }
                    }
                }

                if (!bDataFound)
                {
                    frmAWGClient.ShowMsg("GetDataIndex  --Measurement Curve is Error.",false);

                    throw new Exception("Measurement Curve is Error.");
                }

                if (bIncrease)
                {
                    //if we only find one most close point
                    if (dwPosibleDataIndex[1] > 253333)
                    {
                        dwDataIndex = dwPosibleDataIndex[0];
                    }
                    else
                    {
                        dwDataIndex = dwPosibleDataIndex[1] > dwPosibleDataIndex[0] ? dwPosibleDataIndex[1] : dwPosibleDataIndex[0];
                    }
                }
                else
                {
                    if (dwPosibleDataIndex[0] > 253333)
                    {
                        dwDataIndex = dwPosibleDataIndex[1];
                    }
                    else
                    {
                        dwDataIndex = dwPosibleDataIndex[1] < dwPosibleDataIndex[0] ? dwPosibleDataIndex[1] : dwPosibleDataIndex[0];
                    }
                }

                if (pdwDataArray[dwDataIndex] == dwDestData || dwDataIndex == 0)
                    pdwDestIndex = dwDataIndex;
                else
                {
                    //	Which one is closer, left or right?
                    lDiff1 = dwDestData - pdwDataArray[dwDataIndex - 1];
                    lDiff2 = pdwDataArray[dwDataIndex] - dwDestData;

                    if (Math.Abs(lDiff1) < Math.Abs(lDiff2))
                        pdwDestIndex = dwDataIndex - 1;
                    else
                        pdwDestIndex = dwDataIndex;
                }

                bDataFound = true;
            }

            catch (Exception ex)
            {
                frmAWGClient.ShowMsg("GetDataIndex  --" + ex.ToString(),false);
                pdwDestIndex = dwStartIndex;
            }
            return bDataFound;
        }

        public bool ReadRawData(string strFilePathName)
        {
            int dwChannelIndex, dwLength;
            double dblTemp;
            m_pdwWave = new double[m_dwSamplePoint];
            m_pdwPwr = new double[CHANNEL_COUNT , m_dwSamplePoint];
            m_pdwPDL = new double[CHANNEL_COUNT , m_dwSamplePoint];

            int dwIndex = 0;
            try
            {
                using (CsvReader reader = new CsvReader())
                {
                    int lineNbr = 0;

                    reader.OpenFile(strFilePathName);
                    string[] lineElems;
                    // read the 1st line
                    lineElems = reader.GetLine();
                    lineNbr++;
                    lineElems = reader.GetLine();
                    int lineElemLen = lineElems.Length;
                    if (lineElemLen <= 7)
                        return false;

                    do
                    {
                        lineNbr++;
                        try
                        {
                            //lineElems = reader.GetLine();
                            m_pdwWave[dwIndex] = double.Parse(lineElems[0]);
                            for (dwChannelIndex = 0x00; dwChannelIndex < CHANNEL_COUNT; dwChannelIndex++)
                            {
                                dblTemp = double.Parse(lineElems[1 + dwChannelIndex * 2]);
                                if (dwIndex != 0x00)
                                {
                                    if (dblTemp == 0.0)
                                    {
                                        m_pdwPwr[dwChannelIndex,dwIndex] =
                                            m_pdwPwr[dwChannelIndex, dwIndex - 1];
                                    }
                                    else if (dblTemp < 0.0)
                                        m_pdwPwr[dwChannelIndex, dwIndex] = (-1.0) * dblTemp;
                                    else
                                        m_pdwPwr[dwChannelIndex, dwIndex] = dblTemp;
                                }
                                else
                                {
                                    if (dblTemp == 0.0)
                                        m_pdwPwr[dwChannelIndex, dwIndex] = 55;
                                    else if (dblTemp < 0.0)
                                        m_pdwPwr[dwChannelIndex, dwIndex] = (-1.0) * dblTemp;
                                    else
                                        m_pdwPwr[dwChannelIndex, dwIndex] = dblTemp;
                                }
                                dblTemp = double.Parse(lineElems[2 + dwChannelIndex * 2]);

                                if (dwIndex != 0x00)
                                {
                                    if (dblTemp <= 0.0)
                                    {
                                        m_pdwPDL[dwChannelIndex, dwIndex] =
                                            m_pdwPDL[dwChannelIndex, dwIndex - 1];
                                    }
                                    else
                                        m_pdwPDL[dwChannelIndex, dwIndex] = dblTemp;
                                }
                                else
                                {
                                    if (dblTemp <= 0.0)
                                        m_pdwPDL[dwChannelIndex, dwIndex] = 1;
                                    else
                                        m_pdwPDL[dwChannelIndex, dwIndex] = dblTemp;
                                }
                            }
                            dwIndex++;
                            if (dwIndex >= m_dwSamplePoint)
                                break;
                        }
                        catch (Exception er)
                        {
                            // rethrow exception with line and file info
                            string errMsg = String.Format("Invalid line in  file: '{0}', line {1}",
                                strFilePathName, lineNbr);
                            throw new Exception(errMsg, er);
                        }
                    }
                    while ((lineElems = reader.GetLine()) != null);
                }
            }
            catch (Exception ex)
            {
                frmAWGClient.ShowMsg(ex.ToString(),false);
                return false;
            }
            return true;

        }

        public string[] ReadTxTFile(string filePath)
        {
            int iLength = 0;
            byte[] OriginalData = new byte[iLength];
            string[] temp = new string[iLength];
            UInt16[] Data = new UInt16[128];
            FileStream fs = null;
            StreamReader br = null;
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new Exception(string.Format("The file {0} does not exist !", filePath));
                }
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                br = new StreamReader(fs);
                br.BaseStream.Seek(0, SeekOrigin.Begin); //将文件指针设置到文件开始
                //iLength = fs.Length;
                int iLen = (int)iLength;
                OriginalData = new byte[iLen];
                int iCount = 0;
                string strLine = "";
                do
                {
                    iLength++;
                    strLine = br.ReadLine();
                    if (strLine != null && strLine != "")
                    {
                        Array.Resize(ref temp, iLength);
                        temp[iCount] = strLine;
                    }
                    iCount++;
                } while (strLine != null && strLine != "");
            }
            catch (Exception ex)
            {
                throw new Exception("Read Bin File Error !" + ex);
            }
            finally
            {
                if (br != null)
                {
                    try
                    {
                        br.Close();
                    }
                    catch { }
                }
                if (fs != null)
                {
                    try
                    {
                        fs.Close();
                    }
                    catch { }
                }
            }
            return temp;
        }


        public bool SaveTLSSeting(string tszSectionName, double dblStartWL, double dblStopWL, double dblPower,
                     int dwStepIndex, int dwOutputPort, int dwLLog, int dwWSIndex)
        {
            string strTmplFileName = Directory.GetCurrentDirectory() + @"\Reference\" + "AWGTLSSetting" + dwWSIndex + ".ini";
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Start WL", dblStartWL.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Stop WL", dblStopWL.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Step Size", dwStepIndex.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Output Power", dblPower.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Output Port", dwOutputPort.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Lambda Mode", dwLLog.ToString());

            return true;
        }

        public bool SaveSeting(string sFileName, string tszSectionName, string KeyValue, string Value)
        {
            INIOperationClass.INIWriteValue(sFileName, tszSectionName, KeyValue, Value);
            return true;
        }

        public string GetSeting(string sFileName, string tszSectionName, string KeyValue, string valueDefault)
        {
            string value = INIOperationClass.INIGetStringValue(sFileName, tszSectionName, KeyValue, valueDefault);
            return value;
        }




    }
}
