using System;
using NationalInstruments.NI4882;

namespace AWGTestServer.Instruments
{
	/// <summary>
	/// GPIB 的摘要说明。
	/// </summary>
	public class GPIB
	{
		private static LangInt li;                          // declare an instance of the .NET language interface functions
		private static GpibConstants c;                     // declare an instance of the .NET language interface constants
		private const int ARRAYSIZE = 1024;					// size of ReadBuffer
		private int Dev;								// a reference to an instrument on the bus
		private static string ValueStr;						// holds the string returned from instrument
		private const int BDINDEX = 0;					    // board Index
		//private const int PRIMARY_ADDR_OF_PPS = 1;          // pimary address of device
		private const int NO_SECONDARY_ADDR = 0;            // secondary address of device
		private static int TIMEOUT;                         // tmeout value
		private const int EOTMODE = 1;                      // enable the END message
		private const int EOSMODE = 0;                      // disable the EOS mode		
		private static string[] ErrorMnemonic = {"EDVR", "ECIC", "ENOL", "EADR", "EARG",          // Error codes
													"ESAC", "EABO", "ENEB", "EDMA", "",
													"EOIP", "ECAP", "EFSO", "", "EBUS",
													"ESTB", "ESRQ", "", "", "", "ETAB"};	
		public GPIB(int ADDR)//构造函数
		{
			//ADDR为GPIB设备地址
			try
			{
				li = new LangInt();          // Contains all GPIB functions
				c = new GpibConstants();     // Contains all GPIB global constants
				TIMEOUT=c.T3s;
				
				Dev=li.ibdev(BDINDEX,ADDR,NO_SECONDARY_ADDR,TIMEOUT,EOTMODE,EOSMODE);//Get the device handle
				if((li.ibsta&c.ERR)!=0)
					throw new System.Exception("Unable to open device!\nibsta:"+li.ibsta+"\nERR:"+c.ERR);

				
				li.ibclr(Dev);
				if((li.ibsta&c.ERR)!=0)
					throw new System.Exception("Unable to clear device!\nibsta:"+li.ibsta+"\nERR:"+c.ERR);
			}
			catch(System.Exception ex)
			{
                throw new Exception(ex.Message);
			}
		}
		//wirte to device
		public void GPIBwr(string Commands)
		{
			try
			{
				int BuffSize;
				BuffSize=Commands.Length;
				li.ibwrt(Dev,Commands,BuffSize);
				if((li.ibsta&c.ERR)!=0)
					throw new System.Exception("Unable to write to device!\nibsta:"+li.ibsta+"\nERR:"+c.ERR);
			}
			catch(System.Exception Ex)
			{
//				MessageBox.Show("Error: " + Ex.Message +  //error Name
//					"ibsta = " + li.ibsta +       //ibsta
//					"iberr = " + li.iberr +       //iberr
//					ErrorMnemonic[li.iberr]);     //error code
				string str = Ex.Message +  //error Name
					"ibsta = " + li.ibsta +       //ibsta
					"iberr = " + li.iberr +       //iberr
					ErrorMnemonic[li.iberr];     //error code
				throw new Exception(str);
			}
		}
		//read data form device
		public string GPIBrd(int buffersize)
		{
			try
			{
				li.ibrd(Dev,out ValueStr,buffersize);
				li.ibclr(Dev);
				return ValueStr;
			}
			catch(System.Exception Ex)
			{
				throw new Exception("Error: " + Ex.Message +  //error Name
					"ibsta = " + li.ibsta +       //ibsta
					"iberr = " + li.iberr +       //iberr
					ErrorMnemonic[li.iberr]);     //error code
			}
		}
	}
}
