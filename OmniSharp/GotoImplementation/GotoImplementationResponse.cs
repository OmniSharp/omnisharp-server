using System.Collections.Generic;
using System.Linq;
using OmniSharp.Common;

namespace OmniSharp.GotoImplementation
{
    public class GotoImplementationResponse
    {
        public GotoImplementationResponse() {}

        public GotoImplementationResponse
            (IEnumerable<QuickFix> quickFixes) {

            this.Locations = quickFixes
                .Select(qf => qf.ConvertToLocation())
                .ToArray();
        }

        public IEnumerable<Location> Locations = new List<Location>();
    }
}