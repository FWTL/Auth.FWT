using System;
using System.Configuration;
using Auth.FWT.Core.Extensions;

namespace Auth.FWT.Core
{
    public static class ConfigKeys
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["AppContext"].ConnectionString;
            }
        }

        public static string HangfireConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["FWTHangfire"].ConnectionString;
            }
        }

        public static string RedisConnectionString
        {
            get { return Setting("RedisConnectionString"); }
        }

        public static string TelegramApiHash
        {
            get { return Setting("TelegramApiHash"); }
        }

        public static int TelegramApiId
        {
            get { return Setting<int>("TelegramApiId"); }
        }

        public static string AzureBusConnectionString
        {
            get { return Setting("AzureBusConnectionString"); }
        }

        private static T Setting<T>(string name) where T : struct
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value.IsNull())
            {
                throw new Exception(string.Format("Could not find setting '{0}',", name));
            }

            return value.To<T>();
        }

        private static string Setting(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value.IsNull())
            {
                throw new Exception(string.Format("Could not find setting '{0}',", name));
            }

            return value;
        }
    }
}