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
                    var res = handler.GetLookupAllTypesResponse(includeTypesWithoutSource:true);
                    return Response.AsJson(res);
                };
        }
    }
}