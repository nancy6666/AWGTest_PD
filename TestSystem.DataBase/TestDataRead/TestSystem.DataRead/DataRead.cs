using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;				// OleDbConnection
using TestSystem.InternalData;
using System.Globalization;				// CultureInfo
using MySql.Data;
using MySql.Data.MySqlClient;
namespace TestSystem.DataRead
{
    internal class DataRead
    {
        //internal string connStr = "server=localhost;user=root;database=40g_qsfp;port=3306;password=;";
        //internal string connStr = "server=MySQLSERVER;user=meirong;database=40g_qsfp;port=3306;password=123;";
        internal string connStr = "server=192.168.100.244;user=root;database=" + "irixi_sz_wafer_measurement_statistics" + ";port=3308;password=0000;";

        internal DataRead()
        {

            #region Connect
            //Connect("Provider=MSDAORA; Data Source=" + DataSource + "; User ID=pcas; Password=test");
            Connect(connStr);
            #endregion
        }

                /// <summary>
        /// Connects to the data source
        /// </summary>
        /// <param name="ConnectionString">The database connection string</param>
        private void Connect(string ConnectionString)
        {
            dbConnection = new MySqlConnection(ConnectionString);
            dbConnection.Open();
        }

        /// <summary>
        /// Disconnect from the data source
        /// </summary>
        internal void Disconnect()
        {
            dbConnection.Close();
        }



        /// <summary>
        ///  This is the method used if no WHERE clause is specified.
        /// </summary>
        /// <param name="serialNo">The device's SERIAL_NO</param>
        /// <param name="testStage">The device's TEST_STAGE</param>
        /// <param name="deviceType">The device's DEVICE_TYPE</param>
        /// <param name="specification">The device's SPECIFICATION</param>
        /// <param name="retrieveRawData">True if Blob Data is to be retrieved.</param>
        /// <returns>Datum list containing all previous results data</returns>
        internal DatumList getLatestResultsForDevice(string serialNo, string testStage, string deviceType, string specification, bool retrieveRawData, string tableName)
        {
            DatumList datumList = getLatestResultsFromTableGroups(serialNo, tableName, retrieveRawData);

            return datumList;
        }

        internal DatumList getSameTimeResultsForDevice(string serialNo, string testStage, string deviceType, string specification, bool retrieveRawData, string tableName, string time)
        {
            DatumList datumList = getSameTimeResultsFromTableGroups(serialNo, tableName, retrieveRawData,time);

            return datumList;
        }
        /// <summary>
        /// Retrieves the latest results from a set of related tables.
        /// </summary>
        /// <param name="serialNo">The PCAS serial number</param>
        /// <param name="resultsTableGroup">The PCAS results table group ( e.g. a0152 )</param>
        /// <param name="retrieveRawData">True if Blob Data is to be retrieved.</param>
        /// <returns>A single set of results.</returns>
        private DatumList getLatestResultsFromTableGroups(string serialNo, string resultsTableGroup, bool retrieveRawData)
        {
            //ArrayList tableList = resultsTablesForGroup(resultsTableGroup);
            DatumList datumList = new DatumList();

            //for (int i = 0; i < tableList.Count; i++)
            //{
            string fullResultsTable = resultsTableGroup;

                //string sqlStmt	=	"select * from "+fullResultsTable+" alias" +
                //    " where SERIAL_NO='" + serialNo + "'" +
                //    " and time_date = (select max(time_date) from "+fullResultsTable+" where alias.serial_no=serial_no)";

                //string sqlStmt = "select * from " + fullResultsTable +
                //       " where SERIAL_NO='" + serialNo + "'" +
                //    " and time_date = (select max(time_date) from " + fullResultsTable + " where serial_no='" + serialNo + "')";
            string sqlStmt = "select * from " + fullResultsTable + " where Irixi_Serial_Num='" + serialNo + "'" +
            " and Time_Stamp_Start = (select max(Time_Stamp_Start) from " + fullResultsTable + " where Irixi_Serial_Num='" + serialNo + "')";


                getResultsFromSingleTableGroup(sqlStmt, datumList, retrieveRawData);
            //}

            return datumList;
        }

        private DatumList getSameTimeResultsFromTableGroups(string serialNo, string resultsTableGroup, bool retrieveRawData,string time)
        {
            //ArrayList tableList = resultsTablesForGroup(resultsTableGroup);
            DatumList datumList = new DatumList();

            //for (int i = 0; i < tableList.Count; i++)
            //{
            string fullResultsTable = resultsTableGroup;

            //string sqlStmt	=	"select * from "+fullResultsTable+" alias" +
            //    " where SERIAL_NO='" + serialNo + "'" +
            //    " and time_date = (select max(time_date) from "+fullResultsTable+" where alias.serial_no=serial_no)";

            //string sqlStmt = "select * from " + fullResultsTable +
            //       " where SERIAL_NO='" + serialNo + "'" +
            //    " and time_date = (select max(time_date) from " + fullResultsTable + " where serial_no='" + serialNo + "')";
            string sqlStmt = "select * from " + fullResultsTable + " where Irixi_Serial_Num='" + serialNo + "'" +
            " and Time_Stamp_Start ='" + time + "'";


                getResultsFromSingleTableGroup(sqlStmt, datumList, retrieveRawData);
            //}

            return datumList;
        }

        /// <summary>
        /// Retrieve a single set of results from a related group of one or more tables ( e.g. a0012_0 a0012_1 etc )
        /// </summary>
        /// <param name="sqlStmt">A SQL statement</param>
        /// <param name="resultsData">The datumList to update</param>
        /// <param name="retrieveRawData">True if Blob Data is to be retrieved.</param>
        private void getResultsFromSingleTableGroup(string sqlStmt, DatumList resultsData, bool retrieveRawData)
        {
            MySqlCommand myCommand = new MySqlCommand(sqlStmt, dbConnection);
            MySqlDataReader myReader;

            myReader = myCommand.ExecuteReader();

            try
            {
                if (myReader.Read() == false)
                {
                    myReader.Close();
                    return;
                }

                createDatumListFromSingleTable(resultsData, myReader, retrieveRawData);
            }
            finally
            {
                myReader.Close();
            }
        }

        /// <summary>
        /// Joins data found in one or more related tables into a single datumList.
        /// </summary>
        /// <param name="resultsData">The datumList to update.</param>
        /// <param name="myReader">An existing instance of a DataReader</param>
        /// <param name="retrieveRawData">True if Blob Data is to be retrieved.</param>
        private void createDatumListFromSingleTable(DatumList resultsData, MySqlDataReader myReader, bool retrieveRawData)
        {

            string nodeNumber = "";
            bool updateFilePaths = false;

            // Deal with each field
            for (int icol = 0; icol < myReader.FieldCount; icol++)
            {
                string nameAsString = myReader.GetName(icol);
                Type t = myReader.GetFieldType(icol);

                string valAsString = "";
                double valAsDouble = 0;
                Int32 valAsInteger = 0;
                bool valAsBool = false;


                // using TYPE create an appropriate Datum. Retrieve data as correct type. Add to DatumList.
                switch (t.ToString())
                {
                    case "System.String":
                        if (!myReader.IsDBNull(icol))	// ignore null strings
                        {
                            valAsString = myReader.GetString(icol);
                            // Desc/val lists may be already partially filled with _0 results in which case SPEC_ID , SERIAL_NO & TIME_DATE are already present

                            if (nameAsString == "SERIAL_NO")	// Check whether the field name is "SERIAL_NO"
                            {
                                // See if it already exists
                                if (resultsData.IsPresent("SERIAL_NO"))
                                {
                                    string serialNo = resultsData.ReadString("SERIAL_NO");
                                    // Check that it matches the existing value
                                    if (valAsString != serialNo)
                                    {
                                        myReader.Close();
                                    }
                                    // Don't store it again.
                                    break;
                                }

                            }

                            if (nameAsString == "TIME_DATE")	// Check whether the field name is "TIME_DATE"
                            {
                                // See if it already exixts
                                if (resultsData.IsPresent("TIME_DATE"))
                                {
                                    string timeDate = resultsData.ReadString("TIME_DATE");
                                    // Check that it matches the existing value
                                    if (valAsString != timeDate)
                                    {
                                        myReader.Close();
                                    }
                                    // Don't store it again.
                                    break;
                                }
                            }

                            if (nameAsString == "SPEC_ID")
                            {
                                // See if it already exixts
                                if (resultsData.IsPresent("SPEC_ID"))
                                {
                                    string specId = resultsData.ReadString("SPEC_ID");
                                    // Check that it matches the existing value
                                    if (valAsString != specId)
                                    {
                                        myReader.Close();
                                    }
                                    // Don't store it again.
                                    break;
                                }
                            }

                            // deal with *plot* , *image* , raw_* etc
                            if ((nameAsString.StartsWith("FILE_")
                                && (!nameAsString.EndsWith("_TIMESTAMP"))) ||
                                nameAsString.EndsWith("_FILE") ||
                                (nameAsString.Contains("_FILE_")
                                && (!nameAsString.EndsWith("_TIMESTAMP"))) ||
                                nameAsString.StartsWith("PLOT_") ||
                                nameAsString.EndsWith("_PLOT") ||
                                nameAsString.Contains("_PLOT_") ||
                                nameAsString.StartsWith("IMAGE_") ||
                                nameAsString.EndsWith("_IMAGE") ||
                                nameAsString.Contains("_IMAGE_") ||
                                (nameAsString.StartsWith("MATRIX_") &&
                                (!nameAsString.Equals("MATRIX_NUMBER"))) ||
                                nameAsString.EndsWith("_MATRIX") ||
                                nameAsString.Contains("_MATRIX_") ||
                                nameAsString.StartsWith("RAW_") ||
                                nameAsString.EndsWith("_RAW") ||
                                nameAsString.Contains("_RAW_"))
                            {
                                //Only capture Blob Data results if required.
                                if (retrieveRawData)
                                {
                                    // default the file directory path to "\\unknownServerLocation", just so that the
                                    // UpdateFilePaths function can work properly (abuse of DatumFileLink!)
                                    string filePath = @"\\unknownServer\Location";
                                    int lastSlash = valAsString.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                                    if (lastSlash > -1)
                                    {
                                        filePath = valAsString.Substring(0, lastSlash - 1); // The path is not expected to have a trailing slash
                                        valAsString = valAsString.Substring(lastSlash, valAsString.Length - lastSlash);
                                    }
                                    else
                                    {
                                        // The full file path was not stored. 
                                        // Using our skill and judgement we will need to create this.
                                        updateFilePaths = true;
                                    }

                                    DatumFileLink newDatum = new DatumFileLink(nameAsString, filePath, valAsString);
                                    resultsData.Add(newDatum);
                                }
                            }
                            else
                            {
                                // This is not a 'special' value. Add it to our list. We may need NODE later on in the code, so capture this value.
                                DatumString stringDatum = new DatumString(nameAsString, valAsString);
                                resultsData.Add(stringDatum);
                                if (nameAsString == "NODE")
                                {
                                    // Capture NODE
                                    nodeNumber = valAsString;
                                }
                            }
                        }			// ! myReader.IsDBNull(icol)
                        break;

                    case "System.Boolean":
                        if (!myReader.IsDBNull(icol))
                        {
                            valAsBool = myReader.GetBoolean(icol);
                            DatumBool boolDatum = new DatumBool(nameAsString, valAsBool);
                            resultsData.Add(boolDatum);
                        }
                        break;

                    case "System.Double":
                        if (!myReader.IsDBNull(icol))
                        {
                            valAsDouble = myReader.GetDouble(icol);
                            DatumDouble dblDatum = new DatumDouble(nameAsString, valAsDouble);
                            resultsData.Add(dblDatum);
                            if (nameAsString == "NODE")
                            {
                                // Capture NODE
                                nodeNumber = valAsDouble.ToString(CultureInfo.InvariantCulture);
                            }
                        }
                        break;

                    case "System.Decimal":
                        if (!myReader.IsDBNull(icol))
                        {
                            valAsInteger = checked((int)myReader.GetDecimal(icol));

                            if ((nameAsString.IndexOf("BOOL_") == 0) && ((valAsInteger == 0) || (valAsInteger == 1)))
                            {
                                //Then this is apseudo boolean parameter (supported by Test Engine based soltuions only).
                                DatumBool boolDatum = new DatumBool(nameAsString, Convert.ToBoolean(valAsInteger));
                                resultsData.Add(boolDatum);
                            }
                            else
                            {
                                DatumSint32 decDatum = new DatumSint32(nameAsString, valAsInteger);
                                resultsData.Add(decDatum);
                            }

                            if (nameAsString == "NODE")
                            {
                                // Capture NODE
                                nodeNumber = valAsInteger.ToString(CultureInfo.InvariantCulture);
                            }
                        }
                        break;

                    default:
                        break;
                }

            } // icol

            if (updateFilePaths)
            {

            }
        }

        /// <summary>
        /// Gets/Sets the value of the keyInfo structure.
        /// </summary>
        internal deviceInfo DeviceInfo
        {
            set { keyInfo = value; }
            get { return keyInfo; }
        }

                /// <summary>
        /// Database connection
        /// </summary>
        MySqlConnection dbConnection;
        /// <summary>
        /// Key data structure
        /// </summary>
        private deviceInfo keyInfo;
    }
}
