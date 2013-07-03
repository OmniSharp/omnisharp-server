using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.AutoComplete.Overrides {
    public class AutoCompleteOverrideModule : NancyModule {

        public AutoCompleteOverrideModule
            (AutoCompleteOverrideHandler overrideHandler) {

            Post["/autocompleteoverrides"] = x =>
                {
                    var req = this.Bind<AutoCompleteRequest>();
                    var completions = overrideHandler.GetOverrideTargets(req);
                    return Response.AsJson(completions);
                };
        }

    }
}
