using System;
using System.IO;
using OmniSharp.Solution;

namespace OmniSharp.Build
{
    public class BuildCommandBuilder
    {
        private readonly ISolution _solution;
        private static bool IsUnix
        {
            get
            {
                var p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public BuildCommandBuilder(ISolution solution)
        {
            _solution = solution;
        }

        public string Executable
        {
            get
            {
                return IsUnix
                           ? "xbuild"
                           : Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "Msbuild.exe");
            }
        }

        public string Arguments
        {
            get { return IsUnix ? "" : "/m " + "/nologo /v:q /property:GenerateFullPaths=true \"" + _solution.FileName + "\""; }
        }
    }
}