using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.AutoComplete.Overrides {
    public class GetOverrideTargetsModule : NancyModule {

        public GetOverrideTargetsModule
            (AutoCompleteOverrideHandler overrideHandler) {

            Post["/getoverridetargets"] = x =>
                {
                    var req = this.Bind<AutoCompleteRequest>();
                    var completions = overrideHandler.GetOverrideTargets(req);
                    return Response.AsJson(completions);
                };
        }

    }
}
