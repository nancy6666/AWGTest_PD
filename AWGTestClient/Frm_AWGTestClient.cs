using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Design;
using System.IO;
using TestSystem.TestLibrary.Utilities;
using NPlot;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TestSystem.TestLibrary.INI;
using SarchPMS.Business.Draw;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Configuration;
using TestSystem.TestLibrary.Algorithms;
using System.Data.SqlClient;
using SqlFunctions;
using System.IO.Compression;
using System.Text.RegularExpressions;
using SqlFunctions.Class_Test_Data;
using Newtonsoft.Json;
//using SufeiUtil;
//using SufeiUtil.Common;
using Newtonsoft.Json.Converters;
using DevExpress.XtraCharts;

namespace AWGTestClient
{

    public partial class Frm_AWGTestClient : Office2007Form
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        CConfigurationManagement cfg = new CConfigurationManagement();
        IAWGTest awgTest;

        public Frm_AWGTestClient()
        {
            this.EnableGlass = false;
            InitializeComponent();
        }
       const double C = 299792458;
        string m_strPreTemperature = "25";
         int m_dwSamplePoint;
        static double[] _gpdblSweepRate = new double[13] { 0.5, 1.0, 2.0, 5.0, 10.0, 20.0, 40.0, 50.0, 80.0, 100.0, 150.0, 160.0, 200.0 };
        static double[] _gpdblWindowIL = new double[7] { 1270.0, 1310.0, 1490.0, 1550.0, 1577.0, 1625.0, 1650.0 };
        double m_dblStep;
     
        double m_dwStartWavelength;
        double m_dwStopWavelength;
        double ITU_start;
        double ITU_step;
        SqlCommand cmd = new SqlCommand();
        CDatabase db;
        int m_dwInputPortCounts;
        int m_dwOutputPortCounts;
       
        double m_dblPower;
       
        int m_dwOutput;
        bool m_bLLog;
        
        bool bSetLossWindow = true;
       // int MaxChannel = 1;
        DateTime testStart;

        double[] referenceData = new double[1];


        tagAutoWaveform m_stPLCData;
       
        tagPLCData m_stPLCTestResultData;
        tagPLCCriteria m_stCriteria;
        DeviceInfo deviceInfo;

        string  m_strTmplModel,m_strTmplTemp;       
        string strReceive = "";
        int iStation = 999;
        public string strTmplFileName = Directory.GetCurrentDirectory() + @"\Config\client.ini";

        int server_ret = 999; //0初始化 1表示连接成功, 2 表示连接失败,3表示init成功,4表示init失败,5表示zero成功,6表示zero 失败,7表示test成功,8表示test失败
        bool m_bStop;
        DateTime testTime;
        string strTmplName;
        string strTestTemp;

        bool m_bConnect = false;
        bool m_bCLBandRefDone = false;
        bool m_bTestDone = false;       
        bool bRuning = false;
        string m_strOldSN;
        int m_dwTestIndex;
        bool m_bRadioDrawType=true;
          
        CTestSpecCommon specCommon;
        int MaxChannel;

        CCrosstalkSpec crosstalkSpec;
        CWavelengthSpec wavelengthSpec;
        CLossSpec lossSpec;
        CPassbandSpec passbandSpec;
        CPdlSpec pdlSpec;
        CFrequencySpec frequencySpec;

        AWGTestClient awgTestClient;
        delegate void ThreedClearInput();
        delegate void ThreedShowTextDelegate(string Message);
        delegate void ThreedEnableButtonDelegate(bool bEnable);
        delegate void ThreedShowMsgDelegate(string Message,bool bPass);
        delegate void ThreedShowResultDelegate(string[] Message,int [] iPass);
        delegate void ThreedDrawPicture(double[,] RawData, double[] XData, string strTitle, string yAxisTest, string xAxisTest, bool bIL);


        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      
        private void EnableButton(bool bEnable)
        {
            buttonCalibration.Enabled = bEnable;
            checkBoxSerialNum.Enabled = bEnable;
        }
       
        private void Frm_AWGTestClient_Load(object sender, EventArgs e)
        {
            this.textBoxPartNum.Focus();
            double[] xData = { 2, 4, 6, 8, 10 };
            double[] yData = { 0.71,0.85,0.92,0.8,0.75 };
           
            PolyFit fitData = Alg_PolyFit.PolynomialFit(xData, yData, 2);
        
            Alg_DevFromPolyFit.DevFromPolyFitResults myResults = Alg_DevFromPolyFit.DevFromPolyFit(xData, yData, 0, 4, 2);

            Control.CheckForIllegalCrossThreadCalls = false;
          
            SetResultListView();
            awgTestClient = new AWGTestClient(this, m_stPLCData, m_stPLCTestResultData);
          
            string path = Directory.GetCurrentDirectory() + @"\Auto_Template";
            String[] files = Directory.GetFiles(path, "*.csv", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string file = Path.GetFileName(files[i]);
                file = file.Substring(0, file.Length - 4);
               
            }

            comboBoxCLOption.Items.Add("Test");
            comboBoxCLOption.Items.Add("Bonding");
            comboBoxCLOption.Items.Add("Final Assembly");

            comboBoxOOption.Items.Add("Test");
            comboBoxOOption.Items.Add("Bonding");
            comboBoxOOption.Items.Add("Final Assembly");

            cbxProductType.Items.Add("Mux");
            cbxProductType.Items.Add("Demux");

            cbxProductType.SelectedIndex = -1;
            comboBoxCLOption.SelectedIndex = 0;
            comboBoxOOption.SelectedIndex = 0;

            textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            textBoxTemperature.Text = "23";

            string strIP = INIOperationClass.INIGetStringValue(strTmplFileName, "Main", "server", "");
            string strPort = INIOperationClass.INIGetStringValue(strTmplFileName, "Main", "port", "");
            iStation = int.Parse(INIOperationClass.INIGetStringValue(strTmplFileName, "Main", "worknum", ""));
            m_stPLCData = new tagAutoWaveform();
            m_stPLCTestResultData = new tagPLCData();

            IPAddress ip = IPAddress.Parse(strIP);
            IPEndPoint point = new IPEndPoint(ip, int.Parse(strPort));

            try
            {
                client.Connect(point);
               
                m_bConnect = true;

                ShowMsg("Connect successfully!", true);
                SendMsg("connect station" + iStation);

                Thread th = new Thread(ReceiveMsg);
                th.IsBackground = true;
                th.Start();
            }
            catch (Exception ex)
            {
                m_bConnect = false;
                ShowMsg("Connection failed: " + ex.Message,false );
            }
        }

        private void SetResultListView()
        {
            int iWidth1 = 60;
            int iWidth2 = 60;
            this.parametersList.Columns[0].Width = 30;
            this.parametersList.Columns[1].Width = iWidth1;
            this.parametersList.Columns[2].Width = iWidth1;
            this.parametersList.Columns[3].Width = iWidth1;
            this.parametersList.Columns[4].Width = iWidth1;
            this.parametersList.Columns[5].Width = iWidth2;
            this.parametersList.Columns[6].Width = iWidth2;
            this.parametersList.Columns[7].Width = iWidth2;
            this.parametersList.Columns[8].Width = iWidth2;
            this.parametersList.Columns[9].Width = iWidth2;
            this.parametersList.Columns[10].Width = iWidth2;
            this.parametersList.Columns[11].Width = iWidth2;
            this.parametersList.Columns[12].Width = iWidth1;
            this.parametersList.Columns[13].Width = iWidth2;
            this.parametersList.Columns[14].Width = iWidth2;
            this.parametersList.Columns[15].Width = iWidth2;
            this.parametersList.Columns[16].Width = iWidth2;
            this.parametersList.Columns[17].Width = iWidth2;
           
        }
        private void ReceiveMsg()
        {
            while (true)
            {
                try
                {

                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string s = Encoding.UTF8.GetString(buffer, 0, n);
                    if (s.Contains("status"))
                    {
                        ShowWaitingItem(s);
                    }
                    ShowMsg(client.RemoteEndPoint.ToString() + ":" + s, true);
                    strReceive = s;
                    try
                    {
                        server_ret = int.Parse(strReceive);
                    }
                    catch
                    {

                    }
                }
                catch (Exception ex)
                {
                    ShowMsg("Receive Message Fail ! " + ex.Message, false);
                    break;
                }

            }
        }
        private void SendMsg(string Msg)
        {
            if (client != null)
            {
                try
                {
                    ShowMsg("Send Message: " + Msg, true);
                    byte[] buffer = Encoding.UTF8.GetBytes(Msg);
                    client.Send(buffer);
                }
                catch (Exception ex)
                {
                    ShowMsg("Send Message: " + Msg + " Fail ! " + ex.Message,false);
                }
            }
        }

        private void buttonCalibration_Click_1(object sender, EventArgs e)
        {
            if (bRuning)
                return;
            m_bStop = false;
            listView1.Items.Clear();
            listView2.Items.Clear();
            string strMsg;
            string tszValue;            
            m_strTmplModel = textBoxPartNum.Text;
            m_strTmplTemp = textBoxTestTemp.Text;
            if (strTmplName == "")
            {
                MessageBox.Show("Pls input PN!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if ((strTmplName != m_strTmplModel) || (strTestTemp != m_strTmplTemp))
            {
                MessageBox.Show("PN was changed，pls get test condition again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (m_dwStartWavelength < 1250.0 || m_dwStopWavelength > 1640.0 || m_dwStartWavelength >= m_dwStopWavelength)
            {
                MessageBox.Show("The StartWavelength or StopWavelength is wrong, pls check!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
         
            tszValue = INIOperationClass.INIGetStringValue(strTmplFileName, "Main", "TmplName", "XXX");
            if (tszValue == "XXX")
            {
                MessageBox.Show("The calibration file name in client.ini is null! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult diaResult;
            if (tszValue.Contains(strTmplName) && tszValue.Contains(strTestTemp))
            {
                strMsg = "Already calibrated，calibrate again？";
                diaResult = MessageBox.Show(strMsg, "Calibration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                diaResult = DialogResult.Yes;
            }
          
            if (diaResult == DialogResult.Yes)
            {
                Thread threadStartCalibration = new Thread(new ThreadStart(DoCalibration));
                threadStartCalibration.Name = "StartCalibration";
                threadStartCalibration.Start();
                bRuning = true;
            }
        }
        private void DoCalibration()
        {
            awgTestClient.SaveTLSSeting("TLS Setting", m_dwStartWavelength , m_dwStopWavelength, m_dblPower,
                m_dblStep, m_dwOutput, m_bLLog ? 1 : 0, iStation);
            try
            {
                awgTest.StartSweep();
            }
            catch(Exception ex)
            {
                ShowMsg(ex.Message, false);
                throw ex;
            }

            #region calibration NO.1 powermeter

            MessageBox.Show("请将光纤接入第1个功率计，注意此时不接产品!!!");
            string strMsg = "calibrating Powermeter No.1，pls wait . . .";
            ShowMsg(strMsg, true);
            if (!SendCalibrationMsg())
            {
                bRuning = false;
                return;
            }
            strMsg = "Powermeter NO.1 calibration OK!";
            ShowMsg(strMsg, true);
            #endregion
            /* 待功率计的信号线全部串联好，再enable
            #region calibration NO.2 powermeter

            MessageBox.Show("请将光纤接入第2个功率计，注意此时不接产品!!!");
             strMsg = "calibrating Powermeter No.2，pls wait . . .";
            ShowMsg(strMsg, true);
            if (!SendCalibrationMsg())
            {
                bRuning = false;
                return;
            }
            strMsg = "Powermeter NO.2 calibration OK!";
            ShowMsg(strMsg, true);
            #endregion

            #region calibration NO.3 powermeter

            MessageBox.Show("请将光纤接入第3个功率计，注意此时不接产品!!!");
            strMsg = "calibrating Powermeter No.3，pls wait . . .";
            ShowMsg(strMsg, true);
            if (!SendCalibrationMsg())
            {
                bRuning = false;
                return;
            }
            strMsg = "Powermeter NO.3 calibration OK!";
            ShowMsg(strMsg, true);
            #endregion

            #region calibration NO.4 powermeter

            MessageBox.Show("请将光纤接入第4个功率计，注意此时不接产品!!!");
            strMsg = "calibrating Powermeter No.4，pls wait . . .";
            ShowMsg(strMsg, true);
            if (!SendCalibrationMsg())
            {
                bRuning = false;
                return;
            }
            strMsg = "Powermeter NO.4 calibration OK!";
            ShowMsg(strMsg, true);
            #endregion
            */
            EnableTestButton(true);

            awgTestClient.SaveSeting(strTmplFileName, "Main", "TmplName", strTmplName + " " + strTestTemp);
            this.Invoke(new ThreedEnableButtonDelegate(EnableCalibrationButton), new object[] { true });
            this.Invoke(new ThreedEnableButtonDelegate(EnableTestButton), new object[] { true });
            m_bCLBandRefDone = true;

            string strFileName = Directory.GetCurrentDirectory() + String.Format($"\\Data\\Cali_RawData.csv");
            try
            {
                awgTest.ReadSaveCaliData(strFileName);
            }
            catch(Exception ex)
            {
                ShowMsg(ex.Message, false);
                throw ex;
            }
            strMsg = "Read and Calibration Data OK!";
            ShowMsg(strMsg, true);

            m_strOldSN = "";
            m_dwTestIndex = 0;
            bRuning = false;
        }

        private bool SendCalibrationMsg()
        {
            string strMsg;
            bool bFunctionOK = false;
            server_ret = 0;
            string llog = m_bLLog ? "1" : "0";
            SendMsg("Zero " + iStation + ";" + m_dwStartWavelength + ";" + m_dwStopWavelength + ";" + m_dblStep + ";" + m_dblPower + ";" + m_dwOutput + ";" + llog);
            int iCount = 0;

            while (true)
            {
                if (m_bStop)
                    break;
                System.Threading.Thread.Sleep(1000);
               
                iCount++;
                if (server_ret == 5)
                {
                    bFunctionOK = true;
                    break;
                }
                if (server_ret == 6)
                {
                    strMsg = "Calibration error，pls calibrate again";
                    ShowMsg(strMsg, false);
                    MessageBox.Show(strMsg);
                    break;
                }
                if (iCount > 600)
                {
                    strMsg = "Calibration overtime, pls confirm the running status of Server!";
                    ShowMsg(strMsg, false);
                    MessageBox.Show(strMsg);
                    break;
                }
            }
            if (!bFunctionOK)
            {
                bRuning = false;
                return false;
            }
            return true;
       
        }
        private void buttonStopTest_Click(object sender, EventArgs e)
        {
            m_bStop = true;
        }

        private void buttonStartTest_Click(object sender, EventArgs e)
        {
            if (bRuning)
                return;
            string sPN = textBoxPartNum.Text;
            string sTemp = textBoxTestTemp.Text;
            lstviewTestResult.Items.Clear();
            if (strTmplName == "")
            {
                MessageBox.Show("Pls get test condition at first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           string tszValue = INIOperationClass.INIGetStringValue(strTmplFileName, "Main", "TmplName", "XXX");
            if (tszValue == "XXX")
            {
                MessageBox.Show("The calibration file name in client.ini is null! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!tszValue.Contains(strTmplName) || !tszValue.Contains(strTestTemp))
            {
                string strMsg = $"Current PN {sPN} haven't been calibrated! Pls calibrate at first";
                MessageBox.Show(strMsg, "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            m_bStop = false;
            listView1.Items.Clear();
            listView2.Items.Clear();
            db = new CDatabase();       
            bool bFunctionOK;
            bFunctionOK = CheckInputInfo();
            if (!bFunctionOK)
                return;
            try
            {
                string chip = deviceInfo.m_strWaferID + "-" + deviceInfo.m_strChipID;
                string sql = string.Format($"Select * from dbo.plc_production_baseinfo where chip_id = '{chip}'");
                db.Open(out cmd);
                //check if the chip ID have been inserted into databse in travel card process
                cmd.CommandText = sql;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read() == false)
                {
                    string error = string.Format("There is no Chip ID {0} in Database , pls check!", chip);
                    MessageBox.Show(error, "Error");
                    db.Close();
                    return;
                }
                reader.Close();
                //check if the Spec Condition is the newest version
                cmd.Parameters.Clear();

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Temp",
                    DbType = DbType.String,
                    Value = sTemp
                });
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "Pn",
                    DbType = DbType.String,
                    Value = sPN
                });

                cmd.CommandText = "dbo.select_common_spec";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                DataTable dt = ds.Tables[0];

                var specIDNewest = (int)dt.Rows[0]["id"];
                if(specIDNewest>specCommon.SpecID)
                {
                    MessageBox.Show("测试规格有更新，请重新获取测试条件！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowMsg($"Open Database error {ex.Message}", false);
                db.Close();
                return;
            }
            if (!m_bCLBandRefDone)
            {
                 tszValue = INIOperationClass.INIGetStringValue(strTmplFileName, "Main", "TmplName", "XXX");
                if (tszValue == "XXX")
                {
                    MessageBox.Show("The calibration file name in client.ini is null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                m_bCLBandRefDone = true;
              
            }
            testTime = DateTime.Now;
            parametersList.Items.Clear();
            try
            {
                m_bTestDone = false;
                if (m_strOldSN != deviceInfo.m_EditSerialNumber)
                {
                    m_dwTestIndex = 0x00;
                    m_strOldSN = deviceInfo.m_EditSerialNumber;
                }
                else
                    m_dwTestIndex++;
                m_stPLCData.m_bILOrPDL = m_bRadioDrawType;
               
                m_stPLCData.m_pdwILMaxArray = new double[MaxChannel, m_dwSamplePoint];
                m_stPLCData.m_pdwILMinArray = new double[MaxChannel, m_dwSamplePoint];
                m_stPLCData.m_pdwWavelengthArray = new double[m_dwSamplePoint];
                m_stPLCData.m_dwChannelCount = MaxChannel;
                m_stPLCData.m_dwSampleCount = m_dwSamplePoint;
                m_dwInputPortCounts = 1;
                m_dwOutputPortCounts = MaxChannel;

                m_stPLCTestResultData.m_dblStartWL = m_dwStartWavelength;
                m_stPLCTestResultData.m_dblStopWL = m_dwStopWavelength;
                m_stPLCTestResultData.m_dblStep = m_dblStep;

                m_stPLCTestResultData.m_dblPower = m_dblPower;
                m_stPLCTestResultData.m_bOutputPort = m_dwOutput;
                m_stPLCTestResultData.m_bLambdaLog = m_bLLog;

                m_stPLCTestResultData.m_dwSamplePoint = m_dwSamplePoint;
                m_stPLCTestResultData.m_dwChannelCount = MaxChannel;

                awgTestClient.m_dwStep = m_dblStep;

                Thread threadStartTest = new Thread(new ThreadStart(DoTest));
                threadStartTest.Name = "StartTest";
                threadStartTest.Start();
                bRuning = true;
            }
            catch (Exception ex)
            {
                bRuning = false;
                ShowMsg(ex.ToString(), false);
            }
        }
       
        public void  DoTest()
        {
            int dwStartChannel = 0;
            int dwEndChannel = 0;
            int dwTestChannelCount = 0x00;          
            string strFilePath = "";
            SqlCommand cmd;
            bool bFunctionOK = false;
            CTestDataCommon testcommon = new CTestDataCommon();
            testConditionStruct testCondition = new testConditionStruct();

            int lastClassID=0;
            try
            {
                using (CDatabase db = new CDatabase())
                {
                    db.Open(out cmd);
                    testStart = new DateTime();
                    TimeSpan ts;
                    for (int dwInputPortIndex = 0x00; dwInputPortIndex < m_dwInputPortCounts; dwInputPortIndex++)
                    {
                        if (m_bStop)
                            goto Finished;
                        for (int dwOutputPortIndex = 0x00; dwOutputPortIndex < m_dwOutputPortCounts;)
                        {
                            if (m_bStop)
                                break;
                            if (m_dwOutputPortCounts <= MaxChannel)
                            {
                                dwStartChannel = 1 + m_dwOutputPortCounts * dwInputPortIndex;
                                dwEndChannel = m_dwOutputPortCounts + m_dwOutputPortCounts * dwInputPortIndex;
                            }
                            else
                            {
                                dwStartChannel = dwEndChannel + 1;

                                if ((m_dwOutputPortCounts - dwTestChannelCount) < MaxChannel)
                                    dwEndChannel = dwStartChannel + m_dwOutputPortCounts - dwTestChannelCount - 1;
                                else
                                    dwEndChannel = dwStartChannel + MaxChannel - 1;
                            }

                            int iLen = dwEndChannel - dwStartChannel + 1;
                            IniArray(iLen);


                            string strTemp = string.Format("Testing sample Channel {0} - {1}", dwStartChannel, dwEndChannel);

                            ShowMsg(strTemp, true);


                            string strMsg = "Testing，pls wait . . .";
                            testStart = DateTime.Now;

                            ShowMsg(strMsg, true);
                            server_ret = 0;
                            try
                            {
                                awgTest.InitPowermeter();
                                awgTest.StartSweep();
                            }
                            catch (Exception ex)
                            {
                                ShowMsg($"启动功率计扫描状态出错{ex.Message}", false);
                                throw new Exception($"启动功率计扫描状态出错{ex.Message}");
                            }

                            //  SendMsg("test " + iStation);
                            string llog = m_bLLog ? "1" : "0";
                            SendMsg("test " + iStation + ";" + m_dwStartWavelength + ";" + m_dwStopWavelength + ";" + m_dblStep + ";" + m_dblPower + ";" + m_dwOutput + ";" + llog);

                            int iCount = 0;
                            while (true)
                            {
                                if (m_bStop)
                                    break;
                                System.Threading.Thread.Sleep(500);
                                strMsg = strMsg + (" . ");
                                ShowMsg(strMsg, true);
                                iCount++;
                                if (server_ret == 7)
                                {
                                    bFunctionOK = true;
                                    break;
                                }
                                if (server_ret == 8)
                                {
                                    strMsg = "CLBand TLS test error，pls try again";
                                    ShowMsg(strMsg, false);
                                    MessageBox.Show(strMsg);
                                    break;
                                }
                                if (iCount > 600)
                                {
                                    strMsg = "Operation overtime,pls confirm running status of Server!";
                                    ShowMsg(strMsg, false);
                                    MessageBox.Show(strMsg);
                                    break;
                                }
                            }
                            if (!bFunctionOK)
                            {
                                bRuning = false;
                                goto Finished;
                            }
                            DateTime testEnd = DateTime.Now;
                            ts = testEnd.Subtract(testStart);

                            ShowMsg($"CLBand TLS test . OK! Costs {ts.TotalSeconds}s", true);

                            //创建RawData的文件夹 用于存放所有的RawData文件

                            string strTime = testTime.ToString("yyyy-MM-dd-hh-mm");
                            string strStation = m_dwTestIndex.ToString().PadLeft(3, '0');
                            string strFileName = string.Format("SU-{0}-{1}-{2}-{3}-T-{4}-{5}", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID.Substring(0, 2).PadLeft(3, '0'), deviceInfo.m_strChipID.Substring(2, 2).PadLeft(3, '0'), "S" + iStation.ToString().PadLeft(2, '0'), strStation, strTime);
                            string strZipName = strFileName + ".zip";
                            string strPath = Directory.GetCurrentDirectory() + "\\Data\\";
                            string strZipPath = strPath + strZipName;
                            strFilePath = strPath + strFileName + "\\";
                            if (!Directory.Exists(strFilePath))
                            {
                                Directory.CreateDirectory(strFilePath);
                            }

                            // 测试完成后从功率计读取并保存RawData;
                            ShowMsg("Read Test Data from Powermeter...", true);
                            try
                            {
                                awgTest.ReadSaveTestPower($"{strFilePath}\\RawData.csv");
                            }
                            catch (Exception ex)
                            {
                                ShowMsg($"从功率计读取数据出错{ex.Message}", false);
                                throw ex;
                            }

                            string strCaliFile = Directory.GetCurrentDirectory() + String.Format($"\\Data\\Cali_RawData.csv");

                            try
                            {
                                ShowMsg("Read Calibration Data...", true);
                                awgTest.ReadCaliRawData(strCaliFile);

                                File.Copy(strCaliFile, $"{strFilePath}\\Cali_RawData.csv");

                                //获取ILMin和ILMax的数组
                                ShowMsg("Calculate ILMax ILMin Data...", true);
                                awgTest.GetILMinMax(ref m_stPLCData);
                            }
                            catch (Exception ex)
                            {
                                ShowMsg(ex.Message, false);
                                throw ex;
                            }

                            bFunctionOK = awgTestClient.SaveILMinMaxRawData(m_stPLCData, deviceInfo, iStation, testTime, m_dwTestIndex, strFilePath);
                            if (!bFunctionOK)
                            {
                                strMsg = "Save ILMax ILMin Data Failed !!!";
                                ShowMsg(strMsg, false);
                                bRuning = false;
                                MessageBox.Show(strMsg);
                                goto Finished;
                            }
                            //referenceData = new double[MaxChannel];
                            //ShowMsg("Read Ref Data...", true);
                            //bFunctionOK = awgTestClient.ReadRefRawData(ref referenceData, ref m_stPLCData, 0, MaxChannel);
                            //if (!bFunctionOK)
                            //{
                            //    strMsg = "Read Ref Data Failed !!!";
                            //    ShowMsg(strMsg, false);
                            //    bRuning = false;
                            //    MessageBox.Show(strMsg);
                            //    goto Finished;
                            //}

                            DateTime readRawDataEnd = DateTime.Now;
                            ts = readRawDataEnd.Subtract(testEnd);
                            ShowMsg($"Reading raw data costs {ts.TotalSeconds}s", true);


                            ShowMsg("Draw Picture...", true);
                            DrawPicture(m_stPLCData.m_pdwILMaxArray, m_stPLCData.m_pdwWavelengthArray, "ILMax Plot", "Wavelength(nm)", "LossMax(dB)", m_bRadioDrawType);

                            double dblTemperature = 0;
                            string strTmplFileName = $"{cfg.RawDataPath}\\Config\\StationSetting\\AWGTLSSetting{iStation}.ini";

                            string strTempp = awgTestClient.GetSeting(strTmplFileName, "Temperature", "Temperature", "XXX");
                            if (strTempp == "XXX")
                                dblTemperature = 999;
                            else
                                dblTemperature = double.Parse(strTempp);
                            deviceInfo.m_strTemperature = Math.Round(dblTemperature, 2).ToString();
                            this.Invoke(new ThreedShowTextDelegate(ShowTemperature), new object[] { deviceInfo.m_strTemperature });
                            double dblSpecWL;
                            if (specCommon.Is_Wave)
                            {
                                dblSpecWL = ITU_start + 2 * ITU_step;
                            }
                            else
                            {
                                dblSpecWL = C / (specCommon.ITU_Start_Freq - specCommon.ITU_Step_Freq);
                            }


                            int dwOutputPortCount = dwEndChannel - dwStartChannel + 1;
                            bool bUseTETM = false;
                            TimeSpan timeSpan = DateTime.Now - testTime;

                            #region Get parameter values for Database

                            #region Baseinfo
                            testcommon.baseInfo.Chip_id = deviceInfo.m_strWaferID + "-" + deviceInfo.m_strChipID;
                            testcommon.baseInfo.GetRowID(cmd, testcommon.baseInfo.Chip_id);

                            testcommon.baseInfo.Wafer_id = deviceInfo.m_strWaferID;
                            #endregion

                            #region Common
                            testcommon.Baseinfo_id = testcommon.baseInfo.RowID;
                            testcommon.Temperature = deviceInfo.m_strTemperature;

                            testcommon.Input_channel = int.Parse(deviceInfo.m_strInput);
                            testcommon.Sys_loss = 2;

                            testcommon.Tested_by = 7;//*****
                            testcommon.Test_costs = double.Parse(timeSpan.TotalSeconds.ToString());
                            testcommon.Station = "station" + iStation;
                            testcommon.Comment = "Nah";
                            testcommon.Trimming_cnt = 2;
                            testcommon.Last_trimming = 2;

                            #endregion

                            #region Test Data Detail
                            //double[,] pdwMinLossArrayTest;
                            //double[,] pdwMaxLossArrayTest;

                            //  DateTime filterDataStart = DateTime.Now;

                            //    ShowMsg("Filtering test data ...", true);
                            //bFunctionOK = awgTestClient.CalulateILAve(m_stPLCData, 0, dwOutputPortCount, out pdwMinLossArrayTest, out pdwMaxLossArrayTest);

                            //if (!bFunctionOK)
                            //{
                            //    strMsg = "Filtering test data Failed !!!";
                            //    ShowMsg(strMsg, false);
                            //    bRuning = false;
                            //    MessageBox.Show(strMsg);
                            //}
                            DateTime filterDataEnd = DateTime.Now;
                            //ts = filterDataEnd.Subtract(filterDataStart);
                            //ShowMsg($"Filtering test data costs {ts.Seconds}s", true);

                            ShowMsg("Calculate IL PDL ...", true);
                            foreach (var cond in specCommon.lstCondSpec)
                            {
                                testCondition.iTUWL = new double[MaxChannel];
                                testCondition.ITUStep = ITU_step;
                                testCondition.ITUStepFreq = specCommon.ITU_Step_Freq;
                                testCondition.IsWave = specCommon.Is_Wave;
                                for (int iChan = 0; iChan < MaxChannel; iChan++)
                                {
                                    if (specCommon.Is_Wave)
                                    {
                                        testCondition.iTUWL[iChan] = ITU_start + iChan * ITU_step;
                                    }
                                    else
                                    {
                                        testCondition.iTUWL[iChan] = C / (specCommon.ITU_Start_Freq - iChan * specCommon.ITU_Step_Freq);
                                    }
                                }
                                testCondition.AlignIL = cond.Align_il;
                                testCondition.CrossTalkLosWindow = cond.Xtalk_loss_win;
                                testCondition.RippleLossWindow = cond.Ripple_loss_win;
                                testCondition.ILLossWindow = cond.Il_loss_win;
                                bFunctionOK = awgTestClient.CalculateILPDL_New(ref m_stPLCTestResultData, ref m_stPLCData, testCondition, 0, dwOutputPortCount, true, bUseTETM, false);

                                if (!bFunctionOK)
                                {
                                    strMsg = "Calculate ILPDL Failed !!!";
                                    ShowMsg(strMsg, false);
                                    bRuning = false;
                                    MessageBox.Show(strMsg);
                                    return;
                                }
                                //else
                                //{
                                //    ShowMsg("Calulate ILPDL successfully!", true);
                                //}
                                SpecPerClass uniformitySpec = new SpecPerClass();
                                foreach (var spec in specCommon.lstSpecPerClass)
                                {
                                    if (spec.Class_id == cond.Class_id)
                                    {
                                        uniformitySpec = spec;
                                    }
                                }

                                CUniformity uniformity = new CUniformity()
                                {
                                    SpecClassID = cond.Class_id,
                                    Uniformity = m_stPLCTestResultData.m_dblUniformity,
                                    PredictedTemp = double.Parse(m_strPreTemperature),
                                    specUniformity = uniformitySpec
                                };
                                testcommon.lstUniformity.Add(uniformity);
                                for (int n = 0; n < MaxChannel; n++)
                                {
                                    int channel = n + 1;
                                    #region Crosstalk
                                    double temp = Math.Abs(m_stPLCTestResultData.m_dblAXLeft[n]) < Math.Abs(m_stPLCTestResultData.m_dblAXRight[n]) ? m_stPLCTestResultData.m_dblAXLeft[n] : m_stPLCTestResultData.m_dblAXRight[n];

                                    crosstalkSpec = specCommon.GetCrosstalkSpecBySpecClassAndChannel(cond.Class_id, channel);
                                    CCrosstalk crosstalk = new CCrosstalk()
                                    {
                                        Ax = Math.Round(double.Parse(temp.ToString()), 2),
                                        Ax_n = Math.Round(double.Parse(m_stPLCTestResultData.m_dblAXLeft[n].ToString()), 2),
                                        Ax_p = Math.Round(double.Parse(m_stPLCTestResultData.m_dblAXRight[n].ToString()), 2),
                                        Nx = Math.Round(double.Parse(m_stPLCTestResultData.m_dblNX[n].ToString()), 2),
                                        Tax = Math.Round(double.Parse(m_stPLCTestResultData.m_dblTAX[n].ToString()), 2),
                                        Tnx = Math.Round(double.Parse(m_stPLCTestResultData.m_dblTNX[n].ToString()), 2),
                                        Tx = Math.Round(double.Parse(m_stPLCTestResultData.m_dblTX[n].ToString()), 2),
                                        Channel = channel,
                                        Spec_class_id = crosstalkSpec.RawID,
                                        Spec = crosstalkSpec
                                    };
                                    testcommon.lstTestDetail.Add(crosstalk);
                                    #endregion

                                    #region Wavelength

                                    wavelengthSpec = specCommon.GetWavelengthSpecBySpecClassAndChannel(cond.Class_id, channel);

                                    CWavelength wavelength = new CWavelength()
                                    {
                                        Wavelength = Math.Round(double.Parse(m_stPLCTestResultData.m_dblCW[n].ToString()), 2),
                                        Pdw = Math.Round(double.Parse(m_stPLCTestResultData.m_dblPDW[n].ToString()), 2),
                                        Offset = Math.Round(double.Parse(m_stPLCTestResultData.m_dblShift[n].ToString()), 2),
                                        Channel = channel,
                                        Spec_class_id = wavelengthSpec.RawID,
                                        Spec = wavelengthSpec
                                    };
                                    testcommon.lstTestDetail.Add(wavelength);
                                    #endregion

                                    #region Loss

                                    lossSpec = specCommon.GetLossSpecBySpecClassAndChannel(cond.Class_id, channel);
                                    CLoss loss = new CLoss()
                                    {
                                        Max_at_cw = Math.Round(double.Parse(m_stPLCTestResultData.m_dbMax_at_cw[n].ToString()), 2),
                                        Max_at_itu = Math.Round(double.Parse(m_stPLCTestResultData.m_dbMax_at_itu[n].ToString()), 2),
                                        Max_at_lw = Math.Round(double.Parse(m_stPLCTestResultData.m_dblILMax[n].ToString()), 2),
                                        Min_at_cw = Math.Round(double.Parse(m_stPLCTestResultData.m_dbMin_at_cw[n].ToString()), 2),
                                        Min_at_itu = Math.Round(double.Parse(m_stPLCTestResultData.m_dbMin_at_itu[n].ToString()), 2),
                                        Min_at_lw = Math.Round(double.Parse(m_stPLCTestResultData.m_dblILMin[n].ToString()), 2),
                                        Ripple = Math.Round(double.Parse(m_stPLCTestResultData.m_dblRipple[n].ToString()), 2),
                                        Channel = channel,
                                        Spec_class_id = lossSpec.RawID,
                                        Spec = lossSpec

                                    };
                                    testcommon.lstTestDetail.Add(loss);
                                    #endregion

                                    #region Passband

                                    passbandSpec = specCommon.GetPassbandSpecBySpecClassAndChannel(cond.Class_id, channel);
                                    CPassband passband = new CPassband()
                                    {
                                        At_05db = Math.Round(double.Parse(m_stPLCTestResultData.m_dblBW05dB[n].ToString()), 2),
                                        At_1db = Math.Round(double.Parse(m_stPLCTestResultData.m_dblBW1dB[n].ToString()), 2),
                                        At_20db = Math.Round(double.Parse(m_stPLCTestResultData.m_dblBW20dB[n].ToString()), 2),
                                        At_25db = Math.Round(double.Parse(m_stPLCTestResultData.m_dblBW25dB[n].ToString()), 2),
                                        At_3db = Math.Round(double.Parse(m_stPLCTestResultData.m_dblBW3dB[n].ToString()), 2),
                                        Channel = channel,
                                        Spec_class_id = passbandSpec.RawID,
                                        Spec = passbandSpec
                                    };
                                    testcommon.lstTestDetail.Add(passband);
                                    #endregion

                                    #region Pdl

                                    pdlSpec = specCommon.GetPdlSpecBySpecClassAndChannel(cond.Class_id, channel);
                                    CPdl pdl = new CPdl()
                                    {
                                        Pdl_at_ctr = Math.Round(double.Parse(m_stPLCTestResultData.m_dblPDLCRT[n].ToString()), 2),
                                        Pdl_at_itu = Math.Round(double.Parse(m_stPLCTestResultData.m_dblPDLITU[n].ToString()), 2),
                                        Pdl_max = Math.Round(double.Parse(m_stPLCTestResultData.m_dblPDLMax[n].ToString()), 2),
                                        Channel = channel,
                                        Spec_class_id = pdlSpec.RawID,
                                        Spec = pdlSpec
                                    };
                                    testcommon.lstTestDetail.Add(pdl);
                                    #endregion

                                    #region Frequency

                                    frequencySpec = specCommon.GetFrequencySpecBySpecClassAndChannel(cond.Class_id, channel);

                                    CFrequency frequency;

                                    if (m_stPLCTestResultData.m_dblCW[n] == 0)
                                    {
                                        frequency = new CFrequency()
                                        {
                                            Freq = 0,
                                            Offset = 0,
                                            Channel = channel,
                                            Spec_class_id = frequencySpec.RawID,
                                            Spec = frequencySpec
                                        };
                                    }
                                    else
                                    {
                                        frequency = new CFrequency()
                                        {

                                            Freq = Math.Round(double.Parse((1.0 / (m_stPLCTestResultData.m_dblCW[n] / 2.99792457) * 1e5).ToString()), 2),
                                            Offset = Math.Round(double.Parse(Math.Abs(Math.Round(1.0 / (m_stPLCTestResultData.m_dblCW[n] / 2.99792457) * 1e5 - 1.0 / (testCondition.iTUWL[n] / 2.99792457) * 1e5, 3)).ToString()), 2),
                                            Channel = channel,
                                            Spec_class_id = frequencySpec.RawID,
                                            Spec = frequencySpec
                                        };
                                    }
                                    testcommon.lstTestDetail.Add(frequency);
                                    #endregion
                                }
                                lastClassID = cond.Class_id;
                            }
                            DateTime calculateDataEnd = DateTime.Now;
                            ts = calculateDataEnd.Subtract(filterDataEnd);
                            ShowMsg($"Calculating test data costs {ts.TotalSeconds}s", true);

                            #endregion

                            #region RawData
                            //压缩 RawData的文件夹
                            ZipFile.CreateFromDirectory(strFilePath, strZipPath);
                            ReadRawData zipFiles = new ReadRawData(strZipPath);
                            testcommon.rawData.RawData = zipFiles.BinBytes;
                            testcommon.rawData.File_ext = "zip";
                            #endregion

                            #region Reference

                            for (int n = 0; n < MaxChannel; n++)
                            {
                                CReference reference = new CReference()
                                {
                                    Reference = Math.Round(double.Parse(referenceData[n].ToString()), 2),
                                    Channel = n + 1,

                                };
                                testcommon.lstReference.Add(reference);
                            }
                            #endregion

                            #endregion

                            dwTestChannelCount += dwOutputPortCount;
                            dwOutputPortIndex += MaxChannel;
                        }
                    }
                    int[] iPass = new int[23];
                    string[] ParaName = new string[23];

                    for (int i = 0; i < MaxChannel; i++)
                    {
                        for (int k = 0; k < 23; k++)
                            iPass[k] = 1;
                        GetTestResult(ref ParaName, ref iPass, specCommon, lastClassID, i);
                        ShowResult(ParaName, iPass);
                    }
                    DateTime finshedTest = DateTime.Now;
                    ts = finshedTest.Subtract(testStart);
                    ShowMsg($"The whole test process costs {ts.TotalSeconds}s", true);
                    ///保存测试数据到数据库
                    ///
                    if (MessageBox.Show("Save test result to Database?", "Information", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ShowMsg("Save test result....", true);
                        testcommon.SaveAllData(cmd, specCommon);
                        //显示测试数据在每份spec下的pass状态
                        for (int i = 0; i < specCommon.lstCondSpec.Count; i++)
                        {
                            ListViewItem lt = new ListViewItem();
                            lt.Text = specCommon.lstCondSpec[i].Class_name;

                            if (testcommon.lstFinalPf[i].Pf == false)
                                lt.BackColor = Color.Red;
                            else
                                lt.BackColor = Color.Green;

                            lstviewTestResult.Items.Add(lt);
                        }
                        //保存MES数据
                        MES_TEST_DATA mes = new MES_TEST_DATA();
                        mes.Serial_no = testcommon.baseInfo.Chip_id;
                        mes.Test_start = testStart;
                        mes.Test_time = testStart;
                        mes.Current_station = "";
                        mes.Operator = this.textBoxOperator.Text;
                        mes.Part_no = this.textBoxPartNum.Text;
                        mes.Result = testcommon.Pf ? "Pass" : "Fail";
                        mes.Test_no = testcommon.Station;
                        mes.Workshop_id = "PLC";
                        mes.Current_station = "PLC Test";
                        mes.IN_OUT = "OUT";
                        mes.Create_by = this.textBoxOperator.Text;
                        mes.Create_date = DateTime.Now;

                        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                        string post_json = JsonConvert.SerializeObject(mes, Formatting.Indented, timeConverter);
                        string created_by = this.textBoxOperator.Text;
                        db.SaveMesData(post_json, created_by);
                    }
                    /// 保存测试结果到本地

                    //ShowMsg("Save Result ...", true);
                    //bFunctionOK = SaveTestResult(strFilePath, m_stPLCTestResultData, testcommon, testCondition, deviceInfo, iStation, 0, MaxChannel, false, ref strResult);
                    //if (!bFunctionOK)
                    //{
                    //    string strT = "Save Test Result Failed !!!";
                    //    ShowMsg(strT, false);
                    //    MessageBox.Show(strT);
                    //}
                    ///
                }
            }
            catch (Exception ex)
            {
                string strT = "Test Error !!!"+ ex;
                ShowMsg(strT, false);
                db.Close();
                MessageBox.Show(strT);
            }
        Finished:
            this.Invoke(new ThreedClearInput(ClearInput));//, new object[] { iPass });
            this.Invoke(new ThreedEnableButtonDelegate(EnableCalibrationButton), new object[] { true });
            this.Invoke(new ThreedEnableButtonDelegate(EnableTestButton), new object[] { true });
          
            ShowMsg("Test Done !!!", true);

            MessageBox.Show("Test Done", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //ShowMsg("测试操作完成", true);

            m_bTestDone = true;
            bRuning = false;
            bFunctionOK = true;
            db.Close();
            //return bFunctionOK;.ToString("####.00");
        }
        /// <summary>
        /// 读取RawData的二进制文件
        /// </summary>
        public class ReadRawData
        {
            /// <summary>
            /// 读取RawData的二进制文件
            /// </summary>
            /// <param name="fileName">二进制文件名</param>
            public ReadRawData(string fileName)
            {
                FileInfo info = new FileInfo(fileName);
                FileStream fs = new FileStream(fileName, FileMode.Open);
                this.BinBytes = new byte[info.Length];
                fs.Read(this.BinBytes, 0, this.BinBytes.Length);
                fs.Close();
            }
            public byte[] BinBytes
            {
                private set;
                get;
            }
        }
        private void IniArray(int iLen)
        {
            m_stPLCTestResultData.m_dblCW = new double[iLen];
            m_stPLCTestResultData.m_dblPDW = new double[iLen];
            m_stPLCTestResultData.m_dblShift = new double[iLen];
            m_stPLCTestResultData.m_dblILMin = new double[iLen];
            m_stPLCTestResultData.m_dblILMax = new double[iLen];
            m_stPLCTestResultData.m_dblILITU = new double[iLen];
            m_stPLCTestResultData.m_dblRipple = new double[iLen];
            m_stPLCTestResultData.m_dblPDLCRT = new double[iLen];
            m_stPLCTestResultData.m_dblPDLMax = new double[iLen];
            m_stPLCTestResultData.m_dblBW05dB = new double[iLen];
            m_stPLCTestResultData.m_dblBW1dB = new double[iLen];
            m_stPLCTestResultData.m_dblBW3dB = new double[iLen];
            m_stPLCTestResultData.m_dblBW20dB = new double[iLen];
            m_stPLCTestResultData.m_dblBW25dB = new double[iLen];
            m_stPLCTestResultData.m_dblBW30dB = new double[iLen];
            m_stPLCTestResultData.m_dblPDLITU = new double[iLen];
            m_stPLCTestResultData.m_dblBWCust1 = new double[iLen];
            m_stPLCTestResultData.m_dblBWCust2 = new double[iLen];
            m_stPLCTestResultData.m_dblAXLeft = new double[iLen];
            m_stPLCTestResultData.m_dblAXRight = new double[iLen];
            m_stPLCTestResultData.m_dblTAX = new double[iLen];
            m_stPLCTestResultData.m_dblNX = new double[iLen];
            m_stPLCTestResultData.m_dblTX = new double[iLen];
            m_stPLCTestResultData.m_dblTXAX = new double[iLen];
            m_stPLCTestResultData.m_dblTNX = new double[iLen];
            m_stPLCTestResultData.m_dbMax_at_cw = new double[iLen];
            m_stPLCTestResultData.m_dbMax_at_itu = new double[iLen];
            m_stPLCTestResultData.m_dbMin_at_cw = new double[iLen];
            m_stPLCTestResultData.m_dbMin_at_itu = new double[iLen];
            m_stPLCTestResultData.m_dblRipple = new double[iLen];
            m_stCriteria.m_dblItuWL = new double[iLen];
        }

        
        private void GetTestResult(ref string[] paraName, ref int[] iPass, CTestSpecCommon specCommon ,int lastClassID,int iChan)
        {
            paraName = new string[] {(iChan+1).ToString (), m_stCriteria.m_dblItuWL[iChan].ToString("###0.000"), m_stPLCTestResultData.m_dblCW[iChan].ToString("###0.000"), m_stPLCTestResultData.m_dblPDW[iChan].ToString("###0.000"), m_stPLCTestResultData.m_dblShift[iChan].ToString("###0.000"),
                    m_stPLCTestResultData.m_dblILMin[iChan].ToString("###0.00"),m_stPLCTestResultData.m_dblILMax[iChan].ToString("###0.00"),m_stPLCTestResultData.m_dblILITU[iChan].ToString("###0.00"),m_stPLCTestResultData.m_dblRipple[iChan].ToString("###0.00"),m_stPLCTestResultData.m_dblPDLITU[iChan].ToString("###0.00"),
                    m_stPLCTestResultData.m_dblPDLCRT[iChan].ToString("###0.00"),m_stPLCTestResultData.m_dblPDLMax[iChan].ToString("###0.00"),m_stPLCTestResultData.m_dblBW05dB[iChan].ToString("###0.000"),m_stPLCTestResultData.m_dblBW1dB[iChan].ToString("###0.000"),m_stPLCTestResultData.m_dblBW3dB[iChan].ToString("###0.000"),
                    m_stPLCTestResultData.m_dblBW20dB[iChan].ToString("###0.000"),m_stPLCTestResultData.m_dblBW25dB[iChan].ToString("###0.000"),m_stPLCTestResultData.m_dblBW30dB[iChan].ToString("###0.000")};

            crosstalkSpec = specCommon.GetCrosstalkSpecBySpecClassAndChannel(lastClassID, iChan+1);
            wavelengthSpec = specCommon.GetWavelengthSpecBySpecClassAndChannel(lastClassID, iChan+1);
            lossSpec = specCommon.GetLossSpecBySpecClassAndChannel(lastClassID, iChan+1);
            passbandSpec = specCommon.GetPassbandSpecBySpecClassAndChannel(lastClassID, iChan+1);
            pdlSpec = specCommon.GetPdlSpecBySpecClassAndChannel(lastClassID, iChan+1);
            frequencySpec = specCommon.GetFrequencySpecBySpecClassAndChannel(lastClassID, iChan+1);

            //if (m_stPLCTestResultData.m_dblCW[iChan] > pdlSpec.Pdl_at_ctr_min && m_stPLCTestResultData.m_dblCW[iChan] < pdlSpec.Pdl_at_ctr_max)
            //    iPass[2] = 0;
            if (m_stPLCTestResultData.m_dblPDW[iChan] < wavelengthSpec.Pdw_min || m_stPLCTestResultData.m_dblPDW[iChan] > wavelengthSpec.Pdw_max)
                iPass[3] = 0;

            if (m_stPLCTestResultData.m_dblShift[iChan] < wavelengthSpec.Offset_min || m_stPLCTestResultData.m_dblShift[iChan] > wavelengthSpec.Offset_max)           
                iPass[4] = 0;

            if (m_stPLCTestResultData.m_dblILMin[iChan] < lossSpec.Min_at_lw_min || m_stPLCTestResultData.m_dblILMin[iChan] > lossSpec.Min_at_lw_max)
                iPass[5] = 0;
            if (m_stPLCTestResultData.m_dblILMax[iChan] < lossSpec.Max_at_lw_min || m_stPLCTestResultData.m_dblILMax[iChan] > lossSpec.Max_at_lw_max)
                iPass[6] = 0;
            if (m_stPLCTestResultData.m_dblILITU[iChan] < lossSpec.Max_at_itu_min || m_stPLCTestResultData.m_dblILITU[iChan] > lossSpec.Max_at_itu_max)
                iPass[7] = 0;
            if (m_stPLCTestResultData.m_dblRipple[iChan] < lossSpec.Ripple_min || m_stPLCTestResultData.m_dblRipple[iChan] > lossSpec.Ripple_max)
                iPass[8] = 0;
            if (m_stPLCTestResultData.m_dblPDLITU[iChan] < pdlSpec.Pdl_at_itu_min || m_stPLCTestResultData.m_dblPDLITU[iChan] > pdlSpec.Pdl_at_itu_max)
                iPass[9] = 0;
            if (m_stPLCTestResultData.m_dblPDLCRT[iChan] < pdlSpec.Pdl_at_ctr_min || m_stPLCTestResultData.m_dblPDLCRT[iChan] > pdlSpec.Pdl_at_ctr_max)
                iPass[10] = 0;
            if (m_stPLCTestResultData.m_dblPDLMax[iChan] < pdlSpec.Pdl_max_min || m_stPLCTestResultData.m_dblPDLMax[iChan] > pdlSpec.Pdl_max_max)
                iPass[11] = 0;
            if (m_stPLCTestResultData.m_dblBW05dB[iChan] < passbandSpec.At_05db_min || m_stPLCTestResultData.m_dblBW05dB[iChan] > passbandSpec.At_05db_max)
                iPass[12] = 0;
            if (m_stPLCTestResultData.m_dblBW1dB[iChan] < passbandSpec.At_1db_min || m_stPLCTestResultData.m_dblBW1dB[iChan] > passbandSpec.At_1db_max)
                iPass[13] = 0;
            if (m_stPLCTestResultData.m_dblBW3dB[iChan] < passbandSpec.At_3db_min || m_stPLCTestResultData.m_dblBW3dB[iChan] > passbandSpec.At_3db_max)
                iPass[14] = 0;
            if (m_stPLCTestResultData.m_dblBW20dB[iChan] < passbandSpec.At_20db_min || m_stPLCTestResultData.m_dblBW20dB[iChan] > passbandSpec.At_20db_max)
                iPass[15] = 0;
            if (m_stPLCTestResultData.m_dblBW25dB[iChan] < passbandSpec.At_25db_min || m_stPLCTestResultData.m_dblBW25dB[iChan] > passbandSpec.At_25db_max)
                iPass[16] = 0;
            //if (m_stPLCTestResultData.m_dblBW30dB[iChan] < passbandSpec.At_30db_min || m_stPLCTestResultData.m_dblBW20dB[iChan] > passbandSpec.At_20db_max)
            //    iPass[17] = 0;
            //if (m_stPLCTestResultData.m_dblAXLeft[iChan] < crosstalkSpec.Ax_n_min || m_stPLCTestResultData.m_dblAXLeft[iChan] > crosstalkSpec.Ax_n_max)
            //    iPass[18] = 0;
            //if (m_stPLCTestResultData.m_dblAXRight[iChan] < crosstalkSpec.Ax_p_min || m_stPLCTestResultData.m_dblAXRight[iChan] > crosstalkSpec.Ax_p_max)
            //    iPass[19] = 0;
            //if (m_stPLCTestResultData.m_dblNX[iChan] < crosstalkSpec.Nx_min || m_stPLCTestResultData.m_dblNX[iChan] > crosstalkSpec.Nx_max)
            //    iPass[20] = 0;
            //if (m_stPLCTestResultData.m_dblTX[iChan] < crosstalkSpec.Tx_min || m_stPLCTestResultData.m_dblTX[iChan] > crosstalkSpec.Tx_max)
            //    iPass[21] = 0;
            //if (m_stPLCTestResultData.m_dblTXAX[iChan] < crosstalkSpec.Tax_min || m_stPLCTestResultData.m_dblTXAX[iChan] > crosstalkSpec.Tax_max)
            //    iPass[22] = 0;
        }

        public bool CheckInputInfo()
        {
            deviceInfo.m_EditSerialNumber = textBoxSerialNum.Text;
            deviceInfo.m_EditOperator = textBoxOperator.Text;
            deviceInfo.m_strTestDate = textBoxDate.Text;
            deviceInfo.m_strTemperature = textBoxTemperature.Text;
            deviceInfo.m_strInput = textBoxInput.Text;
            deviceInfo.m_strChipID = textBoxChipID.Text;
            deviceInfo.m_strWaferID = textBoxSerialNum.Text;
            deviceInfo.m_strTestType = comboBoxCLOption.Text;
            deviceInfo.m_dwWSIndex = iStation.ToString();
            deviceInfo.m_strPartNum = textBoxPartNum.Text;
           // deviceInfo.m_strInputTemperature = textBoxTestTemp.Text;
            if (deviceInfo.m_EditSerialNumber == "" || deviceInfo.m_EditOperator == "" || deviceInfo.m_strTestDate == "" ||
    deviceInfo.m_strTemperature == "" || deviceInfo.m_strInput == "" || deviceInfo.m_strChipID == "" || deviceInfo.m_strPartNum == "")
            {
                MessageBox.Show("The product's information can't be empty, pls complete!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!Regex.IsMatch(deviceInfo.m_strWaferID, cfg.WaferFormat))
            {
                MessageBox.Show("The format of wafer is wrong, pls check!");
                return false;
            }
            if (!Regex.IsMatch(deviceInfo.m_strChipID, cfg.ChipIDFormat))
            {
                MessageBox.Show("The format of ChipID is wrong, pls check!");
                return false;
            }
          
            string pattern = @"\d{1}";
            if (!Regex.IsMatch(deviceInfo.m_strInput, pattern))
            {
                MessageBox.Show("The format of Input is wrong，pls check", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            for (int dwIndex = 0x00; dwIndex < MaxChannel; dwIndex++)
            {
                string strFilePathName =Directory .GetCurrentDirectory ()+ string.Format("\\Reference\\Ref_CH{0}.csv", dwIndex + 1);

                if (!File.Exists(strFilePathName))
                {
                    MessageBox.Show($"There is no Reference file {strFilePathName}!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        public void ShowResult(string[] Msg,int [] iPass)
        {
            this.Invoke(new ThreedShowResultDelegate(setParameterDetailRaw), new object[] { Msg, iPass });//, new object[] { iPass });
        }
        private void DisplayPicture(double[,] RawData, double[] XData, string strTitle, string xAxisTest, string yAxisTest,bool bIL)
        {
            //DrawingCurve dc = new DrawingCurve();
            //pictureBox1.Image = dc.DrawImage(RawData, XData, strTitle, xAxisTest, yAxisTest,bIL, pictureBox1.Width, pictureBox1.Height);
            DrawLineDev(RawData, XData);
        }
        public void DrawLineDev(double[,] Rawdata,double[] XData)
        {
            try
            {
                Series ILWave = this.ILChart.Series[0];
                ILWave.Points.Clear();
                
                for(int i=0;i<m_dwSamplePoint;i++)
                {
                    ILWave.Points.Add(new SeriesPoint(XData[i],Rawdata[0,i]));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void ClearInput()
        {
            if (!checkBoxOperator.Checked)
                checkBoxOperator.Text = "";
            if (!checkBoxSerialNum.Checked)
                textBoxSerialNum.Text = "";
           
            if (!checkBoxInput.Checked)
                textBoxInput.Text = "";
         
            textBoxChipID.Text = "";
        }
        public void DrawPicture(double[,] RawData, double[] XData, string strTitle, string xAxisTest, string yAxisTest, bool bIL)
        {
            this.Invoke(new ThreedDrawPicture (DisplayPicture), new object[] { RawData, XData, strTitle, xAxisTest, yAxisTest,bIL });//, new object[] { iPass });
        }

        public void ShowMsg(string msg,bool bPass)
        {
            this.Invoke(new ThreedShowMsgDelegate(ShowMsgg), new object[] { msg,bPass });
        }
        public void EnableCalibrationButton(bool bEnable)
        {
            buttonCalibration.Enabled = bEnable;
        }
        public void EnableTestButton(bool bEnable)
        {
            buttonStartTest.Enabled = bEnable;
        }
       
        void ShowTemperature(string msg)
        {
            textBoxTemperature.Text = msg;
        }
      
        void ShowMsgg(string msg,bool bPass)
        {
            listView1.BeginUpdate();
            try
            {
                ListViewItem lvi = null;
                string temp = "";
                if (msg.Contains("."))
                {
                    int iPos = msg.IndexOf('.');
                    temp = msg.Substring(0, iPos - 1);
                    lvi = listView1.FindItemWithText(temp);
                    if (lvi != null)
                    {
                        listView1.Items.Remove(lvi);
                    }
                }
                lvi = new ListViewItem(msg);
                listView1.Items.Add(lvi);
                //lvi.UseItemStyleForSubItems = false;

                if (!bPass)
                    lvi.ForeColor = Color.Red;
                else
                    lvi.ForeColor = Color.Green;
            }
            finally
            {
                listView1.EndUpdate();
            }
        }
        void ShowWaitingItem(string msg)
        {
            listView2.Items.Clear();
            if (msg != "")
            {
                string[] temp = msg.Split(';');
                listView2.BeginUpdate();
                try
                {
                    ListViewItem lvi = null;
                    for (int i = 1; i < temp.Length; i++)
                    {
                        lvi = new ListViewItem(temp[i]);
                        listView2.Items.Add(lvi);
                    }
                }
                finally
                {
                    listView2.EndUpdate();
                }
            }
        }

        private void setParameterDetailRaw(string[] strResult,int [] iPass)
        {
            parametersList.BeginUpdate();
            try
            {
                ListViewItem lvi = null;
                lvi = new ListViewItem(strResult);
                parametersList.Items.Add(lvi);
                lvi.UseItemStyleForSubItems = false;
                for (int i = 0; i < strResult.Length; i++)
                {
                    if (iPass[i] == 0)
                        lvi.SubItems[i].ForeColor = Color.Red;
                }
            }
            finally
            {
                parametersList.EndUpdate();
            }
        }

        private void Frm_AWGTestClient_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
        private void KillProcess(string ProcessName)
        {
            Process[] allproc = Process.GetProcesses();  //得到系统进程信息
            foreach (Process proc in allproc)
            {
                if (proc.ProcessName == ProcessName)
                {
                    proc.Kill();
                    break;
                }
            }
        }
        private void Frm_AWGTestClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bRuning)
            {
                e.Cancel = true;
            }
            else
            {
                m_bStop = true;
                SendMsg("disconnect station" + iStation);
            }
        }

       
        private void Page_Load(object sender, System.EventArgs e)
        {
            // 在此处放置用户代码以初始化页面
            String DBConnStr;
            DataSet MyDataSet = new DataSet();
            SqlDataAdapter DataAdapter = new SqlDataAdapter();
            DBConnStr = ConfigurationManager.AppSettings["ConnectString"];
            System.Data.SqlClient.SqlConnection myConnection = new System.Data.SqlClient.SqlConnection(DBConnStr);
            if (myConnection.State != ConnectionState.Open)
            {
                myConnection.Open();
            }
            System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand("P_Test", myConnection);
            myCommand.CommandType = CommandType.StoredProcedure;
            //添加输入查询参数、赋予值
            myCommand.Parameters.Add("@Name", SqlDbType.VarChar);
            myCommand.Parameters["@Name"].Value = "A";

            //添加输出参数
            myCommand.Parameters.Add("@Rowcount", SqlDbType.Int);
            myCommand.Parameters["@Rowcount"].Direction = ParameterDirection.Output;


            myCommand.ExecuteNonQuery();
            DataAdapter.SelectCommand = myCommand;

            if (MyDataSet != null)
            {
                DataAdapter.Fill(MyDataSet, "table");
            }

            //DataGrid1.DataSource = MyDataSet;
            //DataGrid1.DataBind();
            ////得到存储过程输出参数
            //Label1.Text = myCommand.Parameters["@Rowcount"].Value.ToString();

            if (myConnection.State == ConnectionState.Open)
            {
                myConnection.Close();
            }

        }
      
        private void buttonGetTestCondition_Click(object sender, EventArgs e)
        {
            strTmplName = textBoxPartNum.Text;
            strTestTemp = textBoxTestTemp.Text;
            SqlCommand cmd;
            db = new CDatabase();
            if (strTmplName == "" || strTestTemp=="")
            {
                MessageBox.Show("请输入PN，测试温度", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(!Regex.IsMatch(strTmplName,cfg.PNNameFormat))
            {
                MessageBox.Show("The fomart of PN is error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {

                if (db.Open(out cmd))
                {
                    specCommon = new CTestSpecCommon(cmd, strTmplName, strTestTemp);
                    MaxChannel = specCommon.MaxChannel;
                    textBoxStartWavelength.Text = specCommon.Sweep_start.ToString();
                    textBoxStopWavelength.Text = specCommon.Sweep_end.ToString();
                    textBoxPower.Text = specCommon.Laser_output_pwr.ToString();

                    m_dwStartWavelength = double.Parse(specCommon.Sweep_start.ToString());
                    m_dwStopWavelength = double.Parse(specCommon.Sweep_end.ToString());
                    m_dblPower = double.Parse(specCommon.Laser_output_pwr.ToString());
                    m_dblStep = double.Parse(specCommon.Sweep_step.ToString());
                    // m_dwStepIndex = int.Parse(specCommon.Sweep_step.ToString());
                    //  m_dblStep = _gpdblSweepRate[m_dwStepIndex] * 0.3*Math.Pow(10,-3);
                    txtbSweepStep.Text = $"{m_dblStep}nm";
                    m_bLLog = specCommon.Llog;
                    ITU_start = specCommon.ITU_Start;
                    ITU_step = specCommon.ITU_Step;

                    awgTestClient.CHANNEL_COUNT = MaxChannel;

                    referenceData = new double[MaxChannel];
                    double mm = (m_dwStopWavelength - m_dwStartWavelength) / m_dblStep + 1;
                    mm = Math.Ceiling(mm);
                    m_dwSamplePoint = int.Parse(mm.ToString());

                    awgTestClient.m_dwSamplePoint = m_dwSamplePoint;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} Pls confirm PN and Temperature! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                db.Close();
            }
           
            if (cbxProductType.SelectedIndex == -1)
            {
                MessageBox.Show("请选择产品类型！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                try
                {
                    if (cbxProductType.SelectedItem.ToString().ToUpper()=="MUX")
                    {
                        awgTest = new MuxTest();
                    }
                    else if (cbxProductType.SelectedItem.ToString().ToUpper()=="DEMUX")
                    {
                        awgTest = new DemuxTest();
                    }
                    awgTest.StopWavelength = m_dwStopWavelength;
                    awgTest.StartWavelength = m_dwStartWavelength;
                    awgTest.StepWavelength = m_dblStep;
                    awgTest.SamplingPoint = m_dwSamplePoint;
                    awgTest.MaxChannel = MaxChannel;
                }
                catch (Exception ex)
                {
                    throw new Exception($"功率计初始化出错{ex.Message}");
                }
            }
        }

        private void btnTETMCalcu_Click(object sender, EventArgs e)
        {
            if (bRuning)
                return;
            if (!bSetLossWindow)
            {
                MessageBox.Show("Please Set CrossTalk, IL, Ripple LossWindow in Templete File !!!");
                return;
            }
          
            int lastClassID = 0;
            string strFileRawData = Directory.GetCurrentDirectory() + string.Format("\\Data\\RawData_Station{0}.csv", iStation);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\Data";
            dlg.Title = "请选择参与计算的文件";

            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "All File (.*)*.*|*.*"; // Filter files by extension 

            // == Process open file dialog box results 
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                strFileRawData = dlg.FileName;
            }
            else
                return;
            string sDirectory = Path.GetDirectoryName(strFileRawData);
            string sFileName = Path.GetFileName(strFileRawData);
            string[] strTempp = sFileName.Split('-');
           
            listView1.Items.Clear();
            listView2.Items.Clear();
            parametersList.Items.Clear();

            bool bFunctionOK = false;

            testTime = DateTime.Now;

           

            double mm = (m_dwStopWavelength - m_dwStartWavelength) / m_dblStep + 1;
            mm = Math.Ceiling(mm);
            m_dwSamplePoint = int.Parse(mm.ToString());
          
            awgTestClient.m_dwSamplePoint = m_dwSamplePoint;
           // m_dwChannelCounts = MaxChannel;
            m_stPLCData.m_bILOrPDL = m_bRadioDrawType;
           
            m_stPLCData.m_pdwWavelengthArray = new double[m_dwSamplePoint];
            m_stPLCData.m_dwChannelCount = MaxChannel;
          
            m_stPLCData.m_pdwILMinArray = new double[MaxChannel, m_dwSamplePoint];
            m_stPLCData.m_pdwILMaxArray = new double[MaxChannel, m_dwSamplePoint];
            m_stPLCData.m_dwChannelCount = MaxChannel;
            m_stPLCData.m_dwSampleCount = m_dwSamplePoint;

            m_stPLCTestResultData.m_dblStartWL = m_dwStartWavelength;
            m_stPLCTestResultData.m_dblStopWL = m_dwStopWavelength;
            m_stPLCTestResultData.m_dblStep = m_dblStep;
            int dwStartChannel = 1;
            int dwEndChannel = MaxChannel;

            int iLen = MaxChannel;
            IniArray(iLen);

            string strMsg = "";
            awgTestClient.CHANNEL_COUNT = MaxChannel;
            bFunctionOK = awgTestClient.ReadDatILMaxMinData(strFileRawData, ref m_stPLCData);
            if (!bFunctionOK)
            {
                strMsg = "Read ILMax ILMin Dat File Failed !!!";
                MessageBox.Show(strMsg);
                ShowMsg(strMsg, false);
                return;
            }
            double dblTemperature = 0;

            deviceInfo.m_strTemperature = Math.Round(dblTemperature, 2).ToString();

            ShowMsg("Draw Picture...", true);
            DrawPicture(m_stPLCData.m_pdwILMaxArray, m_stPLCData.m_pdwWavelengthArray, "ILMax Plot", "Wavelength(nm)", "LossMax(dB)", m_bRadioDrawType);


            ShowMsg("Calculate IL PDL ...", true);
            int dwOutputPortCount = dwEndChannel - dwStartChannel + 1;
            testConditionStruct testCondition = new testConditionStruct();

            foreach (var cond in specCommon.lstCondSpec)
            {
                testCondition.iTUWL = new double[MaxChannel];
                testCondition.ITUStep = ITU_step;
                testCondition.ITUStepFreq = specCommon.ITU_Step_Freq;
                testCondition.IsWave = specCommon.Is_Wave;
                for (int iChan = 0; iChan < MaxChannel; iChan++)
                {
                    if (specCommon.Is_Wave)
                    {
                        testCondition.iTUWL[iChan] = ITU_start + iChan * ITU_step;
                    }
                    else
                    {
                        testCondition.iTUWL[iChan] = C / (specCommon.ITU_Start_Freq - iChan * specCommon.ITU_Step_Freq);
                    }
                }
                testCondition.AlignIL = cond.Align_il;
                testCondition.CrossTalkLosWindow = cond.Xtalk_loss_win;
                testCondition.RippleLossWindow = cond.Ripple_loss_win;
                testCondition.ILLossWindow = cond.Il_loss_win;
                bFunctionOK = awgTestClient.CalculateILPDL_New(ref m_stPLCTestResultData, ref m_stPLCData, testCondition, 0, dwOutputPortCount, true, false, false);
                if (!bFunctionOK)
                {
                    strMsg = "Calculate ILPDL Failed !!!";
                    ShowMsg(strMsg, false);
                    bRuning = false;
                    MessageBox.Show(strMsg);
                }
                lastClassID = cond.Class_id;
            }           
            int[] iPass = new int[18];
            string[] ParaName = new string[18];

            for (int i = 0; i < MaxChannel; i++)
            {
                for (int k = 0; k < 18; k++)
                    iPass[k] = 1;
                GetTestResult(ref ParaName, ref iPass, specCommon, lastClassID, i);

                ShowResult(ParaName, iPass);
            }

            ShowMsg("Calculate Finished !!!", true);
        }

        private void btnILArrayCal_Click(object sender, EventArgs e)
        {
            bool bFunctionOK;
            string strMsg;
            string strFileRawData;
            double mm = (m_dwStopWavelength - m_dwStartWavelength) / m_dblStep + 1;
            mm = Math.Ceiling(mm);
            m_dwSamplePoint = int.Parse(mm.ToString());

            awgTestClient.m_dwSamplePoint = m_dwSamplePoint;
           
            m_stPLCData.m_pdwILMaxArray = new double[MaxChannel, m_dwSamplePoint];
            m_stPLCData.m_pdwILMinArray = new double[MaxChannel, m_dwSamplePoint];
            m_stPLCData.m_pdwWavelengthArray = new double[m_dwSamplePoint];
            m_stPLCData.m_dwChannelCount = MaxChannel;
           
            m_dwInputPortCounts = 1;
            m_dwOutputPortCounts = MaxChannel;
            m_stPLCData.m_dwSampleCount = m_dwSamplePoint;
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择RawData所在文件夹";
            
            // == Process open file dialog box results 
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dlg.SelectedPath))
                {
                   MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
            }
            else
                return;
            string sDirectory = dlg.SelectedPath;

             strFileRawData = $"{sDirectory}\\RawData.csv";
            bFunctionOK = awgTestClient.ReadRawData(strFileRawData);
            if (!bFunctionOK)
            {
                strMsg = "Read Raw Data Failed !!!";
                MessageBox.Show(strMsg);
                ShowMsg(strMsg, false);
            }

            strFileRawData = $"{sDirectory}\\Cali_RawData.csv";
            bFunctionOK = awgTestClient.ReadCaliRawData(strFileRawData);
            if (!bFunctionOK)
            {
                strMsg = "Read Calibration Raw Data Failed !!!";
                MessageBox.Show(strMsg);
                ShowMsg(strMsg, false);
            }

            awgTestClient.GetILMinMax(ref m_stPLCData);
            deviceInfo.m_EditSerialNumber = "B82303-23";
            
            deviceInfo.m_strChipID = "0101";
          
            iStation = 1;
            string strSavePath = $"{sDirectory}\\";
             bFunctionOK = awgTestClient.SaveILMinMaxRawData(m_stPLCData, deviceInfo, iStation, testTime, m_dwTestIndex, strSavePath);
            if (!bFunctionOK)
            {
                strMsg = "Save ILMax ILMin Data Failed !!!";
                ShowMsg(strMsg, false);
                bRuning = false;
                MessageBox.Show(strMsg);
               
            }
        }

        

        public bool SaveTestResult(string sDirectory, tagPLCData pstResultData, CTestDataCommon testCommon, testConditionStruct testCondition , DeviceInfo deviceInfo, int iStation, int dwStartChannel, int dwEndChannel, bool bUseTETM, ref string strResult)
        {
            double dblSpecWL;
            string strTime = testTime.ToString("yyyy-MM-dd-hh-mm");
            string strStation = m_dwTestIndex.ToString().PadLeft(3, '0');

            string strXLSName;
            if (bUseTETM)
                strXLSName = string.Format("SU-TETM-{0}-{1}-{2}-{3}-T-{4}-{5}.xls", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID.Substring(0, 2).PadLeft(3, '0'), deviceInfo.m_strChipID.Substring(2, 2).PadLeft(3, '0'), "S" + iStation.ToString().PadLeft(2, '0'), strStation, strTime);

            //strXLSName = string.Format("SU-TETM-{0}-{1}-T-{2}-{3}.xls", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID, strStation, strTime);
            else
                strXLSName = string.Format("SU-{0}-{1}-{2}-{3}-T-{4}-{5}.xls", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID.Substring(0, 2).PadLeft(3, '0'), deviceInfo.m_strChipID.Substring(2, 2).PadLeft(3, '0'), "S" + iStation.ToString().PadLeft(2, '0'), strStation, strTime);

            //strXLSName = string.Format("SU-{0}-{1}-T-{2}-{3}.xls", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID, strStation, strTime);

            //strXLSName = Directory.GetCurrentDirectory() + @"\Data\" + strXLSName;
            string strXLSNamePath = sDirectory + strXLSName;
            if (File.Exists(strXLSNamePath))
            {
                DialogResult dia = MessageBox.Show("Test Result Data 文件已经存在，是否进行覆盖保存?", "保存测试结果", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dia == DialogResult.No)
                    return false;
                else
                    File.Delete(strXLSNamePath);
            }
            string fileDemo = @"Data\ReportTmpl.xls";
           
            string sPath = Directory.GetCurrentDirectory() + "\\" + fileDemo;
            if (!File.Exists(sPath))
            {
                MessageBox.Show("Unable to find ReportTmpl.xls!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(sPath);
            excelWorkBook.SaveAs(strXLSNamePath);

            Excel.Sheets excelSheets = excelWorkBook.Worksheets;
            string currentSheet = "Datasheet";
            Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelSheets.get_Item(currentSheet);
            Excel.Range excelCell = (Excel.Range)excelWorksheet.UsedRange;
            try
            {
                string strValue = deviceInfo.m_EditSerialNumber + "-" + deviceInfo.m_strChipID;
                excelCell[1, 3].value = strValue;

                strValue = string.Format("SU-{0}-{1}-{2}-{3}-T-{4}-{5}.dat", deviceInfo.m_EditSerialNumber, deviceInfo.m_strChipID.Substring(0, 2).PadLeft(3, '0'), deviceInfo.m_strChipID.Substring(2, 2).PadLeft(3, '0'), "S" + iStation.ToString().PadLeft(2, '0'), strStation, strTime);
                excelCell[5 - 3, 3].value = strValue;

                strValue = deviceInfo.m_strMaskName;
                excelCell[6 - 3, 3].value = strValue;

                strValue = deviceInfo.m_strInput;
                excelCell[7 - 3, 3].value = strValue;

                strValue = "6"; //deviceInfo.dwCenter;
                excelCell[8 - 3, 3].value = strValue;

                strValue = deviceInfo.m_strOutput;
                excelCell[9 - 3, 3].value = strValue;

                strValue = deviceInfo.m_EditOperator + "/" + deviceInfo.m_dwWSIndex;
                excelCell[10 - 3, 3].value = strValue;

                strValue = deviceInfo.m_strComment;
                excelCell[12 - 3, 3].value = strValue;

                strValue = deviceInfo.m_strTemperature;
                excelCell[4 - 3, 8].value = strValue;
                strValue = testCommon.Sys_loss.ToString("####.00");
                excelCell[9 - 3, 8] = strValue;

                excelCell[5 - 3, 8].value = testCommon.Predicted_temp.ToString("####.00");
              //   strValue = string.Format("{0}-{1}", Math.Round(specCommon.lstSpecPerClass Predicted_temp_min, 2).ToString("####.00"), Math.Round(specCommon.Predicted_temp_max, 2).ToString("####.00"));

                excelCell[6 - 3, 8].value = strValue;

                excelCell[7 - 3, 8].value = "0/0";
                excelCell[8 - 3, 8].value = testCommon.Uniformity;



                strValue = testTime.ToString("yyyy/MM/dd HH:mm");
                excelCell[10 - 3, 8].value = strValue;

                double dblSpecFreqMin = 500.0, dblSpecFreqMax = -500.0;
                double dblFreqMin = 500.0, dblFreqMax = -500.0;
                double dblDiffFreqMin = 500.0, dblDiffFreqMax = -500.0;
                double dblSpecWLMin = 2000.0, dblSpecWLMax = -2000.0;
                double dblWLMin = 2000.0, dblWLMax = -2000.0;
                double dblDiffWLMin = 2000.0, dblDiffWLMax = -2000.0;
                double dblPDWMin = 100.0, dblPDWMax = -100.0;
                double dblILMinMin = 100.0, dblILMinMax = -100.0;
                double dblILMaxMin = 100.0, dblILMaxMax = -100.0;
             
                double dblRippleMin = 100.0, dblRippleMax = -100.0;
              
                double dblPDLMaxMin = 100.0, dblPDLMaxMax = -100.0;
               
                double dblBW1dBMin = 100.0, dblBW1dBMax = -100.0;
                double dblBW3dBMin = 100.0, dblBW3dBMax = -100.0;
              
                double dblBW25dBMin = 100.0, dblBW25dBMax = -100.0;
               
                double dblAXLeftMin = 100.0, dblAXLeftMax = -100.0;
                double dblAXRightMin = 100.0, dblAXRightMax = -100.0;
                double dblNXMin = 100.0, dblNXMax = -100.0;
                double dblTXMin = 100.0, dblTXMax = -100.0;
             
                double dblTAXMin = 100.0, dblTAXMax = -100.0;
                double dblTNXMin = 100.0, dblTNXMax = -100.0;

                string strCellPos = "";
                int iPos = 0;
                for (int dwChannelIndex = dwStartChannel; dwChannelIndex < dwEndChannel; dwChannelIndex++)
                {
                   
                    dblSpecWL = testCondition.iTUWL[dwChannelIndex];

                    iPos = 23 + dwChannelIndex;
                    strCellPos = "A" + (23 + dwChannelIndex).ToString();
                    excelCell[iPos - 3, 1].value = (dwChannelIndex + 1).ToString();

                    strCellPos = "B" + (23 + dwChannelIndex).ToString();
                    excelCell[iPos - 3, 2].value = (dwChannelIndex + 1).ToString();

                    strCellPos = "C" + (23 + dwChannelIndex).ToString();
                    double dblSpecFreq = dblSpecWL / 2.99792457;
                    dblSpecFreq = 1 / dblSpecFreq;
                    dblSpecFreq = dblSpecFreq * 1e5;
                    if (dblSpecFreq < dblSpecFreqMin)
                        dblSpecFreqMin = dblSpecFreq;
                    if (dblSpecFreq >= dblSpecFreqMax)
                        dblSpecFreqMax = dblSpecFreq;
                    strValue = Math.Round(dblSpecFreq, 2).ToString("####.00");
                    excelCell[iPos - 3, 3].value = strValue;

                    strCellPos = "D" + (23 + dwChannelIndex).ToString();
                    double dblWL = pstResultData.m_dblCW[dwChannelIndex];
                    double dblFreq = dblWL / 2.99792457;
                    dblFreq = 1 / dblFreq;
                    dblFreq = dblFreq * 1e5;
                    if (dblFreq < dblFreqMin)
                        dblFreqMin = dblFreq;
                    if (dblFreq >= dblFreqMax)
                        dblFreqMax = dblFreq;
                    strValue = Math.Round(dblFreq, 2).ToString("####.00");
                    excelCell[iPos - 3, 4].value = strValue;

                    strCellPos = "E" + (23 + dwChannelIndex).ToString();
                    if ((dblFreq - dblSpecFreq) < dblDiffFreqMin)
                        dblDiffFreqMin = dblFreq - dblSpecFreq;
                    if ((dblFreq - dblSpecFreq) >= dblDiffFreqMax)
                        dblDiffFreqMax = dblFreq - dblSpecFreq;
                    strValue = Math.Round(dblFreq - dblSpecFreq, 2).ToString("####.00");
                    excelCell[iPos - 3, 5].value = strValue;

                    if(frequencySpec.Offset_max<999)
                    {
                        strCellPos = "E20";
                        
                        strValue = Math.Round(frequencySpec.Offset_max, 2).ToString("####.00");
                        excelCell[20 - 3, 5].value = strValue;

                    }

                    strCellPos = "F" + (23 + dwChannelIndex).ToString();
                    if (dblSpecWL < dblSpecWLMin)
                        dblSpecWLMin = dblSpecWL;
                    if (dblSpecWL >= dblSpecWLMax)
                        dblSpecWLMax = dblSpecWL;
                    excelCell[iPos - 3, 6].value = Math.Round(dblSpecWL, 3).ToString("####.000");

                    strCellPos = "G" + (23 + dwChannelIndex).ToString();
                    dblWL = pstResultData.m_dblCW[dwChannelIndex];
                    if (dblWL < dblWLMin)
                        dblWLMin = dblWL;
                    if (dblWL >= dblWLMax)
                        dblWLMax = dblWL;
                    excelCell[iPos - 3, 7].value = Math.Round(dblWL, 3).ToString("####.000");

                    strCellPos = "H" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblPDW[dwChannelIndex] < dblPDWMin)
                        dblPDWMin = pstResultData.m_dblPDW[dwChannelIndex];
                    if (pstResultData.m_dblPDW[dwChannelIndex] >= dblPDWMax)
                        dblPDWMax = pstResultData.m_dblPDW[dwChannelIndex];
                    excelCell[iPos - 3, 8].value = Math.Round(pstResultData.m_dblPDW[dwChannelIndex], 3).ToString("####.000");

                   
                    if(wavelengthSpec.Pdw_max<999 )
                    {
                        excelCell[20 - 3, 8].value = Math.Round(wavelengthSpec.Pdw_max, 3).ToString("####.000");
                    }
                    strCellPos = "I" + (23 + dwChannelIndex).ToString();
                    if ((dblWL - dblSpecWL) < dblDiffWLMin)
                        dblDiffWLMin = dblWL - dblSpecWL;
                    if ((dblWL - dblSpecWL) >= dblDiffWLMax)
                        dblDiffWLMax = dblWL - dblSpecWL;
                    excelCell[iPos - 3, 9].value = Math.Round(dblWL - dblSpecWL, 3).ToString("####.000");

                    if (wavelengthSpec.Offset_max<999)
                    {
                        strCellPos = "I20";
                        excelCell[20 - 3, 9].value = Math.Round(wavelengthSpec.Offset_max, 3).ToString("####.000");

                    }

                    strCellPos = "J" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblILMin[dwChannelIndex] < dblILMinMin)
                        dblILMinMin = pstResultData.m_dblILMin[dwChannelIndex];

                    if (pstResultData.m_dblILMin[dwChannelIndex] > dblILMinMax)
                        dblILMinMax = pstResultData.m_dblILMin[dwChannelIndex];
                    excelCell[iPos - 3, 10].value = Math.Round(pstResultData.m_dblILMin[dwChannelIndex], 2).ToString("####.00");

                    if (lossSpec.Min_at_lw_max<999)
                    {
                        strCellPos = "J20";
                        excelCell[20 - 3, 10].value = Math.Round(lossSpec.Min_at_lw_max, 2).ToString("####.00");                       
                    }

                    strCellPos = "K" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblILMax[dwChannelIndex] < dblILMaxMin)
                        dblILMaxMin = pstResultData.m_dblILMax[dwChannelIndex];
                    if (pstResultData.m_dblILMax[dwChannelIndex] >= dblILMaxMax)
                        dblILMaxMax = pstResultData.m_dblILMax[dwChannelIndex];
                    excelCell[iPos - 3, 11].value = Math.Round(pstResultData.m_dblILMax[dwChannelIndex], 2).ToString("####.00");

                    if (lossSpec.Max_at_lw_max<999)
                    {
                        strCellPos = "K20";
                        excelCell[20 - 3, 11].value = Math.Round(lossSpec.Max_at_lw_max, 2).ToString("####.00");

                    }

                    strCellPos = "L" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblRipple[dwChannelIndex] < dblRippleMin)
                        dblRippleMin = pstResultData.m_dblRipple[dwChannelIndex];
                    if (pstResultData.m_dblRipple[dwChannelIndex] >= dblRippleMax)
                        dblRippleMax = pstResultData.m_dblRipple[dwChannelIndex];
                    excelCell[iPos - 3, 12].value = Math.Round(pstResultData.m_dblRipple[dwChannelIndex], 2).ToString("####.00");
                    if (lossSpec.Ripple_max<999)
                    {
                        strCellPos = "L20";
                        excelCell[20 - 3, 12].value = Math.Round(lossSpec.Ripple_max, 2).ToString("####.00");
                        
                    }

                    strCellPos = "M" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblPDLMax[dwChannelIndex] < dblPDLMaxMin)
                        dblPDLMaxMin = pstResultData.m_dblPDLMax[dwChannelIndex];
                    if (pstResultData.m_dblPDLMax[dwChannelIndex] >= dblPDLMaxMax)
                        dblPDLMaxMax = pstResultData.m_dblPDLMax[dwChannelIndex];
                    excelCell[iPos - 3, 13].value = Math.Round(pstResultData.m_dblPDLMax[dwChannelIndex], 2).ToString("####.00");
                    if (pdlSpec.Pdl_at_itu_max<999)
                    {
                        strCellPos = "M20";
                        excelCell[20 - 3, 13].value = Math.Round(pdlSpec.Pdl_at_itu_max, 2).ToString("####.00");
                        
                    }

                    strCellPos = "N" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblBW1dB[dwChannelIndex] < dblBW1dBMin)
                        dblBW1dBMin = pstResultData.m_dblBW1dB[dwChannelIndex];
                    if (pstResultData.m_dblBW1dB[dwChannelIndex] >= dblBW1dBMax)
                        dblBW1dBMax = pstResultData.m_dblBW1dB[dwChannelIndex];
                    excelCell[iPos - 3, 14].value = Math.Round(pstResultData.m_dblBW1dB[dwChannelIndex], 3).ToString("####.00");
                    if (passbandSpec.At_1db_min>-999)
                    {
                        strCellPos = "N19";
                        excelCell[19 - 3, 14].value = Math.Round(passbandSpec.At_1db_min, 3).ToString("####.00");
               
                    }

                    strCellPos = "O" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblBW3dB[dwChannelIndex] < dblBW3dBMin)
                        dblBW3dBMin = pstResultData.m_dblBW3dB[dwChannelIndex];
                    if (pstResultData.m_dblBW3dB[dwChannelIndex] >= dblBW3dBMax)
                        dblBW3dBMax = pstResultData.m_dblBW3dB[dwChannelIndex];
                    excelCell[iPos - 3, 15].value = Math.Round(pstResultData.m_dblBW3dB[dwChannelIndex], 3).ToString("####.00");
                    if (passbandSpec.At_3db_min>-999)
                    {
                        strCellPos = "O19";
                        excelCell[19 - 3, 15].value = Math.Round(passbandSpec.At_3db_min, 3).ToString("####.00");
                  
                    }

                    strCellPos = "P" + (23 + dwChannelIndex).ToString();
                    if (pstResultData.m_dblBW25dB[dwChannelIndex] < dblBW25dBMin)
                        dblBW25dBMin = pstResultData.m_dblBW25dB[dwChannelIndex];
                    if (pstResultData.m_dblBW25dB[dwChannelIndex] >= dblBW25dBMax)
                        dblBW25dBMax = pstResultData.m_dblBW25dB[dwChannelIndex];
                    excelCell[iPos - 3, 16].value = Math.Round(pstResultData.m_dblBW25dB[dwChannelIndex], 3).ToString("####.00");
                    if (passbandSpec.At_25db_min>-999)
                    {
                        strCellPos = "P19";
                        excelCell[19 - 3, 16].value = Math.Round(passbandSpec.At_25db_min, 3).ToString("####.000");
             
                    }

                    strCellPos = "Q" + (23 + dwChannelIndex).ToString();
                    if (dwChannelIndex != 0x00)
                    {
                        if (1.0 * pstResultData.m_dblAXLeft[dwChannelIndex] < dblAXLeftMin)
                            dblAXLeftMin = 1.0 * pstResultData.m_dblAXLeft[dwChannelIndex];

                        if (1.0 * pstResultData.m_dblAXLeft[dwChannelIndex] >= dblAXLeftMax)
                            dblAXLeftMax = 1.0 * pstResultData.m_dblAXLeft[dwChannelIndex];
                    }
                    if (dwChannelIndex == 0x00)
                        strValue = "";
                    else
                        strValue = Math.Round(1.0 * pstResultData.m_dblAXLeft[dwChannelIndex], 2).ToString("####.00");
                    excelCell[iPos - 3, 17].value = strValue;
                    if (crosstalkSpec.Ax_n_min>-999)
                    {
                        strCellPos = "Q19";
                        excelCell[19 - 3, 17].value = Math.Round(1.0 * crosstalkSpec.Ax_n_min, 2).ToString("####.00");
                   
                    }

                    strCellPos = "R" + (23 + dwChannelIndex).ToString();
                    if (dwChannelIndex != pstResultData.m_dwChannelCount - 1)
                    {
                        if (1.0 * pstResultData.m_dblAXRight[dwChannelIndex] < dblAXRightMin)
                            dblAXRightMin = 1.0 * pstResultData.m_dblAXRight[dwChannelIndex];

                        if (1.0 * pstResultData.m_dblAXRight[dwChannelIndex] >= dblAXRightMax)
                            dblAXRightMax = 1.0 * pstResultData.m_dblAXRight[dwChannelIndex];
                    }
                    if (dwChannelIndex == pstResultData.m_dwChannelCount - 1)
                        strValue = "";
                    else
                        strValue = Math.Round(1.0 * pstResultData.m_dblAXRight[dwChannelIndex], 2).ToString("####.00");
                    excelCell[iPos - 3, 18].value = strValue;
                    if (crosstalkSpec.Ax_p_min>-999)
                    {
                        strCellPos = "R19";
                        excelCell[19 - 3, 18].value = Math.Round(1.0 *crosstalkSpec.Ax_p_min, 2).ToString("####.00");

                    }

                    strCellPos = "S" + (23 + dwChannelIndex).ToString();
                    if (1.0 * pstResultData.m_dblTAX[dwChannelIndex] < dblTAXMin)
                        dblTAXMin = 1.0 * pstResultData.m_dblTAX[dwChannelIndex];
                    if (1.0 * pstResultData.m_dblTAX[dwChannelIndex] >= dblTAXMax)
                        dblTAXMax = 1.0 * pstResultData.m_dblTAX[dwChannelIndex];
                    excelCell[iPos - 3, 19].value = Math.Round(1.0 * pstResultData.m_dblTAX[dwChannelIndex], 2).ToString("####.00");

                    strCellPos = "T" + (23 + dwChannelIndex).ToString();
                    if (1.0 * pstResultData.m_dblNX[dwChannelIndex] < dblNXMin)
                        dblNXMin = 1.0 * pstResultData.m_dblNX[dwChannelIndex];
                    if (1.0 * pstResultData.m_dblNX[dwChannelIndex] >= dblNXMax)
                        dblNXMax = 1.0 * pstResultData.m_dblNX[dwChannelIndex];
                    excelCell[iPos - 3, 20].value = Math.Round(1.0 * pstResultData.m_dblNX[dwChannelIndex], 2).ToString("####.00");
                    if (crosstalkSpec.Nx_min>-999)
                    {
                        strCellPos = "T19";
                        excelCell[19 - 3, 20].value = Math.Round(1.0 * crosstalkSpec.Nx_min, 2).ToString("####.00");

                        if ((-1.0 * pstResultData.m_dblNX[dwChannelIndex]) > (-1.0 * m_stCriteria.m_dblNXCriterion))
                            m_stCriteria.m_dwResult += 1;
                    }

                    strCellPos = "U" + (23 + dwChannelIndex).ToString();
                    if (1.0 * pstResultData.m_dblTNX[dwChannelIndex] < dblTNXMin)
                        dblTNXMin = 1.0 * pstResultData.m_dblTNX[dwChannelIndex];
                    if (1.0 * pstResultData.m_dblTNX[dwChannelIndex] >= dblTNXMax)
                        dblTNXMax = 1.0 * pstResultData.m_dblTNX[dwChannelIndex];
                    excelCell[iPos - 3, 21].value = Math.Round(1.0 * pstResultData.m_dblTNX[dwChannelIndex], 2).ToString("####.00");

                    strCellPos = "V" + (23 + dwChannelIndex).ToString();
                    if (1.0 * pstResultData.m_dblTX[dwChannelIndex] < dblTXMin)
                        dblTXMin = 1.0 * pstResultData.m_dblTX[dwChannelIndex];
                    if (1.0 * pstResultData.m_dblTX[dwChannelIndex] >= dblTXMax)
                        dblTXMax = 1.0 * pstResultData.m_dblTX[dwChannelIndex];
                    excelCell[iPos - 3, 22].value = Math.Round(1.0 * pstResultData.m_dblTX[dwChannelIndex], 2).ToString("####.00");
                    if (crosstalkSpec.Tx_min>-999)
                    {
                        strCellPos = "V19";
                        excelCell[19 - 3, 22].value = Math.Round(1.0 * crosstalkSpec.Tx_min, 2).ToString("####.00");

                    }
                }
                double dblTempOffset = m_stCriteria.m_dblTempOffset;
                double dblMinTemp = m_stCriteria.m_dblLowPreTemperature;
                double dblMaxTemp = m_stCriteria.m_dblHighPreTemperature;

                strCellPos = "K6";
              
                string strCellCaption = "";
               
                for (int i = 0; i < specCommon.lstCondSpec.Count; i++)
                {
                    strCellCaption = testCommon.lstFinalPf[i].Pf ? "Pass" : "Fail";
                    excelCell[6 - 3, 11].value = strCellCaption;
                    strResult = strCellCaption;
                }
                excelCell[6 - 1, 11].value = m_strTmplModel;
                //strResult = m_strTmplModel; 


                excelCell[19 - 3, 3].value = dblSpecFreqMin.ToString("####.0");
                excelCell[20 - 3, 3].value = dblSpecFreqMax.ToString("####.0");

                excelCell[21 - 3, 4].value = dblFreqMin.ToString("####.00"); ;
                excelCell[22 - 3, 4].value = dblFreqMax.ToString("####.00"); ;

                excelCell[21 - 3, 5].value = dblDiffFreqMin.ToString("####.00");
                excelCell[22 - 3, 5].value = dblDiffFreqMax.ToString("####.00");

                excelCell[19 - 3, 6].value = dblSpecWLMin.ToString("####.000");
                excelCell[20 - 3, 6].value = dblSpecWLMax.ToString("####.000");

                excelCell[21 - 3, 7].value = dblWLMin.ToString("####.000"); ;
                excelCell[22 - 3, 7].value = dblWLMax.ToString("####.000"); ;

                excelCell[21 - 3, 8].value = dblPDWMin.ToString("####.000"); ;
                excelCell[22 - 3, 8].value = dblPDWMax.ToString("####.000"); ;

                excelCell[21 - 3, 9].value = dblDiffWLMin.ToString("####.000");
                excelCell[22 - 3, 9].value = dblDiffWLMax.ToString("####.000");

                excelCell[21 - 3, 10].value = dblILMinMin.ToString("####.00");
                excelCell[22 - 3, 10].value = dblILMinMax.ToString("####.00");

                excelCell[21 - 3, 11].value = dblILMaxMin.ToString("####.00");
                excelCell[22 - 3, 11].value = dblILMaxMax.ToString("####.00");

                excelCell[21 - 3, 12].value = dblRippleMin;
                excelCell[22 - 3, 12].value = dblRippleMax;

                excelCell[21 - 3, 13].value = dblPDLMaxMin;
                excelCell[22 - 3, 13].value = dblPDLMaxMax;

                excelCell[21 - 3, 14].value = dblBW1dBMin.ToString("####.00"); ;
                excelCell[22 - 3, 14].value = dblBW1dBMax.ToString("####.00");;

                excelCell[21 - 3, 15].value = dblBW3dBMin.ToString("####.00"); ;
                excelCell[22 - 3, 15].value = dblBW3dBMax.ToString("####.00");;

                excelCell[21 - 3, 16].value = dblBW25dBMin.ToString("####.00"); ;
                excelCell[22 - 3, 16].value = dblBW25dBMax.ToString("####.00");;

                excelCell[21 - 3, 17].value = dblAXLeftMin;
                excelCell[22 - 3, 17].value = dblAXLeftMax;

                excelCell[21 - 3, 18].value = dblAXRightMin;
                excelCell[22 - 3, 18].value = dblAXRightMax;

                excelCell[21 - 3, 19].value = dblTAXMin;
                excelCell[22 - 3, 19].value = dblTAXMax;

                excelCell[21 - 3, 20].value = dblNXMin;
                excelCell[22 - 3, 20].value = dblNXMax;

                excelCell[21 - 3, 21].value = dblTNXMin;
                excelCell[22 - 3, 21].value = dblTNXMax;

                excelCell[21 - 3, 22].value = dblTXMin;
                excelCell[22 - 3, 22].value = dblTXMax;
                excelWorkBook.Save();
                excelWorkBook.Close();
                excelApp.Quit();
                return true;
            }
            catch (Exception ex)
            {
                excelApp.Quit();
                MessageBox.Show("Save Test Result Fail !!!" + ex.ToString());
                return false;
            }
            finally
            {
                IntPtr t = new IntPtr(excelApp.Hwnd);
                int kill = 0;
                GetWindowThreadProcessId(t, out kill);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(kill);
                p.Kill();
            }

        }

    }
}
