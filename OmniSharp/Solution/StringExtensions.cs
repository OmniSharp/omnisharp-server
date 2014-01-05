using System;
using System.IO;
using System.Text.RegularExpressions;

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
    }
}