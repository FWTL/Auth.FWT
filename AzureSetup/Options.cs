using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSetup
{
    public class Options
    {
        public Options(string name)
        {
            NAME = name;
            Util._rng = new Random(name.GetHashCode());
        }

        public string NAME { get; }
        public string AD_APP_APPLICATIONID { get; } = "e81245c3-cd57-4e12-af11-690e2124deba";
        public string AD_APP_SECRET { get; } = "eecfez+THhRtySlckKSeoBu4O2Mu1Qe15aaiKUuSH4g=";

        public string ASPNETCORE_ENVIRONMENT { get; } = "TEST";

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
