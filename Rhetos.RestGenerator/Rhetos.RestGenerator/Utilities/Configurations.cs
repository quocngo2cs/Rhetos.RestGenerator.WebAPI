using System;
using System.Configuration;

namespace Rhetos.WebApiRestGenerator.Utilities
{
    public static class Configurations
    {
        private static Lazy<string> webApiHost = new Lazy<string>(() =>
        {
            var host = ConfigurationSettings.AppSettings["WebApiRest.Host"];
            return string.IsNullOrWhiteSpace(host) ? "localhost" : host;
        });
        public static string WebApiHost
        {
            get
            {
                return webApiHost.Value;
            }
        }

        private static Lazy<int> webApiPort = new Lazy<int>(() =>
        {
            var portInString = ConfigurationSettings.AppSettings["WebApiRest.Port"];
            int port;

            if (int.TryParse(portInString, out port))
            {
                return port;
            }

            return new Random().Next();
        });
        public static int WebApiPort
        {
            get
            {
                return webApiPort.Value;
            }
        }
    }
}
