using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.Solution;
using OmniSharp.AutoComplete;
using OmniSharp.Parser;

namespace OmniSharp.AutoComplete.Overrides {
    public class AutoCompleteOverrideHandler {

        private readonly BufferParser _parser;

        public AutoCompleteOverrideHandler(BufferParser parser) {
            _parser = parser;
        }

        /// <summary>
        ///   Returns the available overridable members in the given
        ///   request.
        /// </summary>
        public IEnumerable<GetAutoCompleteOverridesResponse> GetOverrideTargets
            (AutoCompleteRequest request) {
            var completionContext = new AutoCompleteBufferContext
                (request, this._parser);

            var currentType = completionContext.ParsedContent
                .UnresolvedFile.GetInnermostTypeDefinition
                    (completionContext.TextLocation)
                .Resolve(completionContext.ResolveContext);

            var overrideTargets = currentType.GetMembers
                (m => m.IsVirtual && m.IsOverridable)
                // TODO should we remove duplicates?
                .Select(m => new GetAutoCompleteOverridesResponse(m))
                .ToArray();

            return overrideTargets;
        }

    }
}
