using OmniSharp.Common;

namespace OmniSharp.AutoComplete
{
    public class AutoCompleteRequest : Request
    {
        private string _wordToComplete;
        public string WordToComplete {
            get {
                return _wordToComplete ?? "";
            }
            set {
                _wordToComplete = value;
            }
        }

        public AutoCompleteRequest()
        {
            WantDocumentationForEveryCompletionResult = false;
            WantImportableTypes = false;
            WantMethodHeader = true;
        }

        /// <summary>
        ///   Specifies whether to return the code documentation for
        ///   each and every returned autocomplete result.        
        /// </summary>
        public bool WantDocumentationForEveryCompletionResult { get; set; }

        /// <summary>
        ///   Specifies whether to return importable types. Defaults to
        ///   false. Can be turned off to get a small speed boost.
        /// </summary>
        public bool WantImportableTypes { get; set; }

        /// <summary>
        /// Returns a 'method header' for working with parameter templating.
        /// </summary>
        public bool WantMethodHeader { get; set; }
    }
}
