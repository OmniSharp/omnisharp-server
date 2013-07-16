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
    }
}
