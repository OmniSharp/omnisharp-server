using OmniSharp.Common;

namespace OmniSharp.CodeFormat
{
    public class CodeFormatRequest : Request
    {
        private bool _expandTab = false;

        public bool ExpandTab
        {
            get { return _expandTab; }
            set { _expandTab = value; }
        }

        public bool TabsToSpaces { get; set; } = true;

        public int TabSize { get; set; } = 4;
    }
}
