using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestServer.Instruments
{
   public class N7786B
    {
        AgN778xLib.AgN778x agN778;

        public N7786B(int GPIBAddr)
        {
            agN778 = new AgN778xLib.AgN778x();
            agN778.Initialize($"GPIB::{GPIBAddr}::INSTR", false, false);
        }
        public void Close()
        {
            agN778.Close();
        }
        public void SetWavelength(double wave)
        {
            agN778.Polarimeter.Wavelength = wave;
        }
        public void SetSOP(double[] sop)
        {
            try
            {
                agN778.PolController.Stabilizer.SOP = sop;
            }
            catch(Exception ex)
            {
                throw new Exception($"设置SOP出错，{ex.Message}");
            }
        }
        public Array GetSop()
        {
            // var sop= agN778.SCPIQuery(":POL:SOP?");
            //double[] sop = agN778.Polarimeter.SOP;
            var sop = agN778.PolController.Stabilizer.SOP;
       
            return sop;
        }

    }
}
