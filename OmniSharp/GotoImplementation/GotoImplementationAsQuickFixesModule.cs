using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using OmniSharp.Common;

namespace OmniSharp.GotoImplementation
{
    public class GotoImplementationAsQuickFixesModule : NancyModule
    {
        public GotoImplementationAsQuickFixesModule
            (GotoImplementationHandler handler) {
            Post["/findimplementationsasquickfixes"] = x =>
                {
                    var req = this.Bind<GotoImplementationRequest>();
                    IEnumerable<QuickFix> res =
                        handler.FindDerivedMembersAsQuickFixes(req);
                    return Response.AsJson(res);
                };
        }
    }
}
