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

        /// <remarks>
        ///   This will not be sent over the network. It must be
        ///   accessed in a public property in a derived class. Then
        ///   the public property will be sent.
        /// </remarks>
        protected IEnumerable<QuickFix> QuickFixes { get; set; }
    }
}
