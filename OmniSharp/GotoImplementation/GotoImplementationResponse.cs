using System.Collections.Generic;
using OmniSharp.Common;

namespace OmniSharp.GotoImplementation
{
    public class GotoImplementationResponse : QuickFixResponse {
        public GotoImplementationResponse() : base() {}

        public GotoImplementationResponse
            (IEnumerable<QuickFix> quickFixes) : base(quickFixes) {}

        public IEnumerable<QuickFix> Locations {
            get {return base.QuickFixes;}
            set {base.QuickFixes = value;}
        }
    }
}
