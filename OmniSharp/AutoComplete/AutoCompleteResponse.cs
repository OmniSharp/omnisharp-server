using ICSharpCode.NRefactory.Completion;

namespace OmniSharp.AutoComplete
{
    public class AutoCompleteResponse
    {
        public AutoCompleteResponse() { } // for deserialisation

        public AutoCompleteResponse(ICompletionData d)
        {
            DisplayText = d.DisplayText;
            CompletionText = d.CompletionText;
            Description = d.Description;
            RequiredNamespaceImport = d is CompletionData ? ((CompletionData)d).RequiredNamespaceImport : (string)null;
        }

        public string CompletionText { get; set; }
        public string Description { get; set; }
        public string DisplayText { get; set; }
        public string RequiredNamespaceImport { get; set; }
    }
}
