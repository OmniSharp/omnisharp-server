using System.Collections.Generic;

namespace OmniSharp.Configuration
{
    public class OmniSharpConfiguration
    {
        public OmniSharpConfiguration()
        {
            PathReplacements = new List<PathReplacement>();
        }
        public IEnumerable<PathReplacement> PathReplacements { get; set; }
        public TestCommands TestCommands { get; set; }
        public bool? UseCygpath { get; set; }
        public PathMode? ClientPathMode { get; set; }
        public PathMode? ServerPathMode { get; set; }
    }
}