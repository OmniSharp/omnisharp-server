using Nancy;
using OmniSharp.Solution;

namespace OmniSharp.Build
{
    public class BuildCommandModule : NancyModule
    {
        public BuildCommandModule(BuildCommandBuilder commandBuilder)
        {
            Post["/buildcommand"] = x =>
                Response.AsText(commandBuilder.Executable.ApplyPathReplacementsForClient() + " " + commandBuilder.Arguments);
        }
    }
}
