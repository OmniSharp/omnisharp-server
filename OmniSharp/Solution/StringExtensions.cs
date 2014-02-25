using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using OmniSharp.Configuration;

namespace OmniSharp.Solution
{
    public static class StringExtensions
    {

        /// <example>
        ///   "  " -> " ".
        ///   "foo   \n  bar" -> "foo bar".
        /// </example>
        public static string MultipleWhitespaceCharsToSingleSpace
            (this string stringToTrim) {
            return Regex.Replace(stringToTrim, @"\s+", " ");
        }

        public static string LowerCaseDriveLetter(this string path)
        {
        	return path.Replace(@"C:\", @"c:\").Replace(@"D:\", @"d:\");
        }

		public static string ForceWindowsPathSeparator(this string path)
		{
			return path.Replace ('/', '\\');
		}

		public static string ForceNativePathSeparator(this string path)
		{
			return path.Replace ('\\', Path.DirectorySeparatorChar);
		}

        /// <summary>
        /// Returns the relative path of a file to another file
        /// </summary>
        /// <param name="path">Base path to create relative path</param>
        /// <param name="pathToMakeRelative">Path of file to make relative against path</param>
        /// <returns></returns>
        public static string GetRelativePath(this string path, string pathToMakeRelative)
        {
            return new Uri(path).MakeRelativeUri(new Uri(pathToMakeRelative)).ToString().ForceWindowsPathSeparator();
        }

        public static string ApplyPathReplacementsForServer(this string path)
        {
            if (ConfigurationLoader.Config.UseCygpath.GetValueOrDefault(false))
            {
                path = ApplyCygpath(path, false);
            }
            foreach (var pathReplacement in ConfigurationLoader.Config.PathReplacements)
            {
                path = path.Replace(pathReplacement.From, pathReplacement.To);
            }
            return path;
        }

        public static string ApplyPathReplacementsForClient(this string path)
        {
            if (ConfigurationLoader.Config.UseCygpath.GetValueOrDefault(false))
            {
                path = ApplyCygpath(path, true);
            }
            foreach (var pathReplacement in ConfigurationLoader.Config.PathReplacements)
            {
                path = path.Replace(pathReplacement.To, pathReplacement.From);
            }
            return path;
        }

        private static IDictionary<bool,Process> CygpathProcesses = new Dictionary<bool,Process>();
        private static IDictionary<bool,IDictionary<String,String>> CygpathCache = new Dictionary<bool,IDictionary<String,String>>
        {
            {true, new Dictionary<String,String>()},
            {false, new Dictionary<String,String>()},
        };

        private static string ApplyCygpath(string path, bool toUnix)
        {
            string convertedPath;
            if (!CygpathCache[toUnix].TryGetValue(path, out convertedPath))
            {
                lock(CygpathProcesses)
                {
                    Process process;
                    if (!CygpathProcesses.TryGetValue(toUnix, out process) || process.HasExited)
                    {
                        var processStartInfo = new ProcessStartInfo("cygpath", "-f - "+(toUnix?"-u":"-w"))
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                        };

                        process = CygpathProcesses[toUnix] = Process.Start(processStartInfo);
                    }

                    process.StandardInput.WriteLine(path);
                    convertedPath = process.StandardOutput.ReadLine();
                }
            }
            return convertedPath;
        }
    }
}
