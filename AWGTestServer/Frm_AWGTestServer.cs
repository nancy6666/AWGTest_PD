using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgN778xLib;
//using TestSystem.Information;
//using TestSystem.InternalData;
using System.IO;
using System.Threading;
using TestSystem.TestLibrary.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using TestSystem.TestLibrary.INI;
using AWGTestServer.Instruments;
namespace AWGTestServer
{
    public partial class Frm_AWGTestServer : Form
    {
        ConfigurationManagement cfg;
        Brush mybsh = Brushes.Green;
        Inst_Ag_ILPDL_PD inst_Ag_ILPDL_PD;
        Thread threedStartTest;
        Thread threedSocket;
        int iStation = 999;
       
        string sRequestTestPath =Directory .GetCurrentDirectory() + @"\Config\Information\RequestTest.txt";
        string strIP;
        string strPort;
        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();
        Dictionary<int, string> dicStation = new Dictionary<int, string>();
        Queue<string> strAction = new Queue<string>();
        bool bStop = false;
     
        Socket socketServer = null;
      
        bool[] bInitInstrument = new bool [8];
        bool[] bReference = new bool[8];
        delegate void ThreedShowMsgDelegate(string Message, bool bFail);
        delegate void ThreedClearListView();
        public Frm_AWGTestServer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inst_Ag_ILPDL_PD = new Inst_Ag_ILPDL_PD(this);
            cfg = new ConfigurationManagement();
            strIP = cfg.SocketIP;
            strPort = cfg.SocketPort;
            for (int i=1;i<9;i++)
                SetStationIcon(i, false);

            iStation = 0;

            try
            {
                inst_Ag_ILPDL_PD.InitI();
                for (int i = 0; i < 8; i++)
                {
                   string sStationSettingFilePath = Directory.GetCurrentDirectory() + @"\Config\StationSetting\AWGTLSSetting" + (i).ToString() + ".INI";
                    if (File.Exists(sStationSettingFilePath))
                        inst_Ag_ILPDL_PD.GetTLSetting(sStationSettingFilePath, i);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Init Equipment Fail !!!{ex.Message} ");
                ShowMsg($"Init Equipment Fail !!!{ex.Message} ", true);
                KillProcess("AgServerILPDL");
            }

            Listion();
            bStop = false;
            threedStartTest = new Thread(Test);
            threedStartTest.Start();
        }

        private void Test()
        {
            byte[] bufferr;
            bool bUsing = false;
            while (!bStop)
            {
                try
                {
                    while (strAction.Count > 0)
                    {
                        string WaitingItem = "";
                        foreach (var item in strAction)
                        {
                            WaitingItem += item + ";";
                        }
                        foreach (var item in strAction)
                        {
                            string mmm = item;
                            string[] tempm = mmm.Split(' ');
                            int iStationm = int.Parse(tempm[1]);
                            string sActionm = tempm[0];
                            string strNamem = dicStation[iStationm];
                            bufferr = Encoding.UTF8.GetBytes("status;" + WaitingItem);
                            dic[strNamem].Send(bufferr);
                        }
                        while (bUsing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        ClearListView();
                        string mm = strAction.Dequeue();
                        string[] temp = mm.Split(' ');
                        int iStation = int.Parse(temp[1]);
                        SetStationIcon(iStation + 1, true);
                        string sAction = temp[0];
                        string strName = dicStation[iStation];
                        bool bSuccess = false;
                        string server_ret = "999";
                        string sStationSettingFilePath = Directory.GetCurrentDirectory() + @"\Config\StationSetting\AWGTLSSetting" + iStation + ".INI";

                        switch (sAction.ToLower())
                        {
                            case "zero":
                                try
                                {
                                    bUsing = true;
                                    ShowMsg(iStation + " DoReference Start...", false);
                                    bSuccess = inst_Ag_ILPDL_PD.DoReference(sStationSettingFilePath,iStation);
                                    ShowMsg("Calibration Done!", false);
                                }
                                catch (Exception ex)
                                {
                                    bUsing = false;
                                    ShowMsg(ex.ToString(), true);
                                }
                                if (bSuccess)
                                    server_ret = "5";
                                else
                                    server_ret = "6";
                                bufferr = Encoding.UTF8.GetBytes(server_ret);
                                dic[strName].Send(bufferr);
                                bUsing = false;
                                break;
                            case "test":
                                try
                                {
                                    bUsing = true;
                                    ShowMsg(iStation + " test Start...", false);
                                    string ErrorMsg = "";
                                    bSuccess = inst_Ag_ILPDL_PD.DoTest(iStation, ref ErrorMsg);
                                    if (ErrorMsg != "")
                                    {
                                        ShowMsg(ErrorMsg.ToString(), true);
                                    }
                                    else
                                    {
                                        ShowMsg("Test Done!", false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    bUsing = false;
                                    ShowMsg(ex.ToString(), true);
                                }

                                if (bSuccess)
                                    server_ret = "7";
                                else
                                    server_ret = "8";
                                bufferr = Encoding.UTF8.GetBytes(server_ret);
                                dic[strName].Send(bufferr);
                                bUsing = false;

                                break;
                            default:
                                throw new SystemException("Invalid message: " + sAction);
                        }
                        SetStationIcon(iStation + 1, false);
                    }
                    System.Threading.Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    bUsing = false;
                    ShowMsg(ex.ToString(), true);
                }
            }
        }
        private bool Listion()
        {
            bool bListion = false;
            IPAddress ip = IPAddress.Parse(strIP);

            IPEndPoint point = new IPEndPoint(ip, int.Parse(strPort));

            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socketServer.Bind(point);
                socketServer.Listen(4);
                ShowMsg("服务器开始监听",false);
                threedSocket = new Thread(AcceptInfo);
                threedSocket.IsBackground = true;
                threedSocket.Start(socketServer);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message,true);
            }
            return bListion;

        }

        void AcceptInfo(object o)
        {
            Socket socket = o as Socket;
            while (true)
            {
                try
                {
                    Socket tSocket = socket.Accept();
                    string point = tSocket.RemoteEndPoint.ToString();
                    ShowMsg(point + "连接成功！",false );

                    dic.Add(point, tSocket);
                    Thread th = new Thread(ReceiveMsg);
                    th.IsBackground = true;
                    th.Start(tSocket);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message, true);
                }
            }
        }

        //接收消息

        void ReceiveMsg(object o)
        {
            Socket client = o as Socket;
            int iClose = 999;
            bool bClose = false;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    if (client.Available == 0 && bClose)
                    {
                        dic.Remove(client.RemoteEndPoint.ToString());
                        if (dicStation.ContainsValue(client.RemoteEndPoint.ToString()))
                        {
                            foreach (var temp in dicStation)
                            {
                                if (temp.Value == client.RemoteEndPoint.ToString())
                                {
                                    int iStt = temp.Key;
                                    dicStation.Remove(iStt);
                                    ShowMsg(iStt + "号机台已经断开连接！！！", true);
                                }
                            }
                        }
                        break;
                    }
                    int n = client.Receive(buffer);
                    string words = Encoding.UTF8.GetString(buffer, 0, n);
                    int iSt = 999;
                    if (words.ToLower().Contains("disconnect"))
                    {
                        iSt = words.ToLower().IndexOf("station");
                        iSt = int.Parse(words.Substring(iSt + 7, 1));
                        iClose = iSt;
                        if (dicStation.ContainsKey(iSt))
                        {
                            string temp = dicStation[iSt];
                            dic.Remove(temp);
                            dicStation.Remove(iSt);
                        }

                    }
                    else if (words.ToLower().Contains("connect"))
                    {
                        iSt = words.ToLower().IndexOf("station");
                        iSt = int.Parse(words.Substring(iSt + 7, 1));
                        if (!dicStation.ContainsKey(iSt))
                        {
                            dicStation.Add(iSt, client.RemoteEndPoint.ToString());
                        }
                        else
                        {
                            dicStation.Remove(iSt);
                            dicStation.Add(iSt, client.RemoteEndPoint.ToString());
                        }
                    }
                    else if (words.Contains("关闭") || words == "")
                    {
                        bClose = true;
                    }
                    else
                    {
                        if (words != "")
                        {
                            string[] temp = words.Split(';');
                            string[] tempp = temp[0].Split(' ');
                            iSt = int.Parse(tempp[1]);
                            if (temp.Length > 1)
                            {
                                inst_Ag_ILPDL_PD.m_dblAWGStartWavelength[iSt] = double.Parse(temp[1]);
                                inst_Ag_ILPDL_PD.m_dblAWGStopWavelength[iSt] = double.Parse(temp[2]);
                                inst_Ag_ILPDL_PD.m_dblAWGStepWavelength[iSt] =  double.Parse(temp[3]);
                                inst_Ag_ILPDL_PD.m_dblAWGTLSPower[iSt] = double.Parse(temp[4]);
                                inst_Ag_ILPDL_PD.m_bAWGTLSOutoputPort[iSt] = int.Parse(temp[5]);
                                inst_Ag_ILPDL_PD.m_bAWGLambdaLog[iSt] = int.Parse(temp[6]);
                                inst_Ag_ILPDL_PD.m_dblSamplePoint_OneCircle[iSt] = Convert.ToInt32((inst_Ag_ILPDL_PD.m_dblAWGStopWavelength[iSt] - inst_Ag_ILPDL_PD.m_dblAWGStartWavelength[iSt]) / inst_Ag_ILPDL_PD.m_dblAWGStepWavelength[iSt] + 1);
                                //设置TLS参数
                                inst_Ag_ILPDL_PD.SetDevicesParameters(iSt);
                            }
                            strAction.Enqueue(temp[0]);
                        }
                    }

                    ShowMsg(string.Format("Station{0} :", iSt) + words + " -- " + DateTime.Now.ToString(), false);
                }
                catch (Exception ex)
                {
                    //ShowMsg(ex.Message,true);
                    if (ex.ToString().Contains("关闭"))
                        bClose = true;
                } 
            }
        }
        private void SendSocket(int iStat, string Msg)
        {
            try
            {
                ShowMsg(Msg,false );
                byte[] buffer = Encoding.UTF8.GetBytes(Msg);
                dic[dicStation[iStat]].Send(buffer);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message,true);
            }
        }
        void clearListView()
        {
            listView1.Items.Clear();
        }
        void ShowMsgg(string msg, bool bFail)
        {
            listView1.BeginUpdate();
            try
            {
                ListViewItem lvi = null;
                if (msg.Length > 3)
                {
                    lvi = listView1.FindItemWithText(msg.Substring(0, msg.Length - 3));
                    if (lvi != null)
                    {
                        if (lvi.Index == listView1.Items.Count)
                            listView1.Items.Remove(lvi);
                    }
                }
                lvi = new ListViewItem(msg);
                listView1.Items.Add(lvi);
                //lvi.UseItemStyleForSubItems = false;

                if (bFail)
                    lvi.ForeColor = Color.Red;
                else
                    lvi.ForeColor = Color.Green;
            }
            finally
            {
                listView1.EndUpdate();
            }
        }
        public void ShowMsg(string msg, bool bFail)
        {
            this.Invoke(new ThreedShowMsgDelegate(ShowMsgg), new object[] { msg, bFail });
        }

        public void ClearListView()
        {
            this.Invoke(new ThreedClearListView(clearListView));
        }

        private void SetStationIcon(int iStat,bool bCheck)
        {
            switch (iStat)
            {
                case 1:
                    if (bCheck)
                        buttonStation1.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation1.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 2:
                    if (bCheck)
                        buttonStation2.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation2.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 3:
                    if (bCheck)
                        buttonStation3.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation3.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 4:
                    if (bCheck)
                        buttonStation4.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation4.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 5:
                    if (bCheck)
                        buttonStation5.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation5.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 6:
                    if (bCheck)
                        buttonStation6.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation6.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 7:
                    if (bCheck)
                        buttonStation7.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation7.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                case 8:
                    if (bCheck)
                        buttonStation8.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\Check.bmp");
                    else
                        buttonStation8.Image = Image.FromFile(Directory.GetCurrentDirectory() + @"\\Config\\Icon\\WorkStation.bmp");
                    break;
                default:
                    throw new SystemException("Invalid Channel: " + iStation);
            }
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

        private void Frm_AWGTestServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            socketServer.Close();
            threedSocket.Abort();
            KillProcess("AgServerILPDL");
            threedStartTest.Abort();
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {

            DialogResult dialog = MessageBox.Show("是否确认要退出软件？", "信息确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dialog == DialogResult.Yes)
                this.Close();
        }

        private void buttonStation1_Click(object sender, EventArgs e)
        {

        }
    }
}
