using OmniSharp.Common;

namespace OmniSharp.Build
{
    public class BuildTargetRequest : Request
    {
        public BuildType Type { get; set; }

        public enum BuildType
        {
            Build,
            Rebuild,
            Clean
        }
    }

}
