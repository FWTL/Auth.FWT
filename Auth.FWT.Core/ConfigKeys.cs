using System;
using System.Configuration;
using Auth.FWT.Core.Extensions;

namespace Auth.FWT.Core
{
    public static class ConfigKeys
    {
        public static string Captcha
        {
            get { return Setting("Captcha"); }
        }

        public static string RedisConnectionString
        {
            get { return Setting("Captcha"); }
        }

        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["AppContext"].ConnectionString;
            }
        }

        private static T Setting<T>(string name) where T : struct
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                throw new Exception(string.Format("Could not find setting '{0}',", name));
            }

            return value.To<T>();
        }

        private static string Setting(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                throw new Exception(string.Format("Could not find setting '{0}',", name));
            }

            return value;
        }
    }
}