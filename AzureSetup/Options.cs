using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace AzureSetup
{
    public class Options
    {
        public Options(string name)
        {
            NAME = name;
            Util._rng = new Random(ConfigurationManager.AppSettings["RANDOM_HASH"].GetHashCode());
        }

        public string NAME { get; }
        public string AD_APP_APPLICATIONID { get; set; } = ConfigurationManager.AppSettings["AD_APP_APPLICATIONID"];
        public string AD_APP_SECRET { get; set; } = ConfigurationManager.AppSettings["AD_APP_SECRET"];

        public string READ_AD_APP_APPLICATIONID { get; set; } = ConfigurationManager.AppSettings["READ_AD_APP_APPLICATIONID"];
        public string READ_AD_APP_OBJECT_ID { get; set; } = ConfigurationManager.AppSettings["READ_AD_APP_OBJECT_ID"];
        public string READ_AD_APP_SECRET { get; set; } = ConfigurationManager.AppSettings["READ_AD_APP_SECRET"];

        public string ASPNETCORE_ENVIRONMENT { get; set; } = "Development";

        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        public string AddSettings(string key, string value)
        {
            if (!_settings.ContainsKey(key))
            {
                _settings.Add(key, value);
            }

            return value;
        }

        public string GetKeyValue(string key)
        {
            return _settings[key];
        }

        public void WriteToFile()
        {
            using (StreamWriter writetext = new StreamWriter("settings.txt"))
            {
                foreach (var setting in _settings)
                {
                    writetext.WriteLine($"{setting.Key} : {setting.Value}");
                }
            }
        }
    }
}