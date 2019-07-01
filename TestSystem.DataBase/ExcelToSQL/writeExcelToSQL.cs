using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
//using System.Windows.Data;
//using System.Windows.Controls;
//using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelToSQL
{
    class writeExcelToSQL
    {      
        MySqlDataAdapter daTransceiver;

        public List<string> SendMySQLCommand(string connStr, string commandString)
        {
            // == Send MySQL Command. Return the response message
            List<string> Query_results = new List<string>();
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandString;
                MySqlDataReader rdr;
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string row = "";                   
                    row = rdr.GetValue(0).ToString();
                    Query_results.Add(row);
                }
                rdr.Close();                
            }
            catch
            {
               
            }
            conn.Close();
            return (Query_results);
        }
        
        public DataSet SendMySQLQuery(string connStr, string queryString, string selectTable)
        {
            // == Send MySQL Query. Return the query results
            DataSet dsTransceiver = new DataSet();
            
            try
            {
                daTransceiver = new MySqlDataAdapter(queryString, connStr);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daTransceiver);
                daTransceiver.Fill(dsTransceiver, selectTable);

            }
            catch
            {
               dsTransceiver = null;
            }
            return (dsTransceiver);
        }

        public Boolean writeLastRowOnly(string filename, string currentsheet, string connStr, string selectedTable)
        {

            Boolean IsSuccess = false;

            // == Open excel application, workbook then worksheet 
            Excel.Application excelApp = new Excel.Application(); 
            Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(@filename);
            Excel.Sheets excelSheets = excelWorkBook.Worksheets;
            Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelSheets.get_Item(currentsheet);

            // == Retrieve data of last record
            Excel.Range excelCell = (Excel.Range)excelWorksheet.UsedRange;   
            string[] lastRawContent = new String[excelCell.Columns.Count];
            string addRow = null;
            for (int k = 0; k < excelCell.Columns.Count; k++)
            {
                try
                {
                    lastRawContent[k] = Convert.ToDateTime(excelCell[excelCell.Rows.Count, k + 1].value).ToString("yyyy-MM-dd hh:mm:ss");                    
                }
                catch
                {
                    lastRawContent[k] = Convert.ToString(excelCell[excelCell.Rows.Count, k + 1].value);                    
                }               
                addRow += ((lastRawContent[k] == "") ? "null" : "'" + lastRawContent[k] + "'");
                if (k < (excelCell.Columns.Count - 1)) addRow += ",";
            }            
                        
            // == Close Excel file and application
            excelWorkBook.Close();
            excelApp.Quit();

            // == Open MySQL database           
            MySqlConnection conn = new MySqlConnection(connStr);    

            // == add last record into database
            addRow = "INSERT INTO " + selectedTable + " VALUES (" + addRow + ");";

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(addRow, conn);
                cmd.ExecuteNonQuery();                
                IsSuccess = true ;
            }
            catch
            {
                IsSuccess = false ;
            }
            conn.Close();
            return (IsSuccess);

        }

        public Boolean writeSelectedRows(string filename, string currentsheet, string startDate, string connStr, string selectedTable)
        {

            Boolean IsSuccess = false;
            int dateColumm = 0;

            // == Open excel application, workbook then worksheet 
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(@filename);
            Excel.Sheets excelSheets = excelWorkBook.Worksheets;
            Excel.Worksheet excelWorksheet = (Excel.Worksheet)excelSheets.get_Item(currentsheet);

            // == find the first column contained date information. Assume it is starting date/time
            Excel.Range excelCell = (Excel.Range)excelWorksheet.UsedRange;                     
            for (int k = 0; k < excelCell.Columns.Count; k++)
            {
                try
                {
                    string temp2 = Convert.ToDateTime(excelCell[excelCell.Rows.Count, k + 1].value).ToString("yyyy-MM-dd hh:mm:ss");
                    dateColumm = (k+1);
                    break;
                }
                catch
                {                    
                }
            }

            // == find all rows after date selected

             // == Open MySQL database           
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            for (int j = excelCell.Rows.Count; j >1; j--)
            {
                if (Convert.ToDateTime(excelCell[j, dateColumm].value) > Convert.ToDateTime(startDate))
                {
                    string[] NewRawContent = new String[excelCell.Columns.Count];
                    string addRow = null;
                    for (int k = 0; k < excelCell.Columns.Count; k++)
                    {
                        try
                        {
                            NewRawContent[k] = Convert.ToDateTime(excelCell[j, k + 1].value).ToString("yyyy-MM-dd hh:mm:ss");
                        }
                        catch
                        {
                            NewRawContent[k] = Convert.ToString(excelCell[j, k + 1].value);
                        }
                        addRow += ((NewRawContent[k] == "") ? "null" : "'" + NewRawContent[k] + "'");
                        if (k < (excelCell.Columns.Count - 1)) addRow += ",";
                    }

                    // == add matched record into database
                    addRow = "INSERT INTO " + selectedTable + " VALUES (" + addRow + ");";

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(addRow, conn);
                        cmd.ExecuteNonQuery();
                        IsSuccess = true;
                    }
                    catch
                    {
                        IsSuccess = false;
                    }
                }
                else
                    break;  //assume data is sorted by date. Newest one is listed on the bottom of cell. 

            }
            // == Close Excel file and application
            excelWorkBook.Close();
            excelApp.Quit();
            
            conn.Close();
            return (IsSuccess);
        }
    }
}
