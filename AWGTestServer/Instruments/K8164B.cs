using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWGTestServer.Instruments
{
    class K8164B
    {
        private GPIB k8164B;
        public enum SweepMode { STEP, MAN, CONT }
        public enum TriggerMode { DIS, AVG, MEAS,MOD,STF,SWF,SWST }
        public enum SweepState { STOP, STAR, PAUS,CONT }
        public enum PowerUnit {PW,NW,UW,MW,Watt,DBM };
        public enum WaveUnit { PM,NM,UM,M}
        public double SMSR { get; set; }
        public double CWL { get; set; }
        public K8164B(int GPIBaddr)
        {
            k8164B = new GPIB(GPIBaddr);
        }
        public void SetSweepMode(SweepMode mode)
        {
            try
            {
                switch (mode)
                {
                    case SweepMode.STEP:
                        k8164B.GPIBwr("wav:swe:mode STEP");
                        break;
                    case SweepMode.CONT:
                        k8164B.GPIBwr("wav:swe:mode CONT");
                        break;
                    case SweepMode.MAN:
                        k8164B.GPIBwr("wav:swe:mode MAN");
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred when set sweep mode.\r\n" + e.Message);
            }
        }

        public void SetSweepRep()
        {
            k8164B.GPIBwr(":WAV:SWE:REP ONEW");
        }
        public void SetSweepCycle(float cycle)
        {
            k8164B.GPIBwr($":WAV:SWE:CYCL {cycle}");
        }
      
        public void SetSweepStep(double wave)
        {
            try
            {
                k8164B.GPIBwr($"wav:swe:step {wave}nm");
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred when set sweep step.\r\n" + e.Message);
            }
        }
        /// <summary>
        /// Stops, starts, pauses or continues a wavelength sweep.
        /// </summary>
        /// <param name="state">0 or STOP;1 or STARt;2 or PAUSe;3 or CONTinue</param>
        public void SetSweepState(SweepState state)
        {
            try
            {
                k8164B.GPIBwr($"wav:swe {(uint)state}");
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred when set sweep state.\r\n" + e.Message);
            }
        }

        public void StartSweep(SweepState state)
        {
            // 等待扫描结束的超时时间，单位ms
            const int Sweep_TIMEOUT = 180000;

            bool isTimeout = false;

            Stopwatch sw = new Stopwatch();
            try
            {
                k8164B.GPIBwr($"wav:swe {(uint)state}");
                Thread.Sleep(1000);
                k8164B.GPIBwr($"wav:swe?");
                sw.Start();
                while (true)
                {
                    Thread.Sleep(1000);
                    string status = k8164B.GPIBrd(200);
                    if (status.Contains("0"))
                    {
                        break;
                    }
                    else if (sw.ElapsedTicks > Sweep_TIMEOUT)

                    {
                        isTimeout = true;
                        break;
                    }
                }
                sw.Stop();
                if(isTimeout)
                {
                    throw new Exception("扫描超时！");
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred when start a sweep cycle.\r\n" + e.Message);
            }
        }

     
        public string ReadSweepStatus()
        {
            k8164B.GPIBwr($":WAV:SWE:STAT?");
            string status = k8164B.GPIBrd(200);
            return status;
        }

        public void SetTriggerMode(TriggerMode mode)
        {
            try
            {
                switch (mode)
                {
                    case TriggerMode.AVG:
                        k8164B.GPIBwr("trig:outp AVG");
                        break;
                    case TriggerMode.DIS:
                        k8164B.GPIBwr("trig:outp DIS");
                        break;
                    case TriggerMode.MEAS:
                        k8164B.GPIBwr("trig:outp MEAS");
                        break;
                    case TriggerMode.MOD:
                        k8164B.GPIBwr("trig:outp MOD");
                        break;

                    case TriggerMode.STF:
                        k8164B.GPIBwr("trig:outp STF");
                        break;
                    case TriggerMode.SWF:
                        k8164B.GPIBwr("trig:outp SWF");
                        break;
                    case TriggerMode.SWST:
                        k8164B.GPIBwr("trig:outp SWST");
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred when set trigger mode.\r\n" + e.Message);
            }
        }

        public void SetStartWave(double wave,WaveUnit waveUnit)
        {
            k8164B.GPIBwr($"wav:swe:star {wave}{waveUnit.ToString()}");
        }
        public void SetStopWave(double wave, WaveUnit waveUnit)
        {
            k8164B.GPIBwr($"wav:swe:stop {wave}{waveUnit.ToString()}");
        }
        public void SetOutputPower(double power, PowerUnit PowerUnit)
        {
            k8164B.GPIBwr($"sour:pow {power}{PowerUnit.ToString()}");
        }
        /// <summary>
        /// Switches the laser current off and on. 
        /// </summary>
        /// <param name="bActive"></param>
        public void SetOutputActive(bool bActive)
        {
            var a = bActive ? 1 : 0;
            k8164B.GPIBwr($"outp {a}");
        }
        /// <summary>
        /// Switches lambda logging on or off.
        /// Lambda logging is a feature that records the exact wavelength of a tunable laser module when a trigger is generated during a continuous sweep
        /// </summary>
        /// <param name="bActive">on/off/1/0</param>
        public void SetSweepLLog(int bActive)
        {
            var a = bActive==0 ? 0 : 1;
            k8164B.GPIBwr($"wav:swe:llog {a}");
            //disables amplitude modulation of the laser output when enable lambda logging
            if (a==1)
            {
                k8164B.GPIBwr("sour:am:stat 0");
            }
        }
       
        /// <summary>
        /// Sets the speed for continuous sweeping.
        /// </summary>
        /// <param name="sweepSpeed"></param>
        public void SetSweepSpeed(double sweepSpeed)
        {
            k8164B.GPIBwr($"wav:swe:spe {sweepSpeed}nm/s");
        }
        /// <summary>
        /// Returns whether the currently set sweep parameters (sweep mode, sweep start, stop, width, etc.) are consistent
        /// </summary>
        /// <returns></returns>
        public string GetConfigurationErr()
        {
            k8164B.GPIBwr("sour:wav:swe:chec?");
            var err = k8164B.GPIBrd(200);
            return err;
        }
        /// <summary>
        /// set input trigger mode as ignore
        /// </summary>
        public void SetInputTrigIgn()
        {
            k8164B.GPIBwr("trig:inp ign");
        }
    }
}
