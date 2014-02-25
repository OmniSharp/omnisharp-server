using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OmniSharp.Configuration
{
    public class ConfigurationLoader
    {
        private static OmniSharpConfiguration _config = new OmniSharpConfiguration();
        public static void Load()
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configLocation = Path.Combine(executableLocation, "config.json");
            var config = File.ReadAllText(configLocation);
            _config = new Nancy.Json.JavaScriptSerializer().Deserialize<OmniSharpConfiguration>(config);
        }

        public static OmniSharpConfiguration Config { get { return _config; }}
    }
    public class OmniSharpConfiguration
    {
        public OmniSharpConfiguration()
        {
            PathReplacements = new List<PathReplacement>();
        }
        public IEnumerable<PathReplacement> PathReplacements { get; set; }
        public bool? UseCygpath { get; set; }
    }

    public class PathReplacement
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
