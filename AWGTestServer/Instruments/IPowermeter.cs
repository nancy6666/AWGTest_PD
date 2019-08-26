using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestServer.Instruments
{
    public interface IPowermeter
    {
        bool IsConnected
        {
            set;
            get;
        }
       
        void SetParameters();
        void StartSweep();
        void GetPowermeterData(out double[] powers, int desiredPoint);
        void SetWavelength(int wave);
    }
}
