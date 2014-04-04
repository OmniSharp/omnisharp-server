using System.Collections.Generic;

namespace OmniSharp.Configuration
{
    public class OmniSharpConfiguration
    {
        public OmniSharpConfiguration()
        {
            PathReplacements = new List<PathReplacement>();
            IgnoredCodeIssues = new List<string>();
        }

        public IEnumerable<PathReplacement> PathReplacements { get; set; }
        public IEnumerable<string> IgnoredCodeIssues { get; set; }
        public TestCommands TestCommands { get; set; }
        public bool? UseCygpath { get; set; }
        public PathMode? ClientPathMode { get; set; }
        public PathMode? ServerPathMode { get; set; }
    }
}
