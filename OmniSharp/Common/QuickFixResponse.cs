using System.Collections.Generic;
using OmniSharp.Common;

namespace OmniSharp.Common {

    /// <summary>
    ///   Base class for QuickFix-centered responses.
    /// </summary>
    public abstract class QuickFixResponse {
        public QuickFixResponse() {}

        public QuickFixResponse(IEnumerable<QuickFix> quickFixes) {
            this.QuickFixes = quickFixes;
        }

        protected IEnumerable<QuickFix> QuickFixes { get; set; }
    }
}
