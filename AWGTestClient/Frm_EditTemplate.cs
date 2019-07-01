using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TestSystem.TestLibrary.Utilities;

namespace AWGTestClient
{
    public partial class Frm_EditTemplate : Form
    {
        private static Frm_EditTemplate pEditTemplateForm = null;


        public Frm_EditTemplate()
        {
            InitializeComponent();
            pEditTemplateForm = this;

        }


        public static void ShowEditTemplateFrm()
        {
            // Init static form object
            if (pEditTemplateForm == null)
            {
                // Create new form
                new Frm_EditTemplate();

                // Show the form
                pEditTemplateForm.Show();
            }
            else
                pEditTemplateForm.Show();

            // Set window focus and topmost attributes
            pEditTemplateForm.Focus();
            pEditTemplateForm.TopMost = true;
        }
        public static void CloseEditTemplateFrm()
        {
            // Init static form object
            if (pEditTemplateForm != null)
            {
                pEditTemplateForm.Close();
                pEditTemplateForm = null;
            }
        }

        private void Frm_EditTemplate_Load(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory() + @"\Auto_Template";
            String[] files = Directory.GetFiles(path, "*.csv", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string file = Path.GetFileName(files[i]);
                file = file.Substring(0, file.Length - 4);
                comboBoxTemplete.Items.Add(file);
            }

            comboBoxCLBandOutPort.Items.Add("High Power");
            comboBoxCLBandOutPort.SelectedIndex = 0;

            comboBoxCLBandStep.Items.Add("0.15pm");
            comboBoxCLBandStep.Items.Add("0.3pm");
            comboBoxCLBandStep.Items.Add("0.6pm");
            comboBoxCLBandStep.Items.Add("1.5pm");
            comboBoxCLBandStep.Items.Add("3pm");
            comboBoxCLBandStep.Items.Add("6pm");
            comboBoxCLBandStep.Items.Add("12pm");
            comboBoxCLBandStep.Items.Add("15pm");
            comboBoxCLBandStep.Items.Add("24pm");
            comboBoxCLBandStep.Items.Add("30pm");
            comboBoxCLBandStep.Items.Add("45pm");
            comboBoxCLBandStep.Items.Add("48pm");
            comboBoxCLBandStep.Items.Add("60pm");
            comboBoxCLBandStep.SelectedIndex = 1;

            comboBoxSpan.Items.Add("50G");
            comboBoxSpan.Items.Add("100G");
            comboBoxSpan.Items.Add("200G");
            comboBoxSpan.Items.Add("O Band");
            comboBoxSpan.SelectedIndex = 1;
        }

        private void checkBoxCW_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCW.Enabled = (bool)checkBoxCW.Checked ? true :false;
        }

        private void checkBoxPDW_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPDW.Enabled = (bool)checkBoxPDW.Checked ? true : false;
        }

        private void checkBoxShift_CheckedChanged(object sender, EventArgs e)
        {
            textBoxShift.Enabled = (bool)checkBoxShift.Checked ? true : false;
        }

        private void checkBoxILMin_CheckedChanged(object sender, EventArgs e)
        {
            textBoxILMin.Enabled = (bool)checkBoxILMin.Checked ? true : false;
        }

        private void checkBoxILMax_CheckedChanged(object sender, EventArgs e)
        {
            textBoxILMax.Enabled = (bool)checkBoxILMax.Checked ? true : false;
        }

        private void checkBoxRipple_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRipple.Enabled = (bool)checkBoxRipple.Checked ? true : false;
        }

        private void checkBoxPDLItu_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPDLITU.Enabled = (bool)checkBoxPDLItu.Checked ? true : false;
        }

        private void checkBoxPDLCRT_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPDLCRT.Enabled = (bool)checkBoxPDLCRT.Checked ? true : false;
        }

        private void checkBoxPDWMax_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPDWMax.Enabled = (bool)checkBoxPDWMax.Checked ? true : false;
        }

        private void checkBoxBW05_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBW05.Enabled = (bool)checkBoxBW05.Checked ? true : false;
        }

        private void checkBoxBW1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBW1.Enabled = (bool)checkBoxBW1.Checked ? true : false;
        }

        private void checkBoxBW3_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBW3.Enabled = (bool)checkBoxBW3.Checked ? true : false;
        }

        private void checkBoxBW20_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBW20.Enabled = (bool)checkBoxBW20.Checked ? true : false;
        }

        private void checkBoxBW25_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBW25.Enabled = (bool)checkBoxBW25.Checked ? true : false;
        }

        private void checkBoxBW30_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBW30.Enabled = (bool)checkBoxBW30.Checked ? true : false;
        }

        private void checkBox05Left_CheckedChanged(object sender, EventArgs e)
        {
            textBox05Left.Enabled = (bool)checkBox05Left.Checked ? true : false;
        }

        private void checkBox05Right_CheckedChanged(object sender, EventArgs e)
        {
            textBox05Right.Enabled = (bool)checkBox05Right.Checked ? true : false;
        }

        private void checkBox1Left_CheckedChanged(object sender, EventArgs e)
        {
            textBox1Left.Enabled = (bool)checkBox1Left.Checked ? true : false;
        }

        private void checkBox1Right_CheckedChanged(object sender, EventArgs e)
        {
            textBox1Right.Enabled = (bool)checkBox1Right.Checked ? true : false;
        }

        private void checkBox3Left_CheckedChanged(object sender, EventArgs e)
        {
            textBox3Left.Enabled = (bool)checkBox3Left.Checked ? true : false;
        }

        private void checkBox3Right_CheckedChanged(object sender, EventArgs e)
        {
            textBox3Right.Enabled = (bool)checkBox3Right.Checked ? true : false;
        }

        private void checkBoxAXLeft_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAXLeft.Enabled = (bool)checkBoxAXLeft.Checked ? true : false;
        }

        private void checkBoxAXRight_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAXRight.Enabled = (bool)checkBoxAXRight.Checked ? true : false;
        }

        private void checkBoxNX_CheckedChanged(object sender, EventArgs e)
        {
            textBoxNX.Enabled = (bool)checkBoxNX.Checked ? true : false;
        }

        private void checkBoxTX_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTX.Enabled = (bool)checkBoxTX.Checked ? true : false;
        }

        private void checkBoxTXAX_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTXAX.Enabled = (bool)checkBoxTXAX.Checked ? true : false;
        }

        private void checkBoxILITU_CheckedChanged(object sender, EventArgs e)
        {
            textBoxILITU.Enabled = (bool)checkBoxILITU.Checked ? true : false;
        }

        private void checkBoxBWCust1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBWCust1nm.Enabled = (bool)checkBoxBWCust1.Checked ? true : false;
            textBoxBWCust1dB.Enabled = (bool)checkBoxBWCust1.Checked ? true : false;
        }

        private void checkBoxBWCust2_CheckedChanged(object sender, EventArgs e)
        {
            textBoxBWCust2nm.Enabled = (bool)checkBoxBWCust2.Checked ? true : false;
            textBoxBWCust2dB.Enabled = (bool)checkBoxBWCust2.Checked ? true : false;
        }

        private void checkBoxCoupleLoss_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAlignIL.Enabled = (bool)checkBoxAlignIL.Checked ? true : false;
        }

        private void checkBoxILUni_CheckedChanged(object sender, EventArgs e)
        {
            textBoxILUni.Enabled = (bool)checkBoxILUni.Checked ? true : false;
        }

        private void checkBoxILBWL_CheckedChanged(object sender, EventArgs e)
        {
            textBoxILBWL.Enabled = (bool)checkBoxILBWL.Checked ? true : false;
        }

        private void checkBoxTemperature_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTemperature1.Enabled = (bool)checkBoxTemperature.Checked ? true : false;
            textBoxTemperature2.Enabled = (bool)checkBoxTemperature.Checked ? true : false;
        }

        private void checkBoxTOffset_CheckedChanged(object sender, EventArgs e)
        {
            textBoxTOffset.Enabled = (bool)checkBoxTOffset.Checked ? true : false;
        }

        private void checkBoxPowerOffset_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPowerOffset.Enabled = (bool)checkBoxPowerOffset.Checked ? true : false;

        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            comboBoxCLBandOutPort.SelectedIndex = 1;
            comboBoxCLBandStep.SelectedIndex = 1;
            comboBoxSpan.SelectedIndex = 1;
            textBoxStartWavelength.Text = "1490";
            textBoxStopWavelength.Text = "1640";
            listView1.Items.Clear();
            textBoxITU.Text = "";
            textBoxPower.Text = "5";

            checkBoxCW.Checked = false;
            checkBoxPDW.Checked = false;
            checkBox05Left.Checked = false;
            checkBox05Right.Checked = false;
            checkBox1Left.Checked = false;
            checkBox1Right.Checked = false;
            checkBox3Left.Checked = false;
            checkBox3Right.Checked = false;
            checkBoxAlignIL.Checked = false;
            checkBoxAXLeft.Checked = false;
            checkBoxAXRight.Checked = false;
            checkBoxBW05.Checked = false;
            checkBoxBW1.Checked = false;
            checkBoxBW20.Checked = false;
            checkBoxBW25.Checked = false;
            checkBoxBW3.Checked = false;
            checkBoxBW30.Checked = false;
            checkBoxBWCust1.Checked = false;
            checkBoxBWCust2.Checked = false;
            checkBoxILBWL.Checked = false;
            checkBoxILITU.Checked = false;
            checkBoxILMax.Checked = false;
            checkBoxILMin.Checked = false;
            checkBoxILUni.Checked = false;
            checkBoxNX.Checked = false;
            checkBoxPDLCRT.Checked = false;
            checkBoxPDLItu.Checked = false;
            checkBoxPDWMax.Checked = false;
            checkBoxPowerOffset.Checked = false;
            checkBoxRipple.Checked = false;
            checkBoxShift.Checked = false;
            checkBoxTemperature.Checked = false;
            checkBoxTOffset.Checked = false;
            checkBoxTX.Checked = false;
            checkBoxTXAX.Checked = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxTemplete.Text == "")
            {
                MessageBox.Show("请输入文件名！！！");
                return;
            }
            string sFileName = Directory.GetCurrentDirectory() + @"\Auto_Template\" + comboBoxTemplete.Text + ".csv";
            if (File.Exists(sFileName))
            {
                DialogResult dig= MessageBox.Show("文件已经存在，是否进行覆盖保存?", "保存执行标准模板", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dig == DialogResult.No)
                    return;
                else
                    File.Delete(sFileName);
            }
            int m_dwChannelCounts =int.Parse ( textBoxCounts.Text);
            using (StreamWriter writer = new StreamWriter(sFileName, true))
            {
                string temp = "";
                temp = "Start WL:, " + textBoxStartWavelength.Text.Trim();
                writer.WriteLine(temp);

                temp = "Stop WL:, " + textBoxStopWavelength.Text.Trim();
                writer.WriteLine(temp);

                int nListIndex = comboBoxCLBandStep.SelectedIndex;
                temp ="Step:, "+nListIndex;
                writer.WriteLine(temp);

                temp = "Power:, " + textBoxPower.Text.Trim();
                writer.WriteLine(temp);

                nListIndex = comboBoxCLBandOutPort.SelectedIndex ;
                temp ="Output:, "+nListIndex;
                writer.WriteLine(temp);

                string mm = checkBoxOperator.Checked ? "1" : "0";
                temp = "LLog:, " + mm;
                writer.WriteLine(temp);

                temp ="Channel Count:, "+ m_dwChannelCounts;
                writer.WriteLine(temp);

                temp = "Channel Name:,";
                for (nListIndex = 0x00; nListIndex < m_dwChannelCounts; nListIndex++)
                {
                    string str1 = listView1.Items[nListIndex].Text+",";
                    temp += str1;
                }
                writer.WriteLine(temp);

                if (checkBoxCW .Checked)
                    temp ="CW:, "+ textBoxCW .Text.Trim();
                else
                    temp = "CW:, X";
                writer.WriteLine(temp);

                if (checkBoxPDW.Checked)
                    temp = "PDW:, " + textBoxPDW.Text.Trim();
                else
                    temp = "PDW:, X";
                writer.WriteLine(temp);

                if (checkBoxShift.Checked)
                    temp ="Shift:, "+ textBoxShift .Text.Trim();
                else
                    temp ="Shift:, X";
                writer.WriteLine(temp);

                if (checkBoxILMin .Checked)
                    temp ="ILMin:, "+ textBoxILMin.Text.Trim();
                else
                    temp ="ILMin:, X";
                writer.WriteLine(temp);

                if (checkBoxILMax.Checked)
                    temp ="ILMax:, "+ textBoxILMax .Text.Trim();
                else
                    temp ="ILMax:, X";
                writer.WriteLine(temp);

                if (checkBoxRipple .Checked)
                    temp ="Ripple:, "+textBoxRipple .Text.Trim();
                else
                    temp ="Ripple:, X";
                writer.WriteLine(temp);

                if (checkBoxPDLItu.Checked)
                    temp ="PDLITU:, "+ textBoxPDLITU .Text.Trim();
                else
                    temp ="PDLITU:, X";
                writer.WriteLine(temp);

                if (checkBoxPDLCRT .Checked)
                    temp ="PDLCRT:, "+ textBoxPDLCRT .Text.Trim();
                else
                    temp ="PDLCRT:, X";
                writer.WriteLine(temp);

                if (checkBoxPDWMax.Checked)
                    temp ="PDLMax:, " + textBoxPDWMax .Text.Trim();
                else
                    temp ="PDLMax:, X";
                writer.WriteLine(temp);

                if (checkBoxBW05 .Checked)
                    temp ="BW0.5dB:, " + textBoxBW05 .Text.Trim();
                else
                    temp ="BW0.5dB:, X";
                writer.WriteLine(temp);

                if (checkBoxBW1 .Checked)
                    temp ="BW1dB:, " + textBoxBW1 .Text.Trim();
                else
                    temp ="BW1dB:, X";
                writer.WriteLine(temp);

                if (checkBoxBW3.Checked)
                    temp ="BW3dB:, " + textBoxBW3 .Text.Trim();
                else
                    temp ="BW3dB:, X";
                writer.WriteLine(temp);

                if (checkBoxBW20 .Checked)
                    temp ="BW20dB:, " + textBoxBW20 .Text.Trim();
                else
                    temp ="BW20dB:, X";
                writer.WriteLine(temp);

                if (checkBoxBW25.Checked)
                    temp ="BW25dB:, " + textBoxBW25 .Text.Trim();
                else
                    temp ="BW25dB:, X";
                writer.WriteLine(temp);

                if (checkBoxBW30.Checked)
                    temp ="BW30dB:, " + textBoxBW30 .Text.Trim();
                else
                    temp ="BW30dB:, X";
                writer.WriteLine(temp);

                if (checkBox05Left .Checked)
                    temp ="0.5dBCC-:, " + textBox05Left .Text.Trim();
                else
                    temp ="0.5dBCC+:, X";
                writer.WriteLine(temp);

                if (checkBox05Right .Checked)
                    temp ="0.5dBCC+:, "+ textBox05Right .Text.Trim();
                else
                    temp ="0.5dBCC+:, X";
                writer.WriteLine(temp);

                if (checkBox1Left.Checked)
                    temp ="1dBCC-:, "+ textBox1Left .Text.Trim();
                else
                    temp ="1dBCC+:, X";
                writer.WriteLine(temp);

                if (checkBox1Right.Checked)
                    temp ="1dBCC+:, "+ textBox1Right .Text.Trim();
                else
                    temp ="1dBCC+:, X";
                writer.WriteLine(temp);

                if (checkBox3Left.Checked)
                    temp ="3dBCC-:, "+ textBox3Left .Text.Trim();
                else
                    temp ="3dBCC-:, X";
                writer.WriteLine(temp);

                if (checkBox3Right .Checked)
                    temp ="3dBCC+:, "+ textBox3Right .Text.Trim();
                else
                    temp ="3dBCC+:, X";
                writer.WriteLine(temp);

                if (checkBoxAXLeft.Checked)
                    temp ="AX-:, "+ textBoxAXLeft .Text.Trim();
                else
                    temp ="AX-:, X";
                writer.WriteLine(temp);

                if (checkBoxAXRight .Checked)
                    temp ="AX+:, " + textBoxAXRight .Text.Trim();
                else
                    temp ="AX+:, X";
                writer.WriteLine(temp);

                if (checkBoxNX .Checked)
                    temp ="NX:, " + textBoxNX .Text.Trim();
                else
                    temp ="NX:, X";
                writer.WriteLine(temp);

                if (checkBoxTX .Checked)
                    temp ="TX:, "+ textBoxTX .Text.Trim();
                else
                    temp ="TX:, X";
                writer.WriteLine(temp);

                if (checkBoxTXAX .Checked)
                    temp ="TX-AX:, "+ textBoxTXAX .Text.Trim();
                else
                    temp ="TX-AX:, X";
                writer.WriteLine(temp);

                if (checkBoxILITU .Checked)
                    temp ="ILITU:, "+ textBoxILITU .Text.Trim();
                else
                    temp ="ILITU:, X";
                writer.WriteLine(temp);

                if (checkBoxBWCust1.Checked)
                    temp = string.Format("BWCust1:, {0}, {1}", textBoxBWCust1dB.Text.Trim(), textBoxBWCust1nm.Text.Trim());
                else
                    temp = "BWCust1:, X";
                writer.WriteLine(temp);

                if (checkBoxBWCust2 .Checked)
                    temp =string .Format ( "BWCust2:, {0}, {1}", textBoxBWCust2dB.Text.Trim(), textBoxBWCust2nm.Text.Trim());
                else
                    temp ="BWCust2:, X";
                writer.WriteLine(temp);

                if (checkBoxAlignIL.Checked)
                    temp = "AlignIL:, " + textBoxAlignIL.Text.Trim();
                else
                    temp ="AlignIL:, X";
                writer.WriteLine(temp);

                if (checkBoxILUni.Checked)
                    temp ="Uniformity:, " + textBoxILUni .Text.Trim();
                else
                    temp ="Uniformity:, X";
                writer.WriteLine(temp);

                if (checkBoxILBWL .Checked)
                    temp ="IL LossWindow:, " + textBoxILBWL .Text.Trim();
                else
                    temp ="IL LossWindow:, X";
                writer.WriteLine(temp);

                if (checkBoxRippleLW.Checked)
                    temp = "Ripple LossWindow:, " + textBoxRippleLW.Text.Trim();
                else
                    temp = "Ripple LossWindow:, X";
                writer.WriteLine(temp);

                if (checkBoxCrossTalkLW.Checked)
                    temp = "CrossTalk LossWindow:, " + textBoxCrossTalkLW.Text.Trim();
                else
                    temp = "CrossTalk LossWindow:, X";
                writer.WriteLine(temp);

                if (checkBoxTemperature .Checked)
                    temp =string .Format ( "Temperature:, {0}, {1}", textBoxTemperature1 .Text.Trim(), textBoxTemperature2 .Text.Trim());
                else
                    temp ="Temperature:. X";
                writer.WriteLine(temp);

                if (checkBoxTOffset.Checked)
                    temp ="Temperature Offset:, " + textBoxTOffset .Text.Trim();
                else
                    temp ="Temperature Offset:, X";
                writer.WriteLine(temp);

                if (checkBoxPowerOffset .Checked)
                    temp ="Power Offset:, " + textBoxPowerOffset .Text.Trim();
                else
                    temp ="Power Offset:, X";
                writer.WriteLine(temp);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            CloseEditTemplateFrm();
        }

        private void comboBoxTemplete_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sFileName = Directory.GetCurrentDirectory() + @"\Auto_Template\" + comboBoxTemplete.Text + ".csv";
            LoadTemplateContent(sFileName);
        }

        private void LoadTemplateContent(string sFileName)
        {
            //DatumList Result = new DatumList();
            int m_dwChannelCounts = 0;
            using (CsvReader reader = new CsvReader())
            {
                reader.OpenFile(sFileName);
                string[] lineElems;
                int lineNbr = 0;

                lineElems = reader.GetLine();
                int lineElemLen = lineElems.Length;
                do
                {
                    if (lineElems.Contains("Start WL:"))
                        textBoxStartWavelength.Text = lineElems[1];

                    if (lineElems.Contains("Stop WL:"))
                        textBoxStopWavelength.Text = lineElems[1];

                    if (lineElems.Contains("Step:"))
                        comboBoxCLBandStep.SelectedIndex =int.Parse ( lineElems[1]);

                    if (lineElems.Contains("Power:"))
                        textBoxPower.Text = lineElems[1];

                    if (lineElems.Contains("Output:"))
                        comboBoxCLBandOutPort.Text = lineElems[1];

                    if (lineElems.Contains("LLog:"))
                    {
                        bool bCh= lineElems[1] == " 1" ? true : false;
                        checkBoxOperator.Checked = bCh;
                        //checkBoxOperator.Checked  = lineElems[1]=="1"?true :false;
                    }
                    if (lineElems.Contains("Channel Count:"))
                    {
                        textBoxCounts.Text = lineElems[1];
                        m_dwChannelCounts = int.Parse(lineElems[1]);
                    }

                    if (lineElems.Contains("Channel Name:"))
                    {
                        listView1.Items.Clear();
                        listView1.BeginUpdate();
                        //ListViewItem lvi = null;
                        //lvi = new ListViewItem(msg);
                        //listView1.Items.Add(lvi);

                        string temp = "";
                        for (int i = 0; i < m_dwChannelCounts; i++)
                        {
                            temp = lineElems[1 + i];
                            listView1.Items.Add(new ListViewItem(temp));
                        }
                        listView1.EndUpdate();
                    }

                    if (lineElems.Contains("CW:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxCW .Checked = false;
                            textBoxCW.Text = "0.0";
                        }
                        else
                        {
                            checkBoxCW.Checked = true;
                            textBoxCW.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("PDW:"))
                    {
                        if (lineElems.Contains("X"))
                        {
                            checkBoxPDW.Checked = false;
                            textBoxPDW.Text = "0.0";
                        }
                        else
                        {
                            checkBoxPDW.Checked = true;
                            textBoxPDW.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("Shift:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxShift.Checked = false;
                            textBoxShift.Text = "0.0";
                        }
                        else
                        {
                            checkBoxShift.Checked = true;
                            textBoxShift.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("ILMin:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxILMin.Checked = false;
                            textBoxILMin.Text = "0.0";
                        }
                        else
                        {
                            checkBoxILMin.Checked = true;
                            textBoxILMin.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("ILMax:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxILMax.Checked = false;
                            textBoxILMax.Text = "0.0";
                        }
                        else
                        {
                            checkBoxILMax.Checked = true;
                            textBoxILMax.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("Ripple:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxRipple.Checked = false;
                            textBoxRipple.Text = "0.0";
                        }
                        else
                        {
                            checkBoxRipple.Checked = true;
                            textBoxRipple.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("PDLITU:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxPDLItu.Checked = false;
                            textBoxPDLITU.Text = "0.0";
                        }
                        else
                        {
                            checkBoxPDLItu.Checked = true;
                            textBoxPDLITU.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("PDLCRT:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxPDLCRT.Checked = false;
                            textBoxPDLCRT.Text = "0.0";
                        }
                        else
                        {
                            checkBoxPDLCRT.Checked = true;
                            textBoxPDLCRT.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("PDLMax:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxPDWMax.Checked = false;
                            textBoxPDWMax.Text = "0.0";
                        }
                        else
                        {
                            checkBoxPDWMax.Checked = true;
                            textBoxPDWMax.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("BW0.5dB:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBW05.Checked = false;
                            textBoxBW05.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBW05.Checked = true;
                            textBoxBW05.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("BW1dB:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBW1.Checked = false;
                            textBoxBW1.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBW1.Checked = true;
                            textBoxBW1.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("BW3dB:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBW3.Checked = false;
                            textBoxBW3.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBW3.Checked = true;
                            textBoxBW3.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("BW20dB:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBW20.Checked = false;
                            textBoxBW20.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBW20.Checked = true;
                            textBoxBW20.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("BW25dB:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBW25.Checked = false;
                            textBoxBW25.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBW25.Checked = true;
                            textBoxBW25.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("BW30dB:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBW30.Checked = false;
                            textBoxBW30.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBW30.Checked = true;
                            textBoxBW30.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("0.5dBCC-:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBox05Left.Checked = false;
                            textBox05Left.Text = "0.0";
                        }
                        else
                        {
                            checkBox05Left.Checked = true;
                            textBox05Left.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("0.5dBCC+:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBox05Right.Checked = false;
                            textBox05Right.Text = "0.0";
                        }
                        else
                        {
                            checkBox05Right.Checked = true;
                            textBox05Right.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("1dBCC-:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBox1Left.Checked = false;
                            textBox1Right.Text = "0.0";
                        }
                        else
                        {
                            checkBox1Left.Checked = true;
                            textBox1Right.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("1dBCC+:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBox1Right.Checked = false;
                            textBox1Right.Text = "0.0";
                        }
                        else
                        {
                            checkBox1Right.Checked = true;
                            textBox1Right.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("3dBCC-:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBox3Left.Checked = false;
                            textBox3Left.Text = "0.0";
                        }
                        else
                        {
                            checkBox3Left.Checked = true;
                            textBox3Left.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("3dBCC+:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBox3Right.Checked = false;
                            textBox3Right.Text = "0.0";
                        }
                        else
                        {
                            checkBox3Right.Checked = true;
                            textBox3Right.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("AX-:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxAXLeft.Checked = false;
                            textBoxAXLeft.Text = "0.0";
                        }
                        else
                        {
                            checkBoxAXLeft.Checked = true;
                            textBoxAXLeft.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("AX+:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxAXRight.Checked = false;
                            textBoxAXRight.Text = "0.0";
                        }
                        else
                        {
                            checkBoxAXRight.Checked = true;
                            textBoxAXRight.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("NX:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxNX.Checked = false;
                            textBoxNX.Text = "0.0";
                        }
                        else
                        {
                            checkBoxNX.Checked = true;
                            textBoxNX.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("TX:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxTX.Checked = false;
                            textBoxTX.Text = "0.0";
                        }
                        else
                        {
                            checkBoxTX.Checked = true;
                            textBoxTX.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("TX-AX:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxTXAX.Checked = false;
                            textBoxTXAX.Text = "0.0";
                        }
                        else
                        {
                            checkBoxTXAX.Checked = true;
                            textBoxTXAX.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("ILITU:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxILITU.Checked = false;
                            textBoxILITU.Text = "0.0";
                        }
                        else
                        {
                            checkBoxILITU.Checked = true;
                            textBoxILITU.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("BWCust1:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBWCust1.Checked = false;
                            textBoxBWCust1dB.Text = "0.0";
                            textBoxBWCust1nm.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBWCust1.Checked = true;
                            textBoxBWCust1dB.Text = lineElems[1];
                            textBoxBWCust1nm.Text = lineElems[2];
                        }
                    }
                    if (lineElems.Contains("BWCust2:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxBWCust2.Checked = false;
                            textBoxBWCust2dB.Text = "0.0";
                            textBoxBWCust2nm.Text = "0.0";
                        }
                        else
                        {
                            checkBoxBWCust2.Checked = true;
                            textBoxBWCust2dB.Text = lineElems[1];
                            textBoxBWCust2nm.Text = lineElems[2];
                        }
                    }

                    if (lineElems.Contains("AlignIL:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxAlignIL.Checked = false;
                            textBoxAlignIL.Text = "0.0";
                        }
                        else
                        {
                            checkBoxAlignIL.Checked = true;
                            textBoxAlignIL.Text = lineElems[1];
                        }
                    }

                    if (lineElems.Contains("Uniformity:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxILUni.Checked = false;
                            textBoxILUni.Text = "0.0";
                        }
                        else
                        {
                            checkBoxILUni.Checked = true;
                            textBoxILUni.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("IL LossWindow:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxILBWL.Checked = false;
                            textBoxILBWL.Text = "0.0";
                        }
                        else
                        {
                            checkBoxILBWL.Checked = true;
                            textBoxILBWL.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("Ripple LossWindow:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxRippleLW.Checked = false;
                            textBoxRippleLW.Text = "0.0";
                        }
                        else
                        {
                            checkBoxRippleLW.Checked = true;
                            textBoxRippleLW.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("CrossTalk LossWindow:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxCrossTalkLW.Checked = false;
                            textBoxCrossTalkLW.Text = "0.0";
                        }
                        else
                        {
                            checkBoxCrossTalkLW.Checked = true;
                            textBoxCrossTalkLW.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("Temperature:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxTemperature.Checked = false;
                            textBoxTemperature1.Text = "0.0";
                            textBoxTemperature2.Text = "0.0";
                        }
                        else
                        {
                            checkBoxTemperature.Checked = true;
                            textBoxTemperature1.Text = lineElems[1];
                            textBoxTemperature2.Text = lineElems[2];
                        }
                    }

                    if (lineElems.Contains("Temperature Offset:"))
                    {
                        if (lineElems.Contains("X"))
                        {
                            checkBoxTOffset.Checked = false;
                            textBoxTOffset.Text = "0.0";
                        }
                        else
                        {
                            checkBoxTOffset.Checked = true;
                            textBoxTOffset.Text = lineElems[1];
                        }
                    }
                    if (lineElems.Contains("Power Offset:"))
                    {
                        if (lineElems.Contains(" X"))
                        {
                            checkBoxPowerOffset.Checked = false;
                            textBoxPowerOffset.Text = "0.0";
                        }
                        else
                        {
                            checkBoxPowerOffset.Checked = true;
                            textBoxPowerOffset.Text = lineElems[1];
                        }
                        lineNbr++;
                    }
                } while ((lineElems = reader.GetLine()) != null);
            }
        }

        private void textBoxITU_TextChanged(object sender, EventArgs e)
        {
            string strITU = textBoxITU.Text;
            if (strITU.Length == 3)
            {
                AWGTestClient awgTest = new AWGTestClient();
                double ITU= awgTest.ConvertITUNameToWL(strITU);
                if (ITU == 0)
                {
                    MessageBox.Show("ITU通道范围：L48 - L00, Q48-Q100, C01-C61, H01-H61, O01-O04!", "ITU通道错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    MessageBox.Show("此ITU通道名无效!", "通道名称错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                    textBoxWL.Text = ITU.ToString();
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            bool bValid = true;
            string strITU = textBoxITU.Text;
            AWGTestClient awgTest = new AWGTestClient();
            int m_dwChannelCounts = int.Parse(textBoxCounts.Text);
            double ITU = 0;
            if (strITU.Length != 3)
            {
                bValid = false;
            }
            else
            {
                ITU = awgTest.ConvertITUNameToWL(strITU);
                if (ITU == 0)
                    bValid = false;
            }
            if (!bValid)
            {
                MessageBox.Show("ITU通道范围：L48 - L00, Q48-Q100, C01-C61, H01-H61, O01-O04!", "ITU通道错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                MessageBox.Show("此ITU通道名无效!", "通道名称错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string strLeft = strITU.Substring(0, 1);
            int dwITU = int.Parse(strITU.Substring(1, 2));
            listView1.Items.Clear();
            listView1.BeginUpdate();
            for (int dwChannelIndex = 0x00; dwChannelIndex < m_dwChannelCounts; dwChannelIndex++)
            {
                if (comboBoxSpan.SelectedIndex == 0x01 && dwChannelIndex != 0x00)
                    dwITU -= 1;
                else if (comboBoxSpan.SelectedIndex == 0x02 && dwChannelIndex != 0x00)
                    dwITU -= 2;
                else if (comboBoxSpan.SelectedIndex == 0x03 && dwChannelIndex != 0x00)
                    dwITU -= 1;
                listView1.Items.Add(new ListViewItem(strLeft + dwITU.ToString ()));
            }
            listView1.EndUpdate();
        }

        private void Frm_EditTemplate_FormClosed(object sender, FormClosedEventArgs e)
        {
            pEditTemplateForm = null;
        }

        private void Frm_EditTemplate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.Cancel &&

    this.DialogResult != DialogResult.OK)

                e.Cancel = true;
        }

        private void checkBoxRippleLW_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRippleLW.Enabled = (bool)checkBoxRippleLW.Checked ? true : false;
        }

        private void checkBoxCrossTalkLW_CheckedChanged(object sender, EventArgs e)
        {
            textBoxCrossTalkLW.Enabled = (bool)checkBoxCrossTalkLW.Checked ? true : false;
        }

    }
}
