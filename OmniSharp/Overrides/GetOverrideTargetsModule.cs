using OmniSharp.Common;
using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.Overrides {
    public class GetOverrideTargetsModule : NancyModule {

        public GetOverrideTargetsModule
            (OverrideHandler overrideHandler) {

            Post["/getoverridetargets"] = x =>
                {
                    var req = this.Bind<OmniSharp.Common.Request>();
                    var completions = overrideHandler.GetOverrideTargets(req);
                    return Response.AsJson(completions);
                };
        }

    }
}
