using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestClient
{
    interface IAWGTest
    {
        double StepWavelength { get; set; }
        double StartWavelength { get; set; }
        double StopWavelength { get; set; }
        double MaxChannel { get; set; }
        int SamplingPoint { get; set; }
        void InitPowermeter(double cw);
      
        void ReadSaveCaliData(string strFileName);
        void StartSweep();
        void ReadSaveTestPower(string strFilePath);
        void GetILMinMax(ref tagAutoWaveform pstAutoWaveform);

    }
}
