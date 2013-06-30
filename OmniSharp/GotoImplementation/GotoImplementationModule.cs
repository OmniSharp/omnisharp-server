using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using OmniSharp.Common;

namespace OmniSharp.GotoImplementation
{
    public class GotoImplementationModule : NancyModule
    {
        public GotoImplementationModule(GotoImplementationHandler handler)
        {
            Post["/findimplementations"] = x =>
                {
                    var req = this.Bind<GotoImplementationRequest>();
                    IEnumerable<QuickFix> res =
                        handler.FindDerivedMembersAsQuickFixes(req);
                    return Response.AsJson(res);
                };
        }
    }
}
