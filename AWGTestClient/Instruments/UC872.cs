using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace AWGTestClient.Instruments
{
    class UC872port:IPowermeter
    {
        #region Properties

        SerialPort port;
        private StringBuilder data = new StringBuilder();
        private StringBuilder logging = new StringBuilder();

        public bool IsConnected
        {
            get
            {
                return port != null && port.IsOpen;

            }
        }

        public string ErrorString { get; private set; }
        bool IPowermeter.IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Mutex _mux = new Mutex();
        public enum EnumTriggrerMode
        {
            Ignore, Smeasure, Nextstep, Cmeasure

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
            STARt, STOP
        }

        #endregion

        #region Constructor

        public UC872port(string portName, int baudByte)
        {
            port = new SerialPort(portName, baudByte, Parity.None, 8, StopBits.One);
            if (!Connect8721())
            {
                throw new Exception("uc872连接失败");
            }

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 连接8728
        /// </summary>
        /// <returns></returns>
        public bool Connect8721()
        {
            if (this.IsConnected)
                return this.IsConnected;
            else
            {
                port.Open();
                port.DataReceived += _serialPort_DataReceived;

                UC8721_SendCommand($"METER:SCAN MODE 0");

                data.Clear();
                if (!UC8721_SendCommand("UC8721", data))
                {
                    this.Disconnect();
                    return false;
                }
                if (data.ToString().IndexOf("COM?") < 0)
                {
                    this.Disconnect();
                    return false;
                }

                data.Clear();
                if (!UC8721_SendCommand("COMPATIBLE", data))
                {
                    this.Disconnect();
                    return false;
                }

                if (data.ToString().IndexOf("OK") < 0)
                {
                    this.Disconnect();
                    return false;
                }
                return true;
            }
        }

        public bool Disconnect()
        {
            if (port != null)
            {
                port.DataReceived -= _serialPort_DataReceived;

                if (port.IsOpen)
                {
                    try
                    {
                        port.DiscardOutBuffer();
                        port.Close();
                    }
                    catch
                    {
                    }
                }

                port = null;
            }
            return true;
        }
        /// <summary>
        /// 检查串口是否打开
        /// </summary>
        /// <returns></returns>
        public bool CheckPortValid()
        {
            if (port == null)
                return false;

            if (port.IsOpen)
                return true;
            else
                return false;
        }

        /// <summary>
        /// set trigger mode
        /// </summary>
        /// <param name="triggrerMode"></param>
        public void SetTiggerInput(EnumTriggrerMode triggrerMode)
        {
            switch (triggrerMode)
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
            var b = port.ReadLine();
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
        public void SetParameters(double point, double interval)
        {
            port.WriteLine($"Sens : F : P : L {point},{interval}");
        }

        public void SetWavelength(int cw)
        {
            if (!UC8721_SendCommand($"METER: POW1: WAVE {cw}"))
                throw new Exception("设置功率计波长出错");
    }

        public void SetAveTime(double aveTime)
        {
            data.Clear();
            if (!UC8721_SendCommand($"METER:AVE {aveTime}", data))
                throw new Exception("设置uc872平均时间出错");
        }
        public void SetScanModeState(string state, StringBuilder data = null)
        {
           
              if(! UC8721_SendCommand($"METER:SCAN MODE {state}",data))
          
                throw new Exception("设置功率计扫描状态出错");
            
        }
        public bool GetState(out string state)
        {
            state = "";
            string strCmd = "Sense : Function :State: ?";
            bool result = SendCmdAndGetDataByUart(strCmd, out state);
            return result;
        }
        /// <summary>
        /// 发送串口命令并读取数据
        /// </summary>
        /// <param name="strcmd">字符串的串口命令</param>
        /// <param name="strData">返回的数据</param>
        /// <param name="delay">命令发送后延时，可选项，默认为10毫秒</param>
        /// <returns>true:命令发送成功,false:命令发送失败</returns>
        public bool SendCmdAndGetDataByUart(string strcmd, out string strData, int delay = 10)
        {
            string strout;
            List<byte> buf = new List<byte>();
            //byte[] buf = new byte[1024];
            //int iCount = 0;
            DateTime startTime = DateTime.Now;
            strData = "";
            try
            {
                _mux.WaitOne();
                if (CheckPortValid() == false)
                    return false;

                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                strout = strcmd + "\r\n";
                port.Write(strout);
                //Thread.Sleep(delay);
                while (true)
                {
                    if (CheckPortValid())
                    {
                        int temp = port.ReadByte();
                        buf.Add((byte)temp);
                        if (buf.Count > 2 && temp == 0x3E)
                        {
                            if (buf[buf.Count - 3] == 0x0D && buf[buf.Count - 2] == 0x0A)
                            {
                                strData = Encoding.ASCII.GetString(buf.ToArray(), 0, buf.Count);
                                strData = strData.Replace("\r", "").Replace("\n", "").Replace(">", "");
                                return true;
                            }
                        }

                        //iCount = port.Read(buf, 0, 1024);
                        //if (iCount > 0)
                        //{
                        //    strData = Encoding.ASCII.GetString(buf, 0, iCount);
                        //    Array.Clear(buf, 0, 1024);
                        //    strData = strData.Replace("\r", "").Replace("\n", "").Replace(">", "");
                        //    return true;
                        //}
                    }
                    double dwTimeCurrent = (DateTime.Now - startTime).TotalSeconds;
                    if (dwTimeCurrent > 1000)
                        return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                _mux.ReleaseMutex();
            }
        }
       
        public bool Write(string command)
        {
            if (!IsConnected)
                return false;

            var result = false;

            ClearReceivedBuffer();
            try
            {
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                port.Write(command);
                result = true;
            }
            catch (Exception ex)
            {
                ErrorString = $"UC872串口发送数据失败, {ex.Message}";
            }
            return result;
        }

        public bool Read(out string command, int length = -1, int timeout = 2000)
        {
            var receivedData = new List<byte>();
            if (length > 0)
            {
                var reclenght = 0L;

                var sw = System.Diagnostics.Stopwatch.StartNew();

                while (_dataLenght < length)
                {
                    if (sw.ElapsedMilliseconds > timeout)
                        break;
                    if (_dataLenght > reclenght)
                        sw.Restart();
                    reclenght = _dataLenght;
                    if (reclenght < length)
                        System.Threading.Thread.Sleep(100);
                    else
                        break;
                }
            }

            DataAnalysis(receivedData);
            command = Encoding.GetEncoding("iso-8859-1").GetString(receivedData.ToArray());
            receivedData.Clear();

            if (length > 0 && command.Length < length)
            {
                ErrorString = "读取数据超时";
                return false;
            }
            return true;
        }

        public void ClearBuffer()
        {
            port.WriteLine("Sense:F:BC ");
        }
        #endregion

        #region Private Methods

        private void ClearReceivedBuffer()
        {
            var text = default(byte[]);
            while (!_cq.IsEmpty)
            {
                _cq.TryDequeue(out text);
            }
            _dataLenght = 0L;
        }

        ConcurrentQueue<byte[]> _cq = new ConcurrentQueue<byte[]>();
        private long _dataLenght = 0L;

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = port.BytesToRead;
            if (len == 0) return;
            _dataLenght += len;
            byte[] receiveMsg = new byte[len];
            port.Read(receiveMsg, 0, len);
            _cq.Enqueue(receiveMsg);

        }
        private void DataAnalysis(List<byte> receivedData)
        {
            var text = default(byte[]);
            while (!_cq.IsEmpty)
            {
                if (_cq.TryDequeue(out text))
                    receivedData.AddRange(text);
            }
        }

        private bool UC8721_SendCommand(string command, StringBuilder data = null)
        {
            var errorString = "";

            if (!this.IsConnected)
            {
                errorString = "设备未初始或设备已经断开";
                return false;
            }

            command += "\r\n";

            if (!this.Write(command))
            {
                errorString = this.ErrorString;
                return false;
            }
            errorString = this.ErrorString;

            if (data == null)
                return true;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (this.IsConnected)
            {
                if (sw.ElapsedMilliseconds > 2000)
                {
                    sw.Stop();
                    break;
                }
                var tempData = "";
                if (!this.Read(out tempData))
                {
                    sw.Stop();
                    errorString = this.ErrorString;
                    return false;
                }
                data.Append(tempData);
                if (data.ToString().Contains(">"))
                {
                    sw.Stop();
                    errorString = "读取数据成功";
                    return true;
                }
                Thread.Sleep(1);
            }

            errorString = data.Length > 0 ? $"数据错误:{data.ToString()}" : "读取数据超时";
            if (data.Length > 0)
                System.Diagnostics.Debug.WriteLine(BitConverter.ToString(Encoding.GetEncoding("iso-8859-1").GetBytes(data.ToString()), 0));
            return false;
        }

        public void SetParameters(int cw)
        {
            SetTiggerInput(UC872port.EnumTriggrerMode.Smeasure);
            SetPulseType(UC872port.EnumPulseType.HIGH);
            SetPowerUnit(UC872port.EnumPowerUnit.dB);

            var aveTime = 0.1D; //功率计平均时间，根据需求修改
                                
            SetScanModeState("1");//进入扫描模式
            SetAveTime(aveTime);
            SetScanModeState("0");//退出扫描模式
        }

        public bool IsLogging { set; get; } = false;
        public void StartSweep()
        {
            //功率计启动扫描
            data.Clear();
            try
            {
               SetScanModeState("1", data);
                var text = "";
                logging.Clear();
                new TaskFactory().StartNew(() =>
                {
                    IsLogging = true;
                    while (IsLogging)
                    {
                        Read(out text);
                        if (!string.IsNullOrEmpty(text))
                            logging.Append(text);

                        Thread.Sleep(100);
                    }
                });
                data.Clear();
                while (!IsLogging)
                {
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private const int byteLenght = 5;

        public void GetPowermeterData(out double[] powers, int desiredPoint)
        {
            IsLogging = false;
            var text = "";
            if (logging.Length / byteLenght < desiredPoint)
            {
                while (true)
                {
                    Read(out text);
                    if (!string.IsNullOrEmpty(text))
                        logging.Append(text);
                    else
                        break;
                    Thread.Sleep(100);
                }

            }
            data.Clear();
            SetScanModeState("0", data);

            powers = new double[desiredPoint];
            var cache = logging.ToString();
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(cache);
            var receivelen = bytes.Length / byteLenght;

            if (receivelen < desiredPoint)
            {
                throw new Exception("功率计UC872接受到的数据比预期的少");
            }
            for (int i = 0; i < powers.Length; i++)
            {
                try
                {
                    powers[i] = Math.Round(BitConverter.ToSingle(bytes, i * byteLenght), 3);
                }
                catch (Exception)
                {
                }
            }

        }
        #endregion
    }
}
