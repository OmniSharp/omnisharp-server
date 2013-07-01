using System.Collections.Generic;
using OmniSharp.Common;

namespace OmniSharp.FindUsages
{
    public class FindUsagesResponse : QuickFixResponse
    {
        public FindUsagesResponse() : base() {}

        public FindUsagesResponse(IEnumerable<QuickFix> quickFixes)
            : base(quickFixes) {}

        public IEnumerable<QuickFix> Usages {
            get {return base.QuickFixes;}
            set {base.QuickFixes = value;}
        }
    }
}
