using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace AWGTestServer.Instruments
{
    class UC872port
    {
        #region Properties

        SerialPort port;
        private StringBuilder data = new StringBuilder();
        public bool IsConnected
        {
            get
            {
                return port != null && port.IsOpen;

            }
        }

        public string ErrorString { get; private set; }

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

        public void SetWavelength(double cw)
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
        //public double[] ReadResult(int PointDesired)
        //{
        //    #region
        //    // 串口读取超时时间，单位ms
        //    //const int READ_TIMEOUT = 2000;

        //    //byte[] byteResult=new byte[PointDesired*2];
        //    //double[] dbResult = new double[PointDesired];
        //    //int byteReceived = 0;
        //    //bool isTimeout = false;

        //    //Stopwatch sw = new Stopwatch();

        //    //port.WriteLine("Sense : F : R?");

        //    //sw.Start();

        //    //while (true)
        //    //{
        //    //    Thread.Sleep(100);

        //    //    var byteInBuffer = port.BytesToRead;
        //    //    port.Read(byteResult, byteReceived, byteInBuffer);
        //    //    byteReceived += byteInBuffer;

        //    //    // 接收到足够的数据点。
        //    //    // 一次Trigger采样一个数据点，一个数据点两个字节，因此 PointDesired * 2
        //    //    if (PointDesired * 2 <= byteReceived)
        //    //        break;

        //    //    // 如果接受到数据，复位超时定时器
        //    //    if(byteInBuffer > 0)
        //    //        sw.Restart();

        //    //    // 判断是否接受超时
        //    //    if(sw.ElapsedMilliseconds > READ_TIMEOUT)
        //    //    {
        //    //        isTimeout = true;
        //    //        break;
        //    //    }
        //    //}

        //    //sw.Stop();

        //    //if (isTimeout)
        //    //    throw new TimeoutException($"没有接收到足够的采样点，期望{PointDesired * 2}字节，实际读取{byteReceived}字节。");
        //    //for (int i = 0; i < PointDesired; i++)
        //    //{
        //    //    string str1 = arrData[i * 2].ToString("X");
        //    //    str1 = Converters.HexToDec(str1);
        //    //    double temp1 = double.Parse(str1);
        //    //    string str2 = arrData[i * 2 + 1].ToString("X");
        //    //    str2 = Converters.HexToDec(str2);
        //    //    double temp2 = double.Parse(str2);
        //    //    double y = ((((temp2 - 128) * 128) + temp1) - 10000) / 100;
        //    //    dbResult[i] = y;
        //    //}
        //    #endregion
        //    int len = 2 * PointDesired;
        //    byte[] arrData = new byte[len];
        //    double[] dbResult = new double[PointDesired];
        //    byte[] buf = new byte[len];
        //    int iCount = 0;
        //    List<byte> bufList = new List<byte>();
            
        //    try
        //    {
        //        _mux.WaitOne();
        //        if (CheckPortValid() == false)
        //            throw new Exception("UC872端口不存在或者已被打开！");
        //        port.DiscardInBuffer();
        //        port.DiscardOutBuffer();
        //        string strout = @"Sense : F : R?";
        //        port.Write(strout);
        //        DateTime startTime = DateTime.Now;
        //        Thread.Sleep(300);

        //        #region 
        //        //while (port.BytesToRead <= 0 && (DateTime.Now - startTime).TotalSeconds < 600)
        //        //{
        //        //    Thread.Sleep(2);
        //        //}

        //        //while ( (DateTime.Now - startTime).TotalSeconds < 2)
        //        //{
        //        //    if (port.BytesToRead > 0)
        //        //    {
        //        //        startTime = DateTime.Now;
        //        //        buf = new byte[port.BytesToRead];
        //        //        iCount += port.Read(buf, 0, buf.Length);
        //        //        bufList.AddRange(buf);
        //        //    }
        //        //    Thread.Sleep(2);
        //        //}
        //        //if (bufList.Count != len)
        //        //{
        //        //    throw new Exception("功率计返回数据长度不正确！");
        //        //}
        //        #endregion
        //        while (port.BytesToRead <= 0 && (DateTime.Now - startTime).TotalSeconds < 10)
        //        {
        //            Thread.Sleep(2);
        //        }

        //        if (port.BytesToRead > 0)
        //        {
        //            while (iCount < len && (DateTime.Now - startTime).TotalSeconds < 10)
        //            {
        //                if (port.BytesToRead > 0)
        //                {
        //                    startTime = DateTime.Now;
        //                    buf = new byte[port.BytesToRead];
        //                    iCount += port.Read(buf, 0, buf.Length);
        //                    bufList.AddRange(buf);

        //                    if (iCount >= len)
        //                    {
        //                        arrData = bufList.ToArray();
        //                    }
        //                }
        //                Thread.Sleep(2);
        //            }
        //        }
        //        else
        //        {
        //            //arrData = bufList.ToArray();
        //            for (int i = 0; i < PointDesired; i++)
        //            {
        //                string str1 = arrData[i * 2].ToString("X");
        //                str1 = Converters.HexToDec(str1);
        //                double temp1 = double.Parse(str1);
        //                string str2 = arrData[i * 2 + 1].ToString("X");
        //                str2 = Converters.HexToDec(str2);
        //                double temp2 = double.Parse(str2);
        //                double y = ((((temp2 - 128) * 128) + temp1) - 10000) / 100;
        //                dbResult[i] = y;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"读取功率计的功率出错！{ex.Message}");
        //    }
        //    finally
        //    {
        //        _mux.ReleaseMutex();
                
        //    }
        //    return dbResult;
        //}

        
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
        #endregion
    }
}
