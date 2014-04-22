using System;
using System.IO;
using System.Reflection;

namespace OmniSharp.Configuration
{
    public class ConfigurationLoader
    {
        private static OmniSharpConfiguration _config = new OmniSharpConfiguration();
        public static OmniSharpConfiguration Load()
        {
            return Load(null);
        }

        public static OmniSharpConfiguration Load(string clientMode)
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configLocation = Path.Combine(executableLocation, "config.json");
            var config = File.ReadAllText(configLocation);
            _config = new Nancy.Json.JavaScriptSerializer().Deserialize<OmniSharpConfiguration>(config);

            if (!string.IsNullOrWhiteSpace(clientMode))
            {
                _config.ClientPathMode = (PathMode)Enum.Parse(typeof(PathMode), clientMode);
            }
            if (_config.ServerPathMode == null)
            {
                _config.ServerPathMode = PlatformService.IsUnix ? PathMode.Unix : PathMode.Windows;
            }
            return _config;
        }

        public static OmniSharpConfiguration Config { get { return _config; }}
    }

}
