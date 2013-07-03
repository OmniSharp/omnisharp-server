using System.Collections.Generic;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using OmniSharp.AutoComplete.Overrides;

namespace OmniSharp.AutoComplete.Overrides {

    public class AutoCompleteOverrideResponse : AutoCompleteResponse {

        public AutoCompleteOverrideResponse() : base() {}

        public AutoCompleteOverrideResponse
            ( IUnresolvedMethod m
            , string            descriptionText
            , string            completionText) {

            this.DisplayText    = createDisplayText(m);
            this.Description    = descriptionText;
            this.CompletionText = completionText;

            this.OverrideTypeKind = OverrideTypeKind.Method;
        }

        public AutoCompleteOverrideResponse
            ( IUnresolvedProperty p
            , string              descriptionText
            , string              completionText) {

            this.DisplayText    = createDisplayText(p);
            this.Description    = descriptionText;
            this.CompletionText = completionText;

            this.OverrideTypeKind = OverrideTypeKind.Property;
        }

        public AutoCompleteOverrideResponse
            ( IUnresolvedEvent e
            , string           descriptionText
            , string           completionText) {

            this.DisplayText    = createDisplayText(e);
            this.Description    = descriptionText;
            this.CompletionText = completionText;

            this.OverrideTypeKind = OverrideTypeKind.Event;
        }

        /// <summary>
        ///   Gets or sets a value indicating what kind of overridable
        ///   member this response describes.
        /// </summary>
        /// <remarks>
        ///   Perhaps the users of this API will find a good use for
        ///   this. Better not lose this information in any case.
        /// </remarks>
        public OverrideTypeKind OverrideTypeKind {get; set;}

        public static string createDisplayText(IUnresolvedMethod m) {
            return string.Format
                ( "{0} {1} {2}({3})"
                , m.Accessibility         // e.g. public, protected
                , m.ReturnType.ToString() // e.g. void or object
                , m.Name
                , m.Parameters);
        }

        public static string createDisplayText(IUnresolvedProperty p) {
            return string.Format
                ( @"{0} {1} {2}\{{3}{4}\}"
                , p.Accessibility         // e.g. public, protected
                , p.ReturnType.ToString() // e.g. void or object
                , p.Name
                , p.CanGet ? "get;" : ""
                , p.CanSet ? "set;" : "");
        }

        public static string createDisplayText(IUnresolvedEvent e) {
            throw new NotImplementedException();
        }

    }

}
