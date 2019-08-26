using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestSystem.TestLibrary.Utilities;
using TestSystem.TestLibrary.INI;
using System.Windows.Forms;
using TestSystem.TestLibrary.Algorithms;
using SG_Filter_Demo.RAW;
namespace AWGTestClient
{
    class AWGTestClient
    {
        public int m_dwSamplePoint;
        public double m_dwStep;
        static double[] _gpdblSweepRate = new double[13] { 0.5, 1.0, 2.0, 5.0, 10.0, 20.0, 40.0, 50.0, 80.0, 100.0, 150.0, 160.0, 200.0 };
        static double[] _gpdblWindowIL = new double[7] { 1270.0, 1310.0, 1490.0, 1550.0, 1577.0, 1625.0, 1650.0 };
      
        public int CHANNEL_COUNT = 10;
        tagAutoWaveform m_stPLCData;
        tagPLCData m_stPLCTestResultData;      
        double[] m_pdwWave;
       
        double[] m_pdwPolPwr1;
        double[] m_pdwPolPwr2;
        double[] m_pdwPolPwr3;
        double[] m_pdwPolPwr4;
        double[] m_pdwCaliPolPwr1;
        double[] m_pdwCaliPolPwr2;
        double[] m_pdwCaliPolPwr3;
        double[] m_pdwCaliPolPwr4;
     
        double[] m_pdwRef;
        int window;
    private Frm_AWGTestClient frmAWGClient;
        public AWGTestClient()
        {
        }

        public AWGTestClient(Frm_AWGTestClient frmClient, tagAutoWaveform PLCData, tagPLCData testResult)
        {
            frmAWGClient = frmClient;
            //m_stCriteria = PLCCriteria;
            m_stPLCData = PLCData;
            m_stPLCTestResultData = testResult;          
        }
        public bool SaveResultToDataBase()
        {
            bool bSuccess = true ;


            return bSuccess;
        }
        public bool CalulateILAve(tagAutoWaveform pstAutoWaveform, int dwStartChannel, int dwEndChannel, out double[,] pdwMinLossArrayTest, out double[,] pdwMaxLossArrayTest)
        {
            bool bSuccess = false;
            int dwSamplePoint = pstAutoWaveform.m_dwSampleCount;
            pdwMinLossArrayTest = new double[CHANNEL_COUNT, dwSamplePoint];
            pdwMaxLossArrayTest = new double[CHANNEL_COUNT, dwSamplePoint];
            double[] pdwLossArray = new double[dwSamplePoint];
            double[] pdwPDLArray = new double[dwSamplePoint];
       
            for (int iChannel = dwStartChannel; iChannel < dwEndChannel; iChannel++)
            {
                #region calc ILMax ILMin ILAve

                    for (int i = 0; i < dwSamplePoint; i++)
                    {
                        pdwMinLossArrayTest[iChannel, i] = pstAutoWaveform.m_pdwILMinArray[iChannel, i];
                        pdwMaxLossArrayTest[iChannel, i] = pstAutoWaveform.m_pdwILMaxArray[iChannel, i];
                    }
                    if (pdwMinLossArrayTest == null)
                        return false;
                    if (pdwMaxLossArrayTest == null)
                        return false;
               
                #endregion

                #region smooth the curve

                PointsCollection pointsILMin = new PointsCollection();
                PointsCollection pointsILMax = new PointsCollection();
                for (int index = 0; index < m_dwSamplePoint; index++)
                {
                    pointsILMin.Add(new Point(pstAutoWaveform.m_pdwWavelengthArray[index], pdwMinLossArrayTest[iChannel, index]));
                    pointsILMax.Add(new Point(pstAutoWaveform.m_pdwWavelengthArray[index], pdwMaxLossArrayTest[iChannel, index]));
                }
                List<Point> DataILMin = pointsILMin.DoSGFilter(window, 2);
                List<Point> DataILMax = pointsILMax.DoSGFilter(window, 2);
                for (int index = 0; index < m_dwSamplePoint; index++)
                {
                    pdwMinLossArrayTest[iChannel, index] = DataILMin[index].Intensity;
                    pdwMaxLossArrayTest[iChannel, index] = DataILMax[index].Intensity;
                }
                #endregion
            
            }
            bSuccess = true;
            return bSuccess;
        }
        public bool CalculateILPDL_New(ref tagPLCData pstResultData, ref tagAutoWaveform pstAutoWaveform,testConditionStruct pstCriteria, int dwStartChannel, int dwEndChannel,bool bUseITU,bool bUseTETM,bool bUseMaxMin)
        {
            bool bSuccess = false;
            int dwSamplePoint = pstAutoWaveform.m_dwSampleCount;
            //dwSamplePoint = 7501;
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
           
        //    int indexCW = 0;
            double deltaWave = 0;
               
            double C = 299792458;
            int[] iPointLossWindow = new int[2];
            try
            {
                for (int iChannel = dwStartChannel; iChannel < dwEndChannel; iChannel++)
                {
                    try
                    {
                        double dwCW = pstCriteria.iTUWL[iChannel];

                        for (int index = 0; index < m_dwSamplePoint; index++)
                        {
                            pdwMinLossArray[index] = pstAutoWaveform.m_pdwILMinArray[iChannel, index];
                            pdwMaxLossArray[index] = pstAutoWaveform.m_pdwILMaxArray[iChannel, index];
                        }
                        // CW
                        #region Calc CW

                        //double MaxILMin = Alg_PointSearch.FindMinValueInArray(pdwMaxLossArray);
                        //int MaxILIndex = Alg_PointSearch.FindFirstIndexOfMinValueInArray(pdwMaxLossArray);
                        //double MaxILWavelength = pdwWaveArray[MaxILIndex];
                        //double[] CrossPoint1 = GetCrossPointCW(pdwMaxLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);

                        //double[] CrossPoint = GetCrossPointCW(pdwMinLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);

                        
                        double CWWLStart;
                        double CWWLEnd;
                        //根据Iswave决定使用波长还是频率
                        //波长  计算CW时，采用的测试数据的起止波长分别为中心波长加减 ITUStep/ 2
                        if (pstCriteria.IsWave)
                        {
                            CWWLStart = dwCW - pstCriteria.ITUStep / 2;
                            CWWLEnd = dwCW + pstCriteria.ITUStep / 2;
                        }
                        //频率  计算CW时，采用的测试数据的起止波长分别为中心频率加减 ITUStepFreq/ 2对应的波长
                        else
                        {
                            CWWLStart = C / (C / dwCW + pstCriteria.ITUStepFreq / 2);
                            CWWLEnd = C / (C / dwCW - pstCriteria.ITUStepFreq / 2);
                        }
                        int CWStartIndex;
                        int CWEndIndex;
                        //根据起止波长计算出起止索引
                        if (CWWLStart <= pstResultData.m_dblStartWL)
                        {
                            CWStartIndex = 0;
                        }
                        else
                        {
                            CWStartIndex = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, CWWLStart);
                        }
                        if (CWWLEnd >= pstResultData.m_dblStopWL)
                        {
                            CWEndIndex = m_dwSamplePoint-1;
                        }
                        else
                        {
                            CWEndIndex = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, CWWLEnd);
                        }
                        int endIndex = CWEndIndex;
                        int startIndex = CWStartIndex;
                        double[] maxLossArrayCW = new double[CWEndIndex - CWStartIndex + 1];
                        double[] minLossArrayCW = new double[CWEndIndex - CWStartIndex + 1];
                        double[] waveArrayCW = new double[CWEndIndex - CWStartIndex + 1];
                        //根据起止索引，获取当前通道下用于计算CW的测试数据数组以及波长数组
                        for (int i = 0; i <= CWEndIndex - CWStartIndex; i++)
                        {
                            maxLossArrayCW[i] = pdwMaxLossArray[i + CWStartIndex];
                            minLossArrayCW[i] = pdwMinLossArray[i + CWStartIndex];
                            waveArrayCW[i] = pdwWaveArray[i + CWStartIndex];
                        }
                        double MaxILMin = Alg_PointSearch.FindMinValueInArray(maxLossArrayCW);
                        int MaxILIndex = Alg_PointSearch.FindFirstIndexOfMinValueInArray(maxLossArrayCW);
                        
                        double[] CrossPoint = new double[2];
                        double[] CrossPoint1 = new double[2];
                        double temp = pdwMaxLossArray[0] < pdwMinLossArray[0] ? pdwMaxLossArray[0] : pdwMinLossArray[0];
                        double subs = temp - MaxILMin;
                        //第一通道的第一个测试值与MaxILMin的差值小于3时，左边的交叉点为起点，右边的交叉点为MaxILMin往下（temp - MaxILMin）dB的交叉点
                        if (iChannel == dwStartChannel & subs < 3)
                        {
                            #region Cancel Algorithm for Aglient test scheme
                            //int indexRigtLess = Alg_PointSearch.FindLastValue(maxLossArrayCW, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);
                            //int indexRigtGreater = Alg_PointSearch.FindFirstValue(maxLossArrayCW, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);
                            //int indexRigtLessTM = Alg_PointSearch.FindLastValue(minLossArrayCW, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);
                            //int indexRigtGreaterTM = Alg_PointSearch.FindFirstValue(minLossArrayCW, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);

                            //int j = 1;

                            //while (indexRigtGreater == -1 || indexRigtLess == -1|| indexRigtGreaterTM == -1 || indexRigtLessTM == -1)
                            //{
                            //    //若遍历到整个测试数组的最后一个数还是没有交叉点，则右边的交叉点为当前遍历数组的最后一个索引endIndex - startIndex
                            //    if ( endIndex==(dwSamplePoint-1))
                            //    {
                            //        if (indexRigtGreater == -1)
                            //            indexRigtGreater = endIndex - startIndex;
                            //        if (indexRigtLess == -1)
                            //            indexRigtLess = endIndex - startIndex;
                            //        if (indexRigtGreaterTM == -1)
                            //            indexRigtGreaterTM = endIndex - startIndex;
                            //        if (indexRigtLessTM == -1)
                            //            indexRigtLessTM = endIndex - startIndex;
                            //        break;
                            //    }
                            //    endIndex +=  500 * j;

                            //    if (endIndex >= dwSamplePoint)
                            //    {
                            //        endIndex = dwSamplePoint - 1;
                            //    }
                            //    maxLossArrayCW = new double[endIndex - startIndex + 1];
                            //    minLossArrayCW = new double[endIndex - startIndex + 1];
                            //    waveArrayCW = new double[endIndex - startIndex + 1];
                            //    for (int i = 0; i <= endIndex - startIndex; i++)
                            //    {
                            //        maxLossArrayCW[i] = pdwMaxLossArray[i + startIndex];
                            //        minLossArrayCW[i] = pdwMinLossArray[i + startIndex];
                            //        waveArrayCW[i] = pdwWaveArray[i + startIndex];
                            //    }
                            //    MaxILIndex = Alg_PointSearch.FindFirstIndexOfMinValueInArray(maxLossArrayCW);
                            //    indexRigtLess = Alg_PointSearch.FindLastValue(maxLossArrayCW, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);
                            //    indexRigtGreater = Alg_PointSearch.FindFirstValue(maxLossArrayCW, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);
                            //    indexRigtLessTM = Alg_PointSearch.FindLastValue(minLossArrayCW, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);
                            //    indexRigtGreaterTM = Alg_PointSearch.FindFirstValue(minLossArrayCW, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, MaxILMin + subs, MaxILIndex + 1, maxLossArrayCW.Length - 1);

                            //    j++;
                            //}
                            //double WaveRightLess = waveArrayCW[indexRigtLess];
                            //double WaveRightGreater = waveArrayCW[indexRigtGreater];
                            //double WaveRightLessTM = waveArrayCW[indexRigtLessTM];
                            //double WaveRightGreaterTM = waveArrayCW[indexRigtGreaterTM];

                            //double IL3 = maxLossArrayCW[indexRigtLess];
                            //double IL4 = maxLossArrayCW[indexRigtGreater];
                            //double IL3TM = maxLossArrayCW[indexRigtLessTM];
                            //double IL4TM = maxLossArrayCW[indexRigtGreaterTM];

                            //double waveRightILMax = 0;
                            //double waveRightILMaxTM = 0;

                            //if (IL3 != IL4)
                            //    waveRightILMax = LinearInterpolateAlgorithm.Calculate(IL3, IL4, WaveRightLess, WaveRightGreater, MaxILMin + subs);
                            //else
                            //    waveRightILMax = WaveRightLess;
                            //if (IL3TM != IL4TM)
                            //    waveRightILMaxTM = LinearInterpolateAlgorithm.Calculate(IL3TM, IL4TM, WaveRightLess, WaveRightGreater, MaxILMin + subs);
                            //else
                            //    waveRightILMaxTM = WaveRightLessTM;

                            //CrossPoint[0] = pdwWaveArray[0];
                            //CrossPoint[1] = waveRightILMax;
                            //CrossPoint1[0] = pdwWaveArray[0];
                            //CrossPoint1[1] = waveRightILMaxTM;
                            #endregion

                            CrossPoint = GetCrossPointCW(maxLossArrayCW, waveArrayCW, MaxILMin + subs, MaxILIndex);
                            CrossPoint1 = GetCrossPointCW(minLossArrayCW, waveArrayCW, MaxILMin + subs, MaxILIndex);
                        }
                        else
                        {
                            //根据前面得到的测试数组，计算交叉点
                            CrossPoint = GetCrossPointCW(maxLossArrayCW, waveArrayCW, MaxILMin + 3, MaxILIndex);
                            CrossPoint1 = GetCrossPointCW(minLossArrayCW, waveArrayCW, MaxILMin + 3, MaxILIndex);

                            #region Cancel 若没有交叉点，则扩大测试数组的范围，一次增加400个数据，直到找到为止
                            //int j = 1;
                            //while (CrossPoint.Contains(0) || CrossPoint1.Contains(0))
                            //{
                            //    if (startIndex == 0 && endIndex == m_dwSamplePoint - 1)
                            //    {
                            //        // throw new Exception("计算CW时，没有交叉点！");
                            //        break;
                            //    }
                            //    startIndex -= 200 * j;
                            //    endIndex += 200 * j;
                            //    if (startIndex < 0)
                            //    {
                            //        startIndex = 0;
                            //    }
                            //    if (endIndex >= dwSamplePoint)
                            //    {
                            //        endIndex = dwSamplePoint - 1;
                            //    }
                            //    maxLossArrayCW = new double[endIndex - startIndex + 1];
                            //    minLossArrayCW = new double[endIndex - startIndex + 1];
                            //    waveArrayCW = new double[endIndex - startIndex + 1];
                            //    for (int i = 0; i <= endIndex - startIndex; i++)
                            //    {
                            //        maxLossArrayCW[i] = pdwMaxLossArray[i + startIndex];
                            //        minLossArrayCW[i] = pdwMinLossArray[i + startIndex];
                            //        waveArrayCW[i] = pdwWaveArray[i + startIndex];
                            //    }
                            //    MaxILIndex = Alg_PointSearch.FindFirstIndexOfMinValueInArray(maxLossArrayCW);
                            //    CrossPoint = GetCrossPointCW(maxLossArrayCW, waveArrayCW, MaxILMin + 3, MaxILIndex);

                            //    CrossPoint1 = GetCrossPointCW(minLossArrayCW, waveArrayCW, MaxILMin + 3, MaxILIndex);
                            //    j++;
                            //}
                            #endregion
                        }
                        if(CrossPoint.Contains(0) || CrossPoint1.Contains(0))
                        {
                            //frmAWGClient.ShowMsg($"通道{iChannel}在计算CW时没有交叉点！", false);
                        }
                        pstResultData.m_dblCW[iChannel] = Math.Round((CrossPoint[0] + CrossPoint[1] + CrossPoint1[0] + CrossPoint1[1]) / 4, 3);
                    //    indexCW = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, pstResultData.m_dblCW[iChannel], 0, dwSamplePoint - 1);
                        #endregion

                        //Get IL Loss Window Point
                        #region Get Loss Window point
                        if (pstCriteria.ILLossWindow > 100)
                        {
                            if (bUseITU)
                                deltaWave = C * pstCriteria.ILLossWindow / Math.Pow(C / pstCriteria.iTUWL[iChannel], 2);
                            else
                                deltaWave = C * pstCriteria.ILLossWindow / Math.Pow(C / pstResultData.m_dblCW[iChannel], 2);
                        }
                        else
                            deltaWave = pstCriteria.ILLossWindow;
                        iPointLossWindow = CalcLossWindowsPoint(pdwWaveArray, pstCriteria.ILLossWindow, pstCriteria.iTUWL[iChannel], pstResultData.m_dblCW[iChannel], dwSamplePoint, bUseITU);
                        iStartLossWindow = iPointLossWindow[0];
                        iStopLossWindow = iPointLossWindow[1];
                        #endregion

                        //Get Ripple Loss Window Point
                        #region Get Ripple Loss Window point
                        iPointLossWindow = CalcLossWindowsPoint(pdwWaveArray, pstCriteria.RippleLossWindow, pstCriteria.iTUWL[iChannel], pstResultData.m_dblCW[iChannel], dwSamplePoint, bUseITU);
                        iStartLossWindowRipple = iPointLossWindow[0];
                        iStopLossWindowRipple = iPointLossWindow[1];
                        #endregion

                        //PDW
                        #region PDW
                        //double[] ILMaxCrossPoint = GetCrossPoint(pdwMaxLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);
                        //double[] ILMinCrossPoint = GetCrossPoint(pdwMinLossArray, pdwWaveArray, MaxILMin + 3, MaxILIndex);
                        //pstResultData.m_dblPDW[iChannel] = Math.Round(((ILMinCrossPoint[1] + ILMaxCrossPoint[0]) - (ILMaxCrossPoint[1] + ILMinCrossPoint[0])) / 2, 3);
                        pstResultData.m_dblPDW[iChannel] = Math.Round(((CrossPoint1[1] + CrossPoint[0]) - (CrossPoint[1] + CrossPoint1[0])) / 2, 3);

                        #endregion

                        //Shift
                        #region Shift
                        pstResultData.m_dblShift[iChannel] = Math.Abs(Math.Round(pstResultData.m_dblCW[iChannel] - pstCriteria.iTUWL[iChannel], 3));
                        #endregion

                        //IL Min Max
                        #region IL Min Max
                        double ILMin = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iStartLossWindow, iStopLossWindow);
                        double ILMax = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindow, iStopLossWindow);
                        //dwMinLossMin[iChannel] = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iStartLossWindow, iStopLossWindow);
                        //dwMinLossMax[iChannel] = Alg_PointSearch.FindMaxValueInArray(pdwMinLossArray, iStartLossWindow, iStopLossWindow);

                        dwMaxLossMax[iChannel] = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindow, iStopLossWindow);
                        dwMaxLossMin[iChannel] = Alg_PointSearch.FindMinValueInArray(pdwMaxLossArray, iStartLossWindow, iStopLossWindow);

                       
                        pstResultData.m_dblILMin[iChannel] = Math.Round(ILMin - pstCriteria.AlignIL, 2);
                       
                        pstResultData.m_dblILMax[iChannel] = Math.Round(ILMax - pstCriteria.AlignIL, 2);
                       
                        #endregion

                        // for IL@ITU, Ripple, PDL ITU, PDL CRT, PDL Max
                        #region for IL@ITU , Ripple, PDL ITU, PDL CRT, PDL Max
                        double dwTargetData = 0;
                        dwTargetData = pstCriteria.iTUWL[iChannel];
                        iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                        pstResultData.m_dblILITU[iChannel] = Math.Round(pdwMaxLossArray[iPoint1] - pstCriteria.AlignIL, 2);
                        pstResultData.m_dbMax_at_itu[iChannel] = Math.Round(pdwMaxLossArray[iPoint1] - pstCriteria.AlignIL, 2);
                        pstResultData.m_dbMin_at_itu[iChannel] = Math.Round(pdwMinLossArray[iPoint1] - pstCriteria.AlignIL, 2);
                        iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, pstResultData.m_dblCW[iChannel]);
                        pstResultData.m_dbMax_at_cw[iChannel] = Math.Round(pdwMaxLossArray[iPoint1] - pstCriteria.AlignIL, 2);
                        pstResultData.m_dbMin_at_cw[iChannel] = Math.Round(pdwMinLossArray[iPoint1] - pstCriteria.AlignIL, 2);

                        // for Ripple
                        double ILMinRipple = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iStartLossWindowRipple, iStopLossWindowRipple);
                        double ILMaxRipple = Alg_PointSearch.FindMaxValueInArray(pdwMaxLossArray, iStartLossWindowRipple, iStopLossWindowRipple);
                        pstResultData.m_dblRipple[iChannel] = Math.Round((ILMaxRipple - ILMinRipple), 2);

                        // for PDL ITU
                        dwTargetData = pstCriteria.iTUWL[iChannel];
                        iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                        pstResultData.m_dblPDLITU[iChannel] = Math.Round((pdwMaxLossArray[iPoint1] - pdwMinLossArray[iPoint1]), 2);

                        // for PDL CRT
                        dwTargetData = Math.Truncate(pstResultData.m_dblCW[iChannel]);
                        iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                        pstResultData.m_dblPDLCRT[iChannel] = Math.Round((pdwMaxLossArray[iPoint1] - pdwMinLossArray[iPoint1]), 2);

                        // for PDL Max
                        dwTargetData = Math.Round(pstCriteria.iTUWL[iChannel] - deltaWave, 3);
                        iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);

                        dwTargetData = Math.Round(pstCriteria.iTUWL[iChannel] + deltaWave, 3);
                        iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetData);
                        double lTemp = 0;
                        if (iPoint1 > 1)
                        {
                            for (int dwIndex = iPoint1 - 1; dwIndex < iPoint2; dwIndex++)
                            {
                                double dwILMinLeft = pdwMinLossArray[dwIndex];
                                double dwILMaxLeft = pdwMaxLossArray[dwIndex];

                                if (Math.Abs(dwILMaxLeft - dwILMinLeft) > lTemp)
                                    lTemp = Math.Abs(dwILMaxLeft - dwILMinLeft);
                            }
                        }
                        pstResultData.m_dblPDLMax[iChannel] = Math.Round(lTemp, 2);
                        #endregion

                        // for BW 0.5dB 1dB 3dBm 20dB 25dB 30dB
                        #region for BW 0.5dB 1dB 3dBm 20dB 25dB 30dB

                        CrossPoint = GetCrossPoint(maxLossArrayCW, waveArrayCW, MaxILMin + 0.5, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                        pstResultData.m_dblBW05dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                        CrossPoint = GetCrossPoint(maxLossArrayCW, waveArrayCW, MaxILMin + 1, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                        pstResultData.m_dblBW1dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                        CrossPoint = GetCrossPoint(maxLossArrayCW, waveArrayCW, MaxILMin + 3, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                        pstResultData.m_dblBW3dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);

                        CrossPoint = GetCrossPointForPassBand(maxLossArrayCW, waveArrayCW, MaxILMin + 20, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                        //while (CrossPoint.Contains(0) && waveArrayCW[0] != pdwWaveArray[0] && waveArrayCW.Last() != pdwWaveArray.Last())
                        //{
                            
                        //}
                        pstResultData.m_dblBW20dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);
                        try
                        {
                            CrossPoint = GetCrossPointForPassBand(maxLossArrayCW, waveArrayCW, dwMaxLossMin[iChannel] + 25, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                            int k25 = 1;
                            while (CrossPoint.Contains(0) && waveArrayCW[0] != pdwWaveArray[0] && waveArrayCW.Last() != pdwWaveArray.Last())
                            {
                                startIndex -= 500 * k25;
                                endIndex += 500 * k25;
                                if (startIndex < 0)
                                {
                                    startIndex = 0;
                                }
                                if (endIndex > dwSamplePoint)
                                {
                                    endIndex = dwSamplePoint - 1;
                                }
                                maxLossArrayCW = new double[endIndex - startIndex + 1];

                                waveArrayCW = new double[endIndex - startIndex + 1];
                                for (int i = 0; i <= endIndex - startIndex; i++)
                                {
                                    maxLossArrayCW[i] = pdwMaxLossArray[i + startIndex];

                                    waveArrayCW[i] = pdwWaveArray[i + startIndex];
                                }
                                MaxILIndex = Alg_PointSearch.FindFirstIndexOfMinValueInArray(maxLossArrayCW);
                                CrossPoint = GetCrossPointForPassBand(maxLossArrayCW, waveArrayCW, MaxILMin + 25, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                                k25++;
                            }

                            pstResultData.m_dblBW25dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);
                        }
                        catch
                        {
                            pstResultData.m_dblBW25dB[iChannel] = -999;
                        };
                        try
                        {
                            CrossPoint = GetCrossPointForPassBand(maxLossArrayCW, waveArrayCW, dwMaxLossMin[iChannel] + 30, MaxILIndex, pstResultData.m_dblCW[iChannel]);
                            //int k30 = 1;
                            //while (CrossPoint.Contains(0) && waveArrayCW[0] != pdwWaveArray[0] && waveArrayCW.Last() != pdwWaveArray.Last())
                            //{
                            //   
                            //}
                            pstResultData.m_dblBW30dB[iChannel] = Math.Round((CrossPoint[1] - CrossPoint[0]), 3);
                        }
                        catch
                        {
                            pstResultData.m_dblBW30dB[iChannel] = -999;
                        };
                        #endregion

                       #region Cancel  CrossTalk
                        //double deltaWaveCrossTalk = 0;
                        //if (pstCriteria.CrossTalkLosWindow > 100)
                        //{
                        //    if (bUseITU)
                        //        deltaWaveCrossTalk = C * pstCriteria.CrossTalkLosWindow / Math.Pow(C / pstCriteria.iTUWL[iChannel], 2);
                        //    else
                        //        deltaWaveCrossTalk = C * pstCriteria.CrossTalkLosWindow / Math.Pow(C / pstResultData.m_dblCW[iChannel], 2);
                        //}
                        //else
                        //    deltaWaveCrossTalk = pstCriteria.CrossTalkLosWindow;
                        //for (int iCrossChannel = 0; iCrossChannel < dwEndChannel; iCrossChannel++)
                        //{
                        //    if (iChannel == iCrossChannel)
                        //    {
                        //        CrossTalk[iChannel, iCrossChannel] = 0;
                        //        continue;
                        //    }
                        //    dwCW = pstResultData.m_dblCW[iCrossChannel];

                        //    double dwTargetDataCrossTalk = pstCriteria.iTUWL[iCrossChannel] - deltaWaveCrossTalk;
                        //    iPoint1 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetDataCrossTalk);

                        //    dwTargetDataCrossTalk = pstCriteria.iTUWL[iCrossChannel] + deltaWaveCrossTalk;
                        //    iPoint2 = Alg_PointSearch.FindFirstValue(pdwWaveArray, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, dwTargetDataCrossTalk);
                        //    if (iPoint1 >= 1 && iPoint2 >= 1)
                        //    {
                        //        double cross = Alg_PointSearch.FindMinValueInArray(pdwMinLossArray, iPoint1 - 1, iPoint2);
                        //        CrossTalk[iChannel, iCrossChannel] = Math.Round(cross, 3);
                        //    }
                        //    else
                        //        CrossTalk[iChannel, iCrossChannel] = 999;
                        //}
                        //#endregion

                        //#region for AX-
                        //if (iChannel == 0x00)
                        //    pstResultData.m_dblAXLeft[iChannel] = 99;
                        //else
                        //{
                        //    pstResultData.m_dblAXLeft[iChannel] = Math.Round(CrossTalk[iChannel, iChannel - 1] - dwMaxLossMax[iChannel], 2);
                        //}
                        //#endregion

                        //#region  for AX+
                        //if (iChannel == dwEndChannel - 1)
                        //    pstResultData.m_dblAXRight[iChannel] = 99;
                        //else
                        //{
                        //    pstResultData.m_dblAXRight[iChannel] = Math.Round(CrossTalk[iChannel, iChannel + 1] - dwMaxLossMax[iChannel], 2);
                        //}
                        //#endregion

                        //#region for TAX
                        //double dblTemp1 = Math.Pow(10, -pstResultData.m_dblAXLeft[iChannel] / 10.0);
                        //double dblTemp2 = Math.Pow(10, -pstResultData.m_dblAXRight[iChannel] / 10.0);

                        //double Temp = 0;
                        //if (iChannel == 0)
                        //    Temp = dblTemp2;
                        //else if (iChannel == dwEndChannel - 1)
                        //    Temp = dblTemp1;
                        //else
                        //    Temp = dblTemp1 + dblTemp2;
                        //pstResultData.m_dblTAX[iChannel] = CalcTotal(Temp);
                        //#endregion

                        //#region  for NX
                        //double dblNX = 10000000.0;
                        //for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                        //{
                        //    if (dwITUIndex < iChannel && (iChannel - dwITUIndex) < 2)
                        //        continue;

                        //    if (dwITUIndex >= iChannel && (dwITUIndex - iChannel) < 2)
                        //        continue;

                        //    if ((CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]) < dblNX)
                        //        dblNX = CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel];
                        //}
                   
                        //pstResultData.m_dblNX[iChannel] = Math.Round(dblNX, 2);
                        //#endregion

                        //#region  for TX
                        //double dblTX = 0.0;
                        //for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                        //{
                        //    if (dwITUIndex == iChannel)
                        //        continue;
                        //    dblTX += Calc10Power(CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]);
                        //}
                        //pstResultData.m_dblTX[iChannel] = CalcTotal(dblTX);
                        //#endregion

                        //#region  for TX - AX
                        //double dblTXAX = 0.0;
                        //for (int dwITUIndex = 0x00; dwITUIndex < dwEndChannel; dwITUIndex++)
                        //{
                        //    if (dwITUIndex < iChannel && (iChannel - dwITUIndex) < 2)
                        //        continue;

                        //    if (dwITUIndex >= iChannel && (dwITUIndex - iChannel) < 2)
                        //        continue;

                        //    dblTXAX += Calc10Power(CrossTalk[iChannel, dwITUIndex] - dwMaxLossMax[iChannel]);
                        //}
                        //pstResultData.m_dblTXAX[iChannel] = CalcTotal(dblTXAX);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                       // string mm = "";
                        throw ex;
                    }
                }
                #region  cancel TNX
                //for (int dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
                //{
                //    double dblTemp = 0.0;
                //    for (int dwIndex = dwStartChannel; dwIndex < dwEndChannel; dwIndex++)
                //    {
                //        if (dwIndex < dwChannelIndex && (dwChannelIndex - dwIndex) < 2)
                //            continue;

                //        if (dwIndex >= dwChannelIndex && (dwIndex - dwChannelIndex) < 2)
                //            continue;

                //        double dblNX = CrossTalk[dwChannelIndex, dwIndex] - dwMaxLossMax[dwChannelIndex];

                //        dblTemp += Calc10Power(dblNX);
                //    }
                //    pstResultData.m_dblTNX[dwChannelIndex] = CalcTotal(dblTemp);
                //}
             #endregion

                //Uniformity
                double max = Alg_PointSearch.FindMaxValueInArray(pstResultData.m_dblILMax);
                double min = Alg_PointSearch.FindMinValueInArray(pstResultData.m_dblILMax);
                pstResultData.m_dblUniformity = Math.Round((max - min), 3);
                bSuccess = true;
                //   Frm_AWGTestClient.SaveAutoRawDataTest(pdwMinLossArrayTest, pdwMaxLossArrayTest, pstAutoWaveform.m_pdwWavelengthArray);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + " When Calculate!!!");
            }
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
            double[] CrossPoint = new double[2]; //Index 0 for Left,1 for right
            try
            {
                double WaveLeftLess, WaveLeftGreater, WaveRightLess, WaveRightGreater, IL1, IL2, IL3, IL4;
                double waveLeftILMax = 0;
                double waveRightILMax = 0;
                int indexLeftLess, indexLeftGreater, indexRigtLess, indexRigtGreater;

                int iLen = RawDataY.Length;
                indexLeftLess = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
                indexLeftGreater = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
                indexRigtLess = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);
                indexRigtGreater = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);

                if (indexLeftLess == -1)
                    indexLeftGreater = 0;
                if (indexLeftGreater == -1)
                    indexLeftGreater = 0;

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
            catch { }
            return CrossPoint;
        }
        public double[] GetCrossPointCW(double[] RawDataY, double[] RawDataX, double targetLine, int iSplitIndex)
        {
            double[] CrossPoint = new double[2]; //Index 0 for Left,1 for right
            try
            {
                double WaveLeftLess, WaveLeftGreater, WaveRightLess, WaveRightGreater, IL1, IL2, IL3, IL4;
                double waveLeftILMax = 0;
                double waveRightILMax = 0;
                int indexLeftLess, indexLeftGreater, indexRigtLess, indexRigtGreater;

                int iLen = RawDataY.Length;
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
            }
            catch(Exception ex)
            {
               
            }
            return CrossPoint;
        }

        public double[] GetCrossPoint(double [] RawDataY, double[] RawDataX,double targetLine,int iSplitIndex,double CW)
        {
            double[] CrossPoint = new double[2]; //Index 0 for Left,1 for right
            try
            {
                double WaveLeftLess, WaveLeftGreater, WaveRightLess, WaveRightGreater, IL1, IL2, IL3, IL4;
                double waveLeftILMax = 0;
                double waveRightILMax = 0;
                int indexLeftLess, indexLeftGreater, indexRigtLess, indexRigtGreater;

                int iLen = RawDataY.Length;
                indexLeftLess = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
                indexLeftGreater = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
                indexRigtLess = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);
                indexRigtGreater = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);
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

                    CrossPoint[0] = CW - (waveRightILMax - CW);
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
            }
            catch
            { }
            return CrossPoint;
        }

        public double[] GetCrossPointForPassBand(double[] RawDataY, double[] RawDataX, double targetLine, int iSplitIndex, double CW)
        {
            double[] CrossPoint = new double[2]; //Index 0 for Left,1 for right
            int iLen = RawDataY.Length;

            double[] RawDataYReveses = new double[iLen];
            for (int i = 0; i < iLen; i++)
                RawDataYReveses[i] = RawDataY[iLen - 1 - i];
            try
            {
                double WaveLeftGreater, WaveRightGreater, IL2, IL4;
                double waveLeftILMax = 0;
                double waveRightILMax = 0;
                int indexLeftLess, indexLeftGreater, indexRigtLess, indexRigtGreater;

                //indexLeftLess = Alg_PointSearch.FindFirstValue(RawDataYReveses, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, iLen - iSplitIndex, iLen - 1);
                //indexLeftGreater = Alg_PointSearch.FindLastValue(RawDataYReveses, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, iLen - iSplitIndex, iLen - 1);

                indexLeftLess = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
                indexLeftGreater = Alg_PointSearch.FindLastValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, 0, iSplitIndex - 1);
                indexRigtLess = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.LessThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);
                indexRigtGreater = Alg_PointSearch.FindFirstValue(RawDataY, Alg_PointSearch.ComparisonOperator.GreaterThanOrEqualTo, targetLine, iSplitIndex + 1, iLen - 1);
                //if (indexLeftGreater == -1 || indexLeftLess==-1)
                //{
                //    //WaveRightLess = RawDataX[indexRigtLess];
                //    WaveRightGreater = RawDataX[indexRigtGreater];
                //    //IL3 = RawDataY[indexRigtLess];
                //    //IL4 = RawDataY[indexRigtGreater];
                //    //if (IL3 != IL4)
                //    //    waveRightILMax = LinearInterpolateAlgorithm.Calculate(IL3, IL4, WaveRightLess, WaveRightGreater, targetLine);
                //    //else
                //    waveRightILMax = WaveRightGreater;

                //    CrossPoint[0] = CW - (waveRightILMax - CW);
                //    CrossPoint[1] = waveRightILMax;
                //}
                //else if (indexRigtGreater == -1 || indexRigtLess==-1)
                //{
                //    WaveLeftLess = RawDataX[indexLeftGreater];
                //    //WaveLeftGreater = RawDataX[iLen - indexLeftGreater];
                //    //IL1 = RawDataYReveses[indexLeftLess];
                //    //IL2 = RawDataYReveses[indexLeftGreater];
                //    //if (IL1 != IL2)
                //    //    waveLeftILMax = LinearInterpolateAlgorithm.Calculate(IL1, IL2, WaveLeftLess, WaveLeftGreater, targetLine);
                //    //else
                //    waveLeftILMax = WaveLeftLess;
                //    CrossPoint[0] = waveLeftILMax;
                //    CrossPoint[1] = CW + (CW - waveLeftILMax);
                //}
                //else
                //{

                WaveLeftGreater = RawDataX[indexLeftGreater];

                WaveRightGreater = RawDataX[indexRigtGreater];

                IL2 = RawDataY[indexLeftGreater];

                IL4 = RawDataY[indexRigtGreater];

                waveLeftILMax = WaveLeftGreater;

                waveRightILMax = WaveRightGreater;

                CrossPoint[0] = waveLeftILMax;
                CrossPoint[1] = waveRightILMax;
                //} 
            }
            catch
            { }
            return CrossPoint;
        }

  
        public bool ReadRefRawData(ref double [] reference, ref tagAutoWaveform pstAutoWaveform, int dwStartChannel, int dwEndChannel)
        {
            m_pdwRef = new double[1];
            int dwIndex = 0;
            double dblTemp;
            try
            {
                for (int dwChannelIndex = 0x00; dwChannelIndex < CHANNEL_COUNT; dwChannelIndex++)
                {
                    double sumReference = 0.0;
                    int count = 0;
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
                                sumReference += dblTemp;
                                count++;
                                dwIndex++;

                                Array.Resize(ref m_pdwRef, dwIndex + 1);
                              
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
                    reference[dwChannelIndex] = sumReference / count;
                }
               
                pstAutoWaveform.m_dwSampleCount = m_dwSamplePoint;
              
                for (dwIndex = 0x00; dwIndex < m_dwSamplePoint; dwIndex++)
                {
                    for (int dwChannelIndex = 0x00; dwChannelIndex < dwEndChannel - dwStartChannel; dwChannelIndex++)
                    {
                        pstAutoWaveform.m_pdwILMinArray[dwChannelIndex, dwIndex] -= m_pdwRef[dwIndex + dwChannelIndex * m_dwSamplePoint];

                        pstAutoWaveform.m_pdwILMaxArray[dwChannelIndex, dwIndex] -= m_pdwRef[dwIndex + dwChannelIndex * m_dwSamplePoint];
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
       
        public double ConvertITUNameToWL(string strChannelName)
        {
            double WL = 0;
            string sPath = Directory.GetCurrentDirectory() + "\\ITU.txt";
            string[] strTemp = ReadTxTFile(sPath);
            bool bGet = false;
            for (int i = 1; i < strTemp.Length; i++)
            {
                if (strTemp[i].Contains(strChannelName.ToUpper()))
                {
                    string[] str = strTemp[i].Split(',');
                    WL = double.Parse(str[2]);
                    bGet = true;
                    break;
                }
            }
            if (!bGet)
                throw new Exception("Can not Finde ITU in File " + sPath);

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
        public bool ReadDatILMaxMinData(string strFilePathName, ref tagAutoWaveform pstAutoWaveform)
        {
            double[] m_pdwWaveLengt = new double[m_dwSamplePoint];
            try
            {
                using (CsvReader reader = new CsvReader())
                {
                    reader.OpenFile(strFilePathName);

                    String[] line;
                    int iCount = 0;
                    line = reader.GetLine();
                    while ((line=reader.GetLine()) != null)
                    {
                        m_pdwWaveLengt[iCount] = double.Parse(line[0]);
                        pstAutoWaveform.m_pdwWavelengthArray[iCount] = m_pdwWaveLengt[iCount];
                        for (int i = 0; i < this.CHANNEL_COUNT; i++)
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
                frmAWGClient.ShowMsg(ex.ToString(), false);
                return false;
            }
            return true;
        }

        public bool ReadRawData(string strFilePathName)
        {
            double dblTemp;
            m_pdwWave = new double[m_dwSamplePoint];
            m_pdwPolPwr1 = new double[m_dwSamplePoint];
            m_pdwPolPwr2 = new double[m_dwSamplePoint];
            m_pdwPolPwr3 = new double[m_dwSamplePoint];
            m_pdwPolPwr4 = new double[m_dwSamplePoint];

            int lineNbr = 0;
            int dwIndex = 0;
            //window = Convert.ToInt32(4 / m_dwStep);
            //if (window / 2 == 0)
            //{
            //    window++;
            //};
            try
            {
                using (CsvReader reader = new CsvReader())
                {
                    reader.OpenFile(strFilePathName);
                    string[] lineElems;
                    // read the 1st line
                    lineElems = reader.GetLine();
                    lineNbr++;
                    lineElems = reader.GetLine();
                    int lineElemLen = lineElems.Length;
                    if (lineElemLen < 6)
                        return false;
                    do
                    {
                        lineNbr++;
                        try
                        {
                            m_pdwWave[dwIndex] = double.Parse(lineElems[0]);

                            dblTemp = double.Parse(lineElems[1]);
                            m_pdwPolPwr1[dwIndex] = dblTemp;
                            dblTemp = double.Parse(lineElems[2]);
                            m_pdwPolPwr2[dwIndex] = dblTemp;
                            dblTemp = double.Parse(lineElems[3]);
                            m_pdwPolPwr3[dwIndex] = dblTemp;
                            dblTemp = double.Parse(lineElems[4]);
                            m_pdwPolPwr4[dwIndex] = dblTemp;

                            dwIndex++;
                            if (dwIndex >= m_dwSamplePoint)
                                break;
                        }
                        catch (Exception ex)
                        {
                            string errMsg = String.Format("Invalid line in  file: '{0}', line {1}",
                                strFilePathName, lineNbr);
                            throw new Exception(errMsg, ex);
                        }
                    }
                    while ((lineElems = reader.GetLine()) != null);

                }
            }
            catch (Exception ex)
            {
                string errMsg = String.Format("Invalid line in  file: '{0}', line {1}", strFilePathName, lineNbr);
                //throw new Exception(errMsg, er);
                frmAWGClient.ShowMsg(errMsg + "  " + ex.ToString(),false);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 读取校准的RawData
        /// </summary>
        /// <param name="strFilePathName"></param>
        /// <returns></returns>
        public bool ReadCaliRawData(string strFilePathName)
        {
            double dblTemp;
            m_pdwWave = new double[m_dwSamplePoint];
            m_pdwCaliPolPwr1 = new double[m_dwSamplePoint];
            m_pdwCaliPolPwr2 = new double[m_dwSamplePoint];
            m_pdwCaliPolPwr3 = new double[m_dwSamplePoint];
            m_pdwCaliPolPwr4 = new double[m_dwSamplePoint];

            int lineNbr = 0;

            int dwIndex = 0;
            //window = Convert.ToInt32(4 / m_dwStep);
            //if (window / 2 == 0)
            //{
            //    window++;
            //};
            try
            {
                using (CsvReader reader = new CsvReader())
                {
                    reader.OpenFile(strFilePathName);
                    string[] lineElems;
                    // read the 1st line
                    lineElems = reader.GetLine();
                    lineNbr++;
                    lineElems = reader.GetLine();
                    int lineElemLen = lineElems.Length;
                    if (lineElemLen <6)
                        return false;
                    do
                    {
                        lineNbr++;
                        try
                        {
                            m_pdwWave[dwIndex] = double.Parse(lineElems[0]);
                           
                                dblTemp = double.Parse(lineElems[1]);
                                m_pdwCaliPolPwr1[dwIndex] = dblTemp;
                                dblTemp = double.Parse(lineElems[2]);
                                m_pdwCaliPolPwr2[dwIndex] = dblTemp;
                                dblTemp = double.Parse(lineElems[3]);
                                m_pdwCaliPolPwr3[dwIndex] = dblTemp;
                                dblTemp = double.Parse(lineElems[4]);
                                m_pdwCaliPolPwr4[dwIndex] = dblTemp;
                         
                            dwIndex++;
                            if (dwIndex >= m_dwSamplePoint)
                                break;
                        }
                        catch (Exception ex)
                        {
                            string errMsg = String.Format("Invalid line in  file: '{0}', line {1}",
                                strFilePathName, lineNbr);
                            throw new Exception(errMsg, ex);
                        }
                    }
                    while ((lineElems = reader.GetLine()) != null);

                }
            }
            catch (Exception ex)
            {
                string errMsg = String.Format("Invalid line in  file: '{0}', line {1}", strFilePathName, lineNbr);
                //throw new Exception(errMsg, er);
                frmAWGClient.ShowMsg(errMsg + "  " + ex.ToString(), false);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 根据测试功率和校准功率算出ILmin和ILmax
        /// </summary>
        /// <param name="pstAutoWaveform"></param>
        public void GetILMinMax(ref tagAutoWaveform pstAutoWaveform)
        {
            double m11 ;
            double m12 ;
            double m13 ;
            double m14;
            double TMax;
            double TMin;
            try
            {
                //功率由dBm转换成mW 10^(P/10)
                for (int point = 0; point < m_dwSamplePoint; point++)
                {
                    if(m_pdwCaliPolPwr1[point]<m_pdwPolPwr1[point]|| m_pdwCaliPolPwr2[point] < m_pdwPolPwr2[point]|| m_pdwCaliPolPwr3[point] < m_pdwPolPwr3[point]|| m_pdwCaliPolPwr4[point] < m_pdwPolPwr4[point])
                    {
                        string errMsg = String.Format("校准功率小于产品功率，请确认校准时接线是否准确！");
                        frmAWGClient.ShowMsg(errMsg, false);
                        MessageBox.Show(errMsg, "Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    m11 = (Math.Pow(10, m_pdwPolPwr1[point] / 10) / Math.Pow(10, m_pdwCaliPolPwr1[point] / 10) + Math.Pow(10, m_pdwPolPwr2[point] / 10) / Math.Pow(10, m_pdwCaliPolPwr2[point] / 10)) / 2;
                    m12 = (Math.Pow(10, m_pdwPolPwr1[point] / 10) / Math.Pow(10, m_pdwCaliPolPwr1[point] / 10) - Math.Pow(10, m_pdwPolPwr2[point] / 10) / Math.Pow(10, m_pdwCaliPolPwr2[point] / 10)) / 2;
                    m13 = Math.Pow(10, m_pdwPolPwr3[point] / 10) / Math.Pow(10, m_pdwCaliPolPwr3[point] / 10) - m11;
                    m14 = Math.Pow(10, m_pdwPolPwr4[point] / 10) / Math.Pow(10, m_pdwCaliPolPwr4[point]) / 10 - m11 ;
                    TMax = m11 + Math.Sqrt(m12 * m12 + m13 * m13 + m14 * m14);
                    if ((m12 * m12 - m13 * m13 - m14 * m14) < 0)
                    {
                        TMin = m11;
                    }
                    else
                    {
                        TMin = m11 - Math.Sqrt(m12 * m12 - m13 * m13 - m14 * m14);
                    }
                    for (int ch = 0; ch < CHANNEL_COUNT; ch++)
                    {
                        pstAutoWaveform.m_pdwILMinArray[ch, point] = -10 * Math.Log10(TMax);
                        pstAutoWaveform.m_pdwILMaxArray[ch, point] = -10 * Math.Log10(TMin);
                    }
                    pstAutoWaveform.m_pdwWavelengthArray[point] = m_pdwWave[point];
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"计算ILMin和ILMax出错，{ex.Message}");
            }
        }
      
        public bool SaveILMinMaxRawData(tagAutoWaveform pstAutoWaveform,DeviceInfo deviceInfo, int iStation, DateTime testTime, int m_dwTestIndex, string sPath)
        {
            string strTime = testTime.ToString("yyyy-MM-dd-hh-mm");
            string strStation = m_dwTestIndex.ToString().PadLeft(3, '0');
            string strXLSName = string.Format("SU-{0}-{1}-{2}-{3}-T-{4}-{5}.csv", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID.Substring(0, 2).PadLeft(3, '0'), deviceInfo.m_strChipID.Substring(2, 2).PadLeft(3, '0'), "S" + iStation.ToString().PadLeft(2, '0'), strStation, strTime);
            strXLSName = sPath + strXLSName;
            if (File.Exists(strXLSName))
            {
                DialogResult dia = MessageBox.Show("RawData file exists,overwrite it?", "Save RawData", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dia == DialogResult.No)
                    return false;
                else
                    File.Delete(strXLSName);
            }

            try
            {
                FileStream file = new FileStream(strXLSName, FileMode.Append);
                StreamWriter sw = new StreamWriter(file);

                //sw.Write("JDS SWS PDL Measurements\r\n");
                //sw.Write(string.Format("Part serial no:\t{0}-{1}\r\n", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID));
                //sw.Write(string.Format("Operator:\t{0}\r\n", deviceInfo.m_EditOperator));
                ////string strTemp = comboBoxOOption.Text;
                //sw.Write(string.Format("Test Type:\t{0}\r\n", deviceInfo.m_strTestType));
                //sw.Write(string.Format("Test Bench:\t{0}\r\n", 1));
                //sw.Write(string.Format("Date/Time:\t{0}\t{1}\r\n", testTime.ToString("MM/dd/yyyy"), testTime.ToString("HH:mm")));
                //sw.Write("Scale Factor:\t0.000\r\n");
                //sw.Write(string.Format("Comment:\t{0}\r\n", deviceInfo.m_strComment));
                //sw.Write(string.Format("Temperature (C):\t{0}\r\n", deviceInfo.m_strTemperature));
                //sw.Write(string.Format("Input:\t{0}\r\n", deviceInfo.m_strInput));
                //sw.Write(string.Format("Outputs:\t{0}\r\n", deviceInfo.m_strOutput));
                //sw.Write(string.Format("Mask Name:\t{0}\r\n", deviceInfo.m_strMaskName));

                //sw.Write("blank:\t\r\n");
                //sw.Write("\r\n");
                //sw.Write("\r\n");
                //sw.Write("\r\n");
                //sw.Write("---BEGIN DATA---\r\n");
                //sw.Write("\r\n");

                int dwSampleCount = pstAutoWaveform.m_dwSampleCount;
                int dwChannelCount = CHANNEL_COUNT;
                string strTemp = "Wavelength (nm),";
                string str1 = "";
                
                    str1 = string.Format("ILMIN,ILMAX");
                    strTemp += str1;
                
                sw.Write(strTemp);
                sw.Write("\r\n");
                for (int dwIndex = 0; dwIndex < dwSampleCount; dwIndex++)
                {
                    strTemp = Math.Round(pstAutoWaveform.m_pdwWavelengthArray[dwIndex], 3).ToString("####.000") + ",";
                    
                        double dblILMin, dblILMax;

                        dblILMax = pstAutoWaveform.m_pdwILMaxArray[0, dwIndex];
                        dblILMin = pstAutoWaveform.m_pdwILMinArray[0, dwIndex];

                        str1 = string.Format("{0},{1}", Math.Round(dblILMin, 3).ToString("####.000"), Math.Round(dblILMax, 3).ToString("####.000"));
                        strTemp += str1;
                   
                    sw.Write(strTemp);
                    sw.Write("\r\n");
                }

                sw.Flush();
                sw.Close();
                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Raw Data Failed !!!" + ex.ToString());
                return false;
            }
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
                     double dwStep, int dwOutputPort, int dwLLog, int dwWSIndex)
        {
            string strTmplFileName = Directory.GetCurrentDirectory() + @"\Reference\" + "AWGTLSSetting" + dwWSIndex + ".ini";
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Start WL", dblStartWL.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Stop WL", dblStopWL.ToString());
            INIOperationClass.INIWriteValue(strTmplFileName, tszSectionName, "Step Size", dwStep.ToString());
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
