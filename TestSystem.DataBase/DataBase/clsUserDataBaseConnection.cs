using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Data.Odbc;

namespace TestSystem.DataBase
{
    public class clsUserDataBaseConnection
    {
          string StrConnection;
          string DatabasePath;

        public clsUserDataBaseConnection()
          {
              OleDbConnection userNameConnection = new OleDbConnection("");

              DatabasePath = @"Config\UserInfo.mdb";
              StrConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + DatabasePath;
              oleDbComCon = new OleDbConnection(StrConnection);
              oleDbComAdp = new OleDbDataAdapter("", oleDbComCon);
          }
          public void OpenCon()
          {
              if (oleDbComCon.State != (System.Data.ConnectionState.Open))
              {
                  oleDbComCon.Open();
              }
          }
          public void CloseCon()
          {
              if (oleDbComCon.State != (System.Data.ConnectionState.Closed))
              {
                  oleDbComCon.Close();
              }
          }
        public bool QueryUser(string userName)
        {
            bool bValid = false;
            OleDbCommand queryUserCommand = oleDbComCon.CreateCommand();
            queryUserCommand.CommandText = "select password from user_data where name='" + userName +"'";
            this.OpenCon();
            OleDbDataReader queryUserDataReader = queryUserCommand.ExecuteReader();
            queryUserDataReader.Read();
            //int i = queryUserDataReader.FieldCount;
            bValid = queryUserDataReader.HasRows;
            //queryUserDataReader.
            //string tt = queryUserDataReader[0].ToString();

            this.CloseCon();
            return (!bValid);
        }

        public void SaveUser(string userName, string passWord)
        {
            this.InsertData(userName, passWord, 0);
        }

        public DataTable GetAllData()
        {
            try
            {
                DataTable tblTable = new DataTable();
                oleDbComAdp = new OleDbDataAdapter("select * from user_data", oleDbComCon);
                oleDbComAdp.Fill(tblTable);
                return tblTable;
            }
            catch 
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                return null;
            }
        }

        public bool  QureyPassword(string userName,string passWord)
        {
            string strPassWord = "";
            bool bValid=false ;

            OleDbCommand queryUserCommand =oleDbComCon.CreateCommand();
            queryUserCommand.CommandText = "select password from user_data where name='" + userName + "'";
            this.OpenCon();
            OleDbDataReader queryUserDataReader = queryUserCommand.ExecuteReader();
            queryUserDataReader.Read();
            if (queryUserDataReader.HasRows)
                strPassWord = queryUserDataReader["password"].ToString ();
            this.CloseCon();

            if (strPassWord.ToUpper() == passWord.ToUpper())
                bValid = true;
            else
                bValid = false;
            return bValid;

        }

        public String GetDataByParameterName(string userName)
        {
            String ParameterValue;

            try
            {
                DataTable tblTable = new DataTable();
                oleDbComAdp = new OleDbDataAdapter("select * from user_data where name='" + userName + "'", oleDbComCon);
                oleDbComAdp.Fill(tblTable);
                if (tblTable == null)
                {
                    ParameterValue = "";
                    return ParameterValue;
                }
                else
                {
                    if (tblTable.Rows.Count > 0)
                    {
                        ParameterValue = Convert.ToString(tblTable.Rows[0].ItemArray.GetValue(1));
                    }
                    else
                    {
                        ParameterValue = "";
                    }
                    return ParameterValue;
                }

            }
            catch 
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                ParameterValue = "";
                return ParameterValue;
            }
        }

        public int InsertData(string userName, string passWord,int privilege)
        {
            string strCmd;
            OleDbCommand oleCMD;
            int intResult;
            try
            {
                if (GetDataByParameterName(userName) == "")
                {
                    strCmd = "Insert into user_data values('" + userName + "','" + passWord + "','" + privilege + "')";
                    oleCMD = new OleDbCommand(strCmd, oleDbComCon);
                    this.OpenCon();
                    intResult = oleCMD.ExecuteNonQuery();
                    this.CloseCon();
                    return intResult;
                }
                else
                {
                    intResult = UpdateData(userName, passWord, privilege);
                    return intResult;
                }

            }
            catch 
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int UpdateData(string userName, string passWord,int privilege)
        {
            string strCmd;
            OleDbCommand oleCMD;
            int intResult;
            try
            {
                strCmd = "Update user_data set password ='" + passWord + "'   where name='" + userName + "'";
                oleCMD = new OleDbCommand(strCmd, oleDbComCon);
                this.OpenCon();
                intResult = oleCMD.ExecuteNonQuery();
                this.CloseCon();
                return intResult;
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int DeleteAllData()
        {
            string strCmd;
            OleDbCommand oleCMD;
            int intResult;
            try
            {
                strCmd = "delete from user_data";
                oleCMD = new OleDbCommand(strCmd, oleDbComCon);
                this.OpenCon();
                intResult = oleCMD.ExecuteNonQuery();
                this.CloseCon();
                return intResult;
            }
            catch 
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public static OleDbConnection oleDbComCon;
        public static OleDbDataAdapter oleDbComAdp;
    
    }
}
