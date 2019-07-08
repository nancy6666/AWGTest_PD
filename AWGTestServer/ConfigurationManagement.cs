using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWGTestServer
{
    class ConfigurationManagement
    {
        Configuration cfg;

        public ConfigurationManagement()
        {
            string configPath = $"{Directory.GetCurrentDirectory()}\\config\\Instruments.config";
            if (!File.Exists(configPath))
                throw new Exception(string.Format("配置文件不存在：{0}", configPath));
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configPath;
            cfg = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }
        public string SocketIP
        {
            get
            {
                return cfg.AppSettings.Settings["SocketIP"].Value;
            }
        }
        public string SocketPort
        {
            get
            {
                return cfg.AppSettings.Settings["SocketPort"].Value;
            }
        }
        public string K8164BGPIB
        {
            get
            {
                return cfg.AppSettings.Settings["K8164BGPIB"].Value;
            }
        }
        public string N7786BGPIB
        {
            get
            {
                return cfg.AppSettings.Settings["N7786BGPIB"].Value;
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
    }
}
