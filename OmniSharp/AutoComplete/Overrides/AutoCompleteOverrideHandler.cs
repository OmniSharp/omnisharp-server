using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.Solution;
using OmniSharp.AutoComplete;
using OmniSharp.Parser;

namespace OmniSharp.AutoComplete.Overrides {
    public class AutoCompleteOverrideHandler {

        /// <summary>
        ///   Returns the available overridable members in the given
        ///   request.
        /// </summary>
        public IEnumerable<AutoCompleteOverrideResponse> GetOverrideTargets
            ( AutoCompleteRequest request
            , BufferParser        parser) {
            var completionContext = new AutoCompleteBufferContext
                (request, parser);

            var currentType = completionContext.ParsedContent
                .UnresolvedFile.GetInnermostTypeDefinition
                    (completionContext.TextLocation);

            var overrideTargets = this.GetOverridableMethods(currentType)
                .Concat(this.GetOverridableProperties(currentType))
                .Concat(this.GetOverridableEvents(currentType))
                // TODO should we remove duplicates?
                .ToArray();

            return overrideTargets;
        }

        public IEnumerable<AutoCompleteOverrideResponse> GetOverridableMethods
            (IUnresolvedTypeDefinition type) {

            var overridableMethods = type.Methods
                .Where(method => method.IsOverridable)
                .Select(m => new AutoCompleteOverrideResponse
                        ( m
                        , descriptionText : "TODO descriptionText"
                        , completionText  : "TODO completionText"));

            return overridableMethods;
        }

        public IEnumerable<AutoCompleteOverrideResponse> GetOverridableProperties
            (IUnresolvedTypeDefinition type) {

            var overridableProperties = type.Properties
                .Where(property => property.IsOverridable)
                .Select(p => new AutoCompleteOverrideResponse
                        ( p
                        , descriptionText : "TODO descriptionText"
                        , completionText  : "TODO completionText"));

            return overridableProperties;
        }

        public IEnumerable<AutoCompleteOverrideResponse> GetOverridableEvents
            (IUnresolvedTypeDefinition type) {

            var overridableProperties = type.Events
                .Where(@event => @event.IsOverridable)
                .Select(e => new AutoCompleteOverrideResponse
                        ( e
                        , descriptionText : "TODO descriptionText"
                        , completionText  : "TODO completionText"));

            return overridableProperties;
        }
    }
}
