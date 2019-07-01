using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestClient
{
    public struct DeviceInfo
    {
        public string m_EditSerialNumber;
        public string m_strChipID;
        public string m_strWaferID;
        public string m_strMaskName;
        public string m_strInput;
        public string dwCenter;
        public string m_strOutput;
        public string m_EditOperator;
        public string m_dwWSIndex;
        public string m_strComment;
        public string m_strTemperature;
        public string m_strTestDate;
        public string m_strTestType;
        public string m_strPartNum;
        public string m_strInputTemperature;
    }
    public struct testConditionStruct
    {
        public double[] iTUWL;
        public double ITUStep;
        public double ITUStepFreq;
        public bool IsWave;
        public double ILLossWindow;
        public double RippleLossWindow;
        public double CrossTalkLosWindow;
        public double AlignIL;
    }
    public struct tagPLCCriteria
    {
        public double m_dblWL1IL;
        public double m_dblWL2IL;
        public double m_dblWL3IL;
        public double m_dblWL4IL;
        public double m_dblWindowIL;
        public double m_dblWLPDL;
        public double m_dblWindowPDL;
        public double m_dblWLUnion1;
        public double m_dblWindowUnion1;
        public double m_dblWLUnion2;

        public bool m_bCW;
        public bool m_bPDW;
        public bool m_bShift;
        public bool m_bILMin;
        public bool m_bILMax;
        public bool m_bILITU;
        public bool m_bRipple;
        public bool m_bPDLITU;
        public bool m_bPDLCRT;
        public bool m_bPDLMax;
        public bool m_bBW05dB;
        public bool m_bBW1dB;
        public bool m_bBW3dB;
        public bool m_bBW20dB;
        public bool m_bBW25dB;
        public bool m_bBW30dB;
        public bool m_b05dBCCLeft;
        public bool m_b05dBCCRight;
        public bool m_b1dBCCLeft;
        public bool m_b1dBCCRight;
        public bool m_b3dBCCLeft;
        public bool m_b3dBCCRight;
        public bool m_bAXLeft;
        public bool m_bAXRight;
        public bool m_bNX;
        public bool m_bTX;
        public bool m_bTXAX;
        public bool m_bBWCust1;
        public bool m_bBWCust2;
        public bool m_bAlignIL;
        public bool m_bUniformity;
        public bool m_bILLossWindow;
        public bool m_bRippleLossWindow;
        public bool m_bCrossTalkLossWindow;
        public bool m_bPreTemperature;
        public bool m_bTempOffset;
        public bool m_bPowerOffset;

        public double m_dblCWCriterion;
        public double m_dblPDWCriterion;
        public double m_dblShiftCriterion;
        public double m_dblILMinCriterion;
        public double m_dblILMaxCriterion;
        public double m_dblILITUCriterion;
        public double m_dblRippleCriterion;
        public double m_dblPDLITUCriterion;
        public double m_dblPDLCRTCriterion;
        public double m_dblPDLMaxCriterion;
        public double m_dblBW05dBCriterion;
        public double m_dblBW1dBCriterion;
        public double m_dblBW3dBCriterion;
        public double m_dblBW20dBCriterion;
        public double m_dblBW25dBCriterion;
        public double m_dblBW30dBCriterion;
        public double m_dbl05dBCCLeftCriterion;
        public double m_dbl05dBCCRightCriterion;
        public double m_dbl1dBCCLeftCriterion;
        public double m_dbl1dBCCRightCriterion;
        public double m_dbl3dBCCLeftCriterion;
        public double m_dbl3dBCCRightCriterion;
        public double m_dblAXLeftCriterion;
        public double m_dblAXRightCriterion;
        public double m_dblNXCriterion;
        public double m_dblTXCriterion;
        public double m_dblTXAXCriterion;
        public double m_dblBWCust1Value;
        public double m_dblBWCust1Criterion;
        public double m_dblBWCust2Value;
        public double m_dblBWCust2Criterion;
        public double m_dblAlignIL;
        public double m_dblILLossWindow;
        public double m_dblRippleLossWindow;
        public double m_dblCrossTalkLossWindow;
        public double m_dblUniformitCriterion;
        public double m_dblLowPreTemperature;
        public double m_dblHighPreTemperature;
        public double m_dblTempOffset;
        public double m_dblPowerOffset;

        public int m_dwResult;
        public double[] m_dblItuWL;

        public string[] m_strChannelName;
    }

    public struct tagAutoWaveform
    {
        public bool m_bILOrPDL;
        public int m_dwSampleCount, m_dwChannelCount;
        public double m_dwStep;
        public double[] m_pdwWavelengthArray;
        public double[,] m_pdwLossArray;
        public double[,] m_pdwPDLArray;
        public double[,] m_pdwTEArray;
        public double[,] m_pdwTMArray;
        public double[,] m_pdwILMinArray;
        public double[,] m_pdwILMaxArray;
    }

    public struct tagPLCData
    {
        public double m_dblStartWL;
        public double m_dblStopWL;
        public double m_dblStep;
        public double m_dblPower;
        public int m_bOutputPort;
        public bool m_bLambdaLog;

        public int m_dwSamplePoint;
        public int m_dwChannelCount;

        public double[] m_dblCW;
        public double[] m_dblPDW;
        public double[] m_dblShift;
        public double[] m_dblILMin;
        public double[] m_dblILMax;
        public double[] m_dblILITU;
        public double[] m_dblRipple;
        public double[] m_dblPDLITU;
        public double[] m_dblPDLCRT;
        public double[] m_dblPDLMax;
        public double[] m_dblBW05dB;
        public double[] m_dblBW1dB;
        public double[] m_dblBW3dB;
        public double[] m_dblBW20dB;
        public double[] m_dblBW25dB;
        public double[] m_dblBW30dB;

        public double[] m_dblAXLeft;
        public double[] m_dblAXRight;
        public double[] m_dblNX;
        public double[] m_dblTX;
        public double[] m_dblTXAX;
        public double[] m_dblTAX;
        public double[] m_dblTNX;
        public double[] m_dblBWCust1;
        public double[] m_dblBWCust2;
        public double m_dblPreTemperature;
        public double m_dblUniformity;
        public double[] m_dbMax_at_cw;
        public double[] m_dbMin_at_cw;
        public double[] m_dbMax_at_itu;
        public double[] m_dbMin_at_itu;
    }

}
