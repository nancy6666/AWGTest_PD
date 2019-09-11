using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestClient.Instruments
{
    public interface IPowermeter
    {
        void SetParameters(int cw);

        void StartSweep();

        void GetPowermeterData(out double[] powers, int desiredPoint);
        void Open();
        void Close();
    }
}
