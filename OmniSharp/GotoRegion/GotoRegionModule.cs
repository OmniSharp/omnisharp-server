using Nancy;
using Nancy.ModelBinding;

namespace OmniSharp.GotoRegion {
    public class GotoRegionAsFlatModule : NancyModule {
        public GotoRegionAsFlatModule
            (GotoRegionHandler handler) {

            Post["/gotoregion"] = x =>
            {
                var req = this.Bind<Common.Request>();
                var members = handler.GetFileRegions(req);
                return Response.AsJson(members);
            };
        }
        #region TestRegion

        #endregion TestRegion

        #region TestRegion22222

        #endregion TestRegion22222
    }
}
