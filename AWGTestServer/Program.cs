using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace AWGTestServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //UIThread mainFormThread = new UIThread();
            //mainFormThread.Run();
            //bool bAbort = false;

            //CurrentProgram currentProgram = new CurrentProgram();
            //bAbort = currentProgram.Run();

            //if (bAbort)
            //{
            //    UIThread.uiThread.Abort();
            //    //ShowDeviceFrm();
            //}
           Application.Run(new Frm_AWGTestServer());
        }
    }
}
