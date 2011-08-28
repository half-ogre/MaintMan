using System;
using System.Configuration;

namespace MaintMan
{
    public class Configuration : IConfiguration
    {
        public static string ReadFromConfig(
               string key,
               string defaultValue = null)
        {
            var configKey = "MaintMan:" + key;

            var configValue = ConfigurationManager.AppSettings[configKey];

            return configValue ?? defaultValue;
        }

        public string BaseUrl
        {
            get
            {
                return new Lazy<string>(() =>
                    Configuration.ReadFromConfig("BaseUrl", "http://localhost")).Value;
            }
        }
    }
}