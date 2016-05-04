using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OmniSharp.Configuration
{
    public class ConfigurationLoader
    {
        private static OmniSharpConfiguration _config = new OmniSharpConfiguration();
        public static OmniSharpConfiguration Load()
        {
            return Load(configLocation: "", clientMode: null);
        }

        public static OmniSharpConfiguration Load(string configLocation, string clientMode)
        {
            if (string.IsNullOrEmpty(configLocation))
            {
                string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                configLocation = Path.Combine(executableLocation, "config.json");
            }
            var config = StripComments(File.ReadAllText(configLocation));
	    try
	    {
	        _config = new Nancy.Json.JavaScriptSerializer().Deserialize<OmniSharpConfiguration>(config);
		_config.ConfigFileLocation = configLocation;
	    } catch (System.ArgumentException e)
	    {
		Console.WriteLine(e.Message);
		const string pattern = @"\(([0-9]+)\)$";
		int offset;
		if (int.TryParse(Regex.Match(e.Message, pattern).Groups[1].Value, out offset))
                {
                    string[] lines = config.Replace("\r\n","\n").Replace("\n\r","\n").Split('\n');

                    int lengthSum = 0;
                    for (int i = 0; i<lines.Length; i++)
                    {
                        lengthSum += lines[i].Length;
                        if ((lengthSum + lines[i].Length) >= offset)
                        {
                            Console.WriteLine(configLocation + "(" + i + "," + (offset-lengthSum) + "):"+lines[i]);
                            break;
                        }
                    }
                    Environment.Exit(1);
                    // The system cannot work because of a User
                    // error. Therefore we Exit(). If we would throw
                    // the error again, we would trigger a stacktrace.
                    // That would lead the user to think this is a
                    // programming error.
                }
            }

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

        private static string StripComments(string json)
        {
            const string pattern = @"/\*(?>(?:(?>[^*]+)|\*(?!/))*)\*/";

            return Regex.Replace(json, pattern, string.Empty, RegexOptions.Multiline);    
        }

        public static OmniSharpConfiguration Config { get { return _config; }}
    }

}
