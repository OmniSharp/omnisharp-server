using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.ContextInformation
{
    public class GetContextModule : NancyModule
    {
        public GetContextModule(GetContextHandler handler)
        {
            Post["/getcontext"] = x =>
                {
                    var request = this.Bind<Common.Request>();
                    return Response.AsJson(handler.GetContextResponse(request));
                };
        }
    }
}