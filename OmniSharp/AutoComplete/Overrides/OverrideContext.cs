using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.AutoComplete;
using OmniSharp.AutoComplete.Overrides;
using OmniSharp.Common;
using OmniSharp.Parser;

namespace Omnisharp.AutoComplete.Overrides {

    /// <summary>
    ///   Represents a context where some base class members are going
    ///   to be overridden.
    /// </summary>
    public class OverrideContext {

        public OverrideContext
            (AutoCompleteRequest request, BufferParser parser) {

            this.BufferParser = parser;
            this.CompletionContext = new AutoCompleteBufferContext
                (request, this.BufferParser);

            this.CurrentType = this.CompletionContext.ParsedContent
                .UnresolvedFile.GetInnermostTypeDefinition
                    (this.CompletionContext.TextLocation)
                .Resolve(this.CompletionContext.ResolveContext);

            this.OverrideTargets =
                GetOverridableMembers()
                // TODO should we remove duplicates?
                .Select(m => new GetOverrideTargetsResponse(m))
                .ToArray();
        }

        /// <summary>
        ///   The type currently under the cursor in this context.
        /// </summary>
        public IType CurrentType {get; set;}
        public IEnumerable<GetOverrideTargetsResponse> OverrideTargets {get; set;}
        public AutoCompleteBufferContext CompletionContext {get; set;}

        public BufferParser BufferParser {get; set;}

        public IEnumerable<IMember> GetOverridableMembers() {
            return this.CurrentType
                .GetMembers(m => m.IsVirtual && m.IsOverridable);
        }
    }

}
