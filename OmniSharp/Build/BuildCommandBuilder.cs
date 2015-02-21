using System.IO;
using OmniSharp.Solution;
using OmniSharp.Configuration;

namespace OmniSharp.Build
{
    public class BuildCommandBuilder
    {
        private readonly ISolution _solution;
        private readonly OmniSharpConfiguration _config;

        public BuildCommandBuilder(
            ISolution solution,
            OmniSharpConfiguration config)
        {
            _solution = solution;
            _config = config;
        }

        public string Executable
        {
            get
            {
                return PlatformService.IsUnix
                    ? "xbuild"
                    : Path.Combine(
                    _config.MSBuildPath ?? System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
                    "Msbuild.exe");
            }
        }

        public string Arguments
        {
            get { return (PlatformService.IsUnix ? "" : "/m ") + "/nologo /v:q /property:GenerateFullPaths=true \"" + _solution.FileName + "\""; }
        }

        public BuildTargetResponse BuildCommand(BuildTargetRequest req)
        {
            return new BuildTargetResponse { Command = this.Executable.ApplyPathReplacementsForClient() + " " + this.Arguments + " /target:" + req.Type.ToString() + " " + "/p:Configuration=" + req.Configuration.ToString() };
        }
    }
}
