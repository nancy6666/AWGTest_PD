using System;
using System.Configuration;

namespace SqlFunctions
{
   public class CConfigurationManagement
    {
        Configuration cfg;

        public CConfigurationManagement()
        {
            cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string DBIP
        {
            get
            {
                return cfg.AppSettings.Settings["DBIP"].Value;
            }
        }

        public string DBName
        {
            get
            {
                return cfg.AppSettings.Settings["DBName"].Value;
            }
        }

        public string DBUser
        {
            get
            {
                return cfg.AppSettings.Settings["DBUser"].Value;
            }
        }

        public string DBPassword
        {
            get
            {
                return cfg.AppSettings.Settings["DBPwd"].Value;
            }
        }
        public string RawDataPath
        {
            get
            {
                return cfg.AppSettings.Settings["RawDataPath"].Value;
            }
        }
        public string PNNameFormat
        {
            get
            {
                return cfg.AppSettings.Settings["PNNameFormat"].Value;
            }
        }
        public string WaferFormat
        {
            get
            {
                return cfg.AppSettings.Settings["WaferFormat"].Value;
            }
        }
        public string ChipIDFormat
        {
            get
            {
                return cfg.AppSettings.Settings["ChipIDFormat"].Value;
            }
        }
        public string MaskFormat
        {
            get
            {
                return cfg.AppSettings.Settings["MaskFormat"].Value;
            }
        }
    }
}
