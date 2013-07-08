using Nancy;
using OmniSharp.Solution;

namespace OmniSharp.PokeModule
{
    public class PokeModule : NancyModule
    {
        public PokeModule(ISolution solution)
        {
            Post["/poke"] = x =>
                {
                    return Response.AsJson(true);
                };
        }
    }
}
