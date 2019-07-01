using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSystem.InternalData;
using System.Collections.Specialized;	// StringDictionary
using System.Globalization;				// CultureInfo

namespace TestSystem.DataRead
{
    public class TestDataeRead
    {

        public TestDataeRead()
        {

        }

        /// <summary>
        /// Extracts key parameters from the keyTable
        /// </summary>
        /// <param name="keyTable">Name/value pairs of keys</param>
        private void ProcessKeyData(StringDictionary keyTable)
        {
            // Clear any existing data
            inputKeyData = new deviceInfo();
            keyDataWhereClause = new StringBuilder();

            foreach (string key in keyTable.Keys)
            {
                string keyName = key.ToUpper(CultureInfo.InvariantCulture);
                switch (keyName)
                {
                    case "DEVICE_TYPE":
                        inputKeyData.deviceType = keyTable[key].ToLower(CultureInfo.InvariantCulture);		// Always Lowercase in PCAS
                        break;
                    case "TEST_STAGE":
                        inputKeyData.testStage = keyTable[key].ToLower(CultureInfo.InvariantCulture);		// Always Lowercase in PCAS
                        break;
                    //case "SPECIFICATION":
                    //    inputKeyData.specification = keyTable[key].ToLower(CultureInfo.InvariantCulture);	// Always Lowercase in PCAS
                    //    break;
                    case "SERIAL_NO":
                        inputKeyData.serialNo = keyTable[key].ToUpper(CultureInfo.InvariantCulture);		// Always Uppercase in PCAS
                        break;
                    case "TABLE_NAME":
                        inputKeyData.tableName = keyTable[key].ToLower(CultureInfo.InvariantCulture);			// Always Lowercase in PCAS
                        break;

                    default:
                        // Use any extra key data for a where clause
                        if (keyDataWhereClause.Length > 0)
                            keyDataWhereClause.Append(" and ");

                        keyDataWhereClause.Append(keyName + "=" + keyTable[key]);
                        //logger.DebugOut("PcasRead", "Extra key information : " + keyDataWhereClause.ToString());
                        break;
                }
            }
            // Check that the required keys were present
            CheckKeyData();
        }

        /// <summary>
        ///  Checks for existence of keys.
        /// SERIAL_NO and SCHEMA must be present. Others are optional.
        /// </summary>
        private void CheckKeyData()
        {


            StringBuilder missingKeys = new StringBuilder();

            if (inputKeyData.serialNo.Length == 0)
                missingKeys.Append("SERIAL_NO ");


            if (missingKeys.Length > 0)
            {
                throw new Exception("Missing key data for '" + missingKeys.ToString());
            }

        }

        /// <summary>
        /// Retrieves latest results for the device specified by keyObject
        /// </summary>
        /// <param name="keyObject">Must contain SCHEMA and SERIAL_NUMBER. May contain one or more of DEVICE_TYPE , TEST_STAGE , SPECIFICATION. Additional value pairs will be treated as strings data and appended to the where clause.</param>
        /// <param name="retrieveRawData">Not implemented in this release..</param>
        /// <returns>DatumList containing a single set of results.</returns>
        public DatumList GetLatestResults(StringDictionary keyObject, bool retrieveRawData)
        {
            DatumList datumList = this.GetLatestResults(keyObject, retrieveRawData, "");
            return datumList;
        }
        public DatumList GetSameTimeResults(StringDictionary keyObject, bool retrieveRawData,string time)
        {
            DatumList datumList = this.GetSameTimeResults(keyObject, retrieveRawData, "", time);
            return datumList;
        }

        public DatumList GetLatestResults(StringDictionary keyObject, bool retrieveRawData, string whereClause)
        {
            ProcessKeyData(keyObject);

            DataRead pcasRead = null;
			DatumList resultsData = null;
			string fullWhereClause = whereClause;

            try
            {
                pcasRead = new DataRead();
                pcasRead.DeviceInfo = inputKeyData;

                if (keyDataWhereClause.Length > 0)
                {
                    // use any extra key data to form a where clause
                    if (whereClause == null || whereClause.Length == 0)
                    {
                        fullWhereClause = keyDataWhereClause.ToString();
                    }
                    else
                    {
                        fullWhereClause = keyDataWhereClause.ToString() + " and " + whereClause;
                    }
                }

                if (fullWhereClause.Length == 0)
                {
                    // No WHERE clause.

                    // Find latest results from test_link using type / stage / spec if present
                    resultsData = pcasRead.getLatestResultsForDevice(inputKeyData.serialNo, inputKeyData.testStage, inputKeyData.deviceType, "", retrieveRawData,inputKeyData.tableName);
                }
            }
            finally
            {
                // Always call Disconnect
                if (pcasRead != null)
                    pcasRead.Disconnect();
            }

            return resultsData;

        }

        public DatumList GetSameTimeResults(StringDictionary keyObject, bool retrieveRawData, string whereClause,string time)
        {
            ProcessKeyData(keyObject);

            DataRead pcasRead = null;
            DatumList resultsData = null;
            string fullWhereClause = whereClause;

            try
            {
                pcasRead = new DataRead();
                pcasRead.DeviceInfo = inputKeyData;

                if (keyDataWhereClause.Length > 0)
                {
                    // use any extra key data to form a where clause
                    if (whereClause == null || whereClause.Length == 0)
                    {
                        fullWhereClause = keyDataWhereClause.ToString();
                    }
                    else
                    {
                        fullWhereClause = keyDataWhereClause.ToString() + " and " + whereClause;
                    }
                }

                if (fullWhereClause.Length == 0)
                {
                    // No WHERE clause.

                    // Find latest results from test_link using type / stage / spec if present
                    resultsData = pcasRead.getSameTimeResultsForDevice(inputKeyData.serialNo, inputKeyData.testStage, inputKeyData.deviceType, "", retrieveRawData, inputKeyData.tableName,time);
                }
            }
            finally
            {
                // Always call Disconnect
                if (pcasRead != null)
                    pcasRead.Disconnect();
            }

            return resultsData;

        }

        internal StringBuilder keyDataWhereClause;
        /// <summary>
        /// Used by the Logging component as the 'searchLabel' 
        /// </summary>
        const string LOG_LABEL = "ExtData_PcasDataRead";
        /// <summary>
        /// A structure containing key data relating to one device.
        /// </summary>
        internal deviceInfo inputKeyData;


    }
    /// <summary>
    /// A structure containing key data relating to one device.
    /// </summary>
    internal struct deviceInfo
    {
        /// <summary>
        /// Serial number of the device
        /// </summary>
        internal string serialNo;
        /// <summary>
        /// PCAS device type
        /// </summary>
        internal string deviceType;
        /// <summary>
        /// PCAS test stage
        /// </summary>
        internal string testStage;

        internal string tableName;

    }
}
