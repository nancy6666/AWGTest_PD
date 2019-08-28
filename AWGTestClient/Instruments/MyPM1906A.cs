using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM1906AHelper;
using PM1906AHelper.Core;
using PM1906AHelper.Calibration;

namespace AWGTestClient.Instruments
{
    class MyPM1906A : IPowermeter
    {
        PM1906A pm;
        SerialPort port;
        ConfigurationInstruments cfg = new ConfigurationInstruments();
        private bool _isConnected;
        public bool IsConnected
        {
            set
            {
                _isConnected = (port != null && port.IsOpen);
            }
            get
            {
                return _isConnected;
            }
        }

        public MyPM1906A(string PortName, int BaudRate)
        {
                pm = new PM1906A(PortName, BaudRate);

                pm.Open();

                this.IsConnected = true;
        }

        public void SetWavelength(int wave)
        {
            pm.SetWavelength(wave);
        }
        public void GetPowermeterData(out double[] powers, int desiredPoint)
        {
            try
            {
                var len = pm.Trigger_GetUsedBuffLen();
                if (len != desiredPoint)
                {
                    throw new Exception("获取到的数据，与预期的数量不同！");
                }
                powers = pm.Trigger_ReadBuffer().ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                pm.Trigger_Stop();
            }
        }

        public void SetParameters(double cw)
        {
            string strRange = cfg.PM1906_Range.ToUpper();
            var range = new PM1906AHelper.Core.RangeEnum();
            switch(strRange)
            {
                case "RANGE1":
                    range = PM1906AHelper.Core.RangeEnum.RANGE1;
                    break;
                case "RANGE2":
                    range = PM1906AHelper.Core.RangeEnum.RANGE2;
                    break;
                case "RANGE3":
                    range = PM1906AHelper.Core.RangeEnum.RANGE3;
                    break;
                case "RANGE4":
                    range = PM1906AHelper.Core.RangeEnum.RANGE4;
                    break;
            }
            pm.SetRange(range);
            pm.SetUnit(PM1906AHelper.Core.UnitEnum.dBm);
            pm.SetWavelength(Convert.ToInt32(cw));
         
        }
        public void StartSweep()
        {
            pm.Trigger_CleanBuffer();
            pm.Trigger_Start();
        }
    }
}
