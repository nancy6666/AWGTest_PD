using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace AWGTestServer.Instruments
{
    class UC872_SerialPort
    {
        #region Properties
        SerialPort port;
        public enum EnumTriggrerMode
        {
            Ignore,Smeasure,Nextstep,Cmeasure

        }
      public enum EnumPulseType
        {
            HIGH,
            LOW
        }
        public enum EnumPowerUnit
        {
            dBm,
            mW,
            dB
        }
        public enum EnumState
        {
            STARt,STOP
        }
   
        #endregion

        #region Constructor
        public UC872_SerialPort(string portName, int baudByte)
        {
            port = new SerialPort(portName, baudByte, Parity.None, 8, StopBits.One);
            try
            {
                port.ReadTimeout = 1000;
                port.Open();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// set trigger mode
        /// </summary>
        /// <param name="triggrerMode"></param>
        public void SetTiggerInput(EnumTriggrerMode triggrerMode)
        {
            switch(triggrerMode)
            {
                case EnumTriggrerMode.Cmeasure:
                    port.WriteLine("Sense:Trigger:INPUT Cmeasure");
                    break;
                case EnumTriggrerMode.Smeasure:
                    port.WriteLine("Sense:Trigger:INPUT Smeasure");
                    break;
                case EnumTriggrerMode.Ignore:
                    port.WriteLine("Sense:Trigger:INPUT Igonore");
                    break;
                case EnumTriggrerMode.Nextstep:
                    port.WriteLine("Sense:Trigger:INPUT Nextstep");
                    break;
            }
        }
        /// <summary>
        /// get current power 
        /// </summary>
        /// <returns></returns>
        public string ReadPower()
        {
            port.WriteLine("*IDN?");
            var a = port.ReadLine();
         //port.WriteLine("S:P:W?");
            port.WriteLine("Sense : Function :State: ?");
          //  port.WriteLine("R : P?");
            var b= port.ReadLine();
            return b;
        }

        /// <summary>
        /// set pulse type
        /// </summary>
        /// <param name="pulseType">HIGH/LOW</param>
        public void SetPulseType(EnumPulseType pulseType)
        {
            switch (pulseType)
            {
                case EnumPulseType.HIGH:
                    port.WriteLine("INITSYS:PULSE HIGH");
                    break;
                case EnumPulseType.LOW:
                    port.WriteLine("INITSYS:PULSE LOW");
                    break;
            }
        }
        /// <summary>
        /// 设置最后一次设置的内部功能函数的状态。
        /// </summary>
        /// <param name="enumState">start/stop</param>
        public void SetFunctionState(EnumState enumState)
        {
            port.WriteLine($"Sense:Function:State:{enumState.ToString()}");
        }
        public void SetPowerUnit(EnumPowerUnit powerUnit)
        {
            switch (powerUnit)
            {
                case EnumPowerUnit.dB:
                    port.WriteLine("S1:P:U dB");
                    break;
                case EnumPowerUnit.dBm:
                    port.WriteLine("S1:P:U dBm");
                    break;
                case EnumPowerUnit.mW:
                    port.WriteLine("S1:P:U mW");
                    break;
            }
        }
        /// <summary>
        /// 设置内部功能函数的参数,采样点数及时间
        /// </summary>
        /// <param name="point">通道采集的数据点</param>
        /// <param name="interval">功率计采样每个数据点所需平均时间，mS</param>
        public void SetParameters(double point,double interval)
        {
            port.WriteLine($"Sens : F : P : L {point},{interval}");
        }
        public double[] ReadResult(int PointDesired)
        {
            // 串口读取超时时间，单位ms
            const int READ_TIMEOUT = 2000;

            byte[] byteResult=new byte[PointDesired*2];
            double[] dbResult = new double[PointDesired];
            int byteReceived = 0;
            bool isTimeout = false;

            Stopwatch sw = new Stopwatch();

            port.WriteLine("Sense : F : R?");

            sw.Start();

            while (true)
            {
                Thread.Sleep(100);

                var byteInBuffer = port.BytesToRead;
                port.Read(byteResult, byteReceived, byteInBuffer);
                byteReceived += byteInBuffer;

                // 接收到足够的数据点。
                // 一次Trigger采样一个数据点，一个数据点两个字节，因此 PointDesired * 2
                if (PointDesired * 2 <= byteReceived)
                    break;
                
                // 如果接受到数据，复位超时定时器
                if(byteInBuffer > 0)
                    sw.Restart();

                // 判断是否接受超时
                if(sw.ElapsedMilliseconds > READ_TIMEOUT)
                {
                    isTimeout = true;
                    break;
                }
            }

            sw.Stop();

            if (isTimeout)
                throw new TimeoutException($"没有接收到足够的采样点，期望{PointDesired * 2}字节，实际读取{byteReceived}字节。");
            else
            {
                for (int i = 0; i < PointDesired; i++)
                {
                    string str1 = byteResult[i * 2].ToString("X");
                    str1 = Converters.HexToDec(str1);
                    double temp1 = double.Parse(str1);
                    string str2 = byteResult[i * 2 + 1].ToString("X");
                    str2 = Converters.HexToDec(str2);
                    double temp2 = double.Parse(str2);
                    double y = (((temp2 - 128) * 128 + temp1) - 10000) / 100;
                    dbResult[i] = y;
                }
            }
                return dbResult;
        }
        public void ClearBuffer()
        {
            port.WriteLine("Sense:F:BC ");
        }
        #endregion
    }
}
