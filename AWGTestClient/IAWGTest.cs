using AWGTestClient.Instruments;
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

        List<IPowermeter> lstPowermeter { get; set; }
        List<double[]> lstCaliResult { get; set; }
        void InitPowermeter();
      
        void SaveCaliData(string strFileName);

        void StartSweep();

        void StopSweep();

        void ReadSaveTestPower(string strFilePath);

        void GetILMinMax(ref tagAutoWaveform pstAutoWaveform);

        void SaveILMinMax(tagAutoWaveform pstAutoWaveform,string strFile);

        void ReadCali(IPowermeter pm);

        void ReadCaliRawData(string caliFile);

        void ReadDatILMaxMinData(string strFilePathName, ref tagAutoWaveform pstAutoWaveform);

        void OpenPowermeter();

        void ClosePowermeter();
    }
}
