using System.Collections.Generic;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using OmniSharp.AutoComplete.Overrides;

// Without this all OverrideTypeKind references will default to the
// class member of the same name
using kind = OmniSharp.AutoComplete.Overrides.OverrideTypeKind;

namespace OmniSharp.AutoComplete.Overrides {

    public class AutoCompleteOverrideResponse : AutoCompleteResponse {

        public AutoCompleteOverrideResponse() : base() {}

        public AutoCompleteOverrideResponse
            ( IMethod m
            , string            descriptionText
            , string            completionText) {

            this.DisplayText    = createDisplayText(m);
            this.Description    = descriptionText;
            this.CompletionText = completionText;

            this.OverrideTypeKind = kind.Method.ToString();
        }

        public AutoCompleteOverrideResponse
            ( IProperty p
            , string              descriptionText
            , string              completionText) {

            this.DisplayText    = createDisplayText(p);
            this.Description    = descriptionText;
            this.CompletionText = completionText;

            this.OverrideTypeKind = kind.Property.ToString();
        }

        public AutoCompleteOverrideResponse
            ( IEvent e
            , string           descriptionText
            , string           completionText) {

            this.DisplayText    = createDisplayText(e);
            this.Description    = descriptionText;
            this.CompletionText = completionText;

            this.OverrideTypeKind = kind.Event.ToString();
        }

        private OverrideTypeKind _overrideTypeKind;

        /// <summary>
        ///   Gets or sets a value indicating what kind of overridable
        ///   member this response describes.
        /// </summary>
        /// <remarks>
        ///   Perhaps the users of this API will find a good use for
        ///   this. Better not lose this information in any case.
        /// </remarks>
        public string OverrideTypeKind {
            get {
                return _overrideTypeKind.ToString();
            }
            set {
                _overrideTypeKind = (OverrideTypeKind)
                    Enum.Parse(typeof(OverrideTypeKind), value);
            }
        }

        public static string createDisplayText(IMethod m) {
            return string.Format
                ( "{0} {1} {2}({3})"
                , m.Accessibility         // e.g. public, protected
                , m.ReturnType.ToString() // e.g. void or object
                , m.Name
                , m.Parameters);
        }

        public static string createDisplayText(IProperty p) {
            return string.Format
                ( @"{0} {1} {2}\{{3}{4}\}"
                , p.Accessibility         // e.g. public, protected
                , p.ReturnType.ToString() // e.g. void or object
                , p.Name
                , p.CanGet ? "get;" : ""
                , p.CanSet ? "set;" : "");
        }

        public static string createDisplayText(IEvent e) {
            throw new NotImplementedException();
        }

    }

}
