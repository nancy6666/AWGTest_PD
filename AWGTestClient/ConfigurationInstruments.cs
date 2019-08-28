using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestClient
{
    class ConfigurationInstruments
    {
        Configuration cfg;

        public ConfigurationInstruments()
        {
            string configPath = $"{Directory.GetCurrentDirectory()}\\config\\Instruments.config";
            if (!File.Exists(configPath))
                throw new Exception(string.Format("配置文件不存在：{0}", configPath));
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configPath;
            cfg = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }
       
        public string PM1906Com1
        {
            get
            {
                return cfg.AppSettings.Settings["PM1906Com1"].Value;
            }
        }

        public string PM1906Com2
        {
            get
            {
                return cfg.AppSettings.Settings["PM1906Com2"].Value;
            }
        }
       

        public string PM1906Com3
        {
            get
            {
                return cfg.AppSettings.Settings["PM1906Com3"].Value;
            }
        }
       

        public string PM1906Com4
        {
            get
            {
                return cfg.AppSettings.Settings["PM1906Com4"].Value;
            }
        }
        public int PM1906Rate
        {
            get
            {
                return Convert.ToInt32(cfg.AppSettings.Settings["PM1906Rate"].Value);
            }
        }

        public string UC872Com
        {
            get
            {
                return cfg.AppSettings.Settings["UC872Com"].Value;
            }
        }
        public string UC872Rate
        {
            get
            {
                return cfg.AppSettings.Settings["UC872Rate"].Value;
            }
        }
        public string PowerMeterType
        {
            get
            {
                return cfg.AppSettings.Settings["PowerMeterType"].Value;
            }
        }
        public string PM1906_Range
        {
            get
            {
                return cfg.AppSettings.Settings["PM1906_Range"].Value;
            }
        }
    }
}
