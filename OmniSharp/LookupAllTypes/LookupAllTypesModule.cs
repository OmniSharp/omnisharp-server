using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.LookupAllTypes
{
    public class LookupAllTypesModule : NancyModule
    {
        public LookupAllTypesModule(LookupAllTypesHandler handler)
        {
            Post["/lookupalltypes"] = x =>
            {
                var req = this.Bind<LookupAllTypesRequest>();
                var res = handler.GetLookupAllTypesResponse(req);
                return Response.AsJson(res);
            };
        }
    }
}