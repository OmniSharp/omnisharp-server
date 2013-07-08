using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.AutoComplete;
using OmniSharp.Common;
using OmniSharp.Parser;
using OmniSharp.Refactoring;
using OmniSharp.Solution;
using Omnisharp.AutoComplete.Overrides;

namespace OmniSharp.AutoComplete.Overrides {
    public class OverrideHandler {

        private readonly BufferParser _parser;

        public OverrideHandler(BufferParser parser) {
            _parser = parser;
        }

        /// <summary>
        ///   Returns the available overridable members in the given
        ///   request.
        /// </summary>
        public IEnumerable<GetOverrideTargetsResponse> GetOverrideTargets
            (AutoCompleteRequest request) {
            var overrideContext = new OverrideContext(request, this._parser);

            return overrideContext.OverrideTargets;
        }

        /// <summary>
        ///   Takes an editing context. Inserts an override
        ///   declaration of the chosen member in the context. Returns
        ///   the new context.
        /// </summary>
        public RunOverrideTargetResponse RunOverrideTarget
            (RunOverrideTargetRequest request) {

            throw new NotImplementedException();
        }


    }
}
