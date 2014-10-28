using Nancy;
using OmniSharp.Solution;
using Nancy.ModelBinding;

namespace OmniSharp.Build
{
    public class BuildCommandModule : NancyModule
    {
        public BuildCommandModule(BuildCommandBuilder commandBuilder)
        {
            Post["/buildcommand"] = x =>
                Response.AsText(commandBuilder.Executable.ApplyPathReplacementsForClient() + " " + commandBuilder.Arguments);

            Post["/buildtarget"] = x =>
            {
                var req = this.Bind<BuildTargetRequest>();
                return commandBuilder.BuildCommand(req);
            };
        }
    }
}
