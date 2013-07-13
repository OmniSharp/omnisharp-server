using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.AutoComplete.Overrides {
    public class RunOverrideTargetModule : NancyModule {

        public RunOverrideTargetModule
            (OverrideHandler overrideHandler) {

            Post["/runoverridetarget"] = x =>
                {
                    var req = this.Bind<RunOverrideTargetRequest>();
                    var response = overrideHandler.RunOverrideTarget(req);
                    return Response.AsJson(response);
                };
        }
    }
}
