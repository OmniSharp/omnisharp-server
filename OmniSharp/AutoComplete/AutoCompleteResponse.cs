namespace OmniSharp.AutoComplete
{
    public class AutoCompleteResponse
    {
        public AutoCompleteResponse() { } // for deserialisation

        public AutoCompleteResponse(CompletionData d)
        {
            DisplayText = d.DisplayText;
            CompletionText = d.CompletionText;
            Description = d.Description;
            RequiredNamespaceImport = d.RequiredNamespaceImport;
            MethodHeader = d.MethodHeader;
            ReturnType = d.ReturnType;

        }

        public string CompletionText { get; set; }
        public string Description { get; set; }
        public string DisplayText { get; set; }
        public string RequiredNamespaceImport { get; set; }
        public string MethodHeader { get; set; }
        public string ReturnType { get; set; }
    }
}
