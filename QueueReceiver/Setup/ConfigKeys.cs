using System;
using System.Configuration;
using Auth.FWT.Core.Extensions;

namespace QueueReceiver
{
    internal class ConfigKeys
    {
        public static string RedisConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Redis"].ConnectionString; }
        }

        public static string ServiceBus
        {
            get { return ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString; }
        }

        public static string RedisServer
        {
            get { return Setting("RedisServer"); }
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