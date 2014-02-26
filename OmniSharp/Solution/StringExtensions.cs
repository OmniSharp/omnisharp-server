using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using OmniSharp.Common;
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
                path = CygPathWrapper.GetCygpath(path, false);
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
                path = CygPathWrapper.GetCygpath(path, true);
            }
            foreach (var pathReplacement in ConfigurationLoader.Config.PathReplacements)
            {
                path = path.Replace(pathReplacement.To, pathReplacement.From);
            }
            return path;
        }

        
    }
}
