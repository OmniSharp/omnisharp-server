using OmniSharp.GotoImplementation;

namespace OmniSharp.Common
{
    public class QuickFix
    {
        public string FileName { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }

        public Location ConvertToLocation() {
            return new Location()
                { FileName = this.FileName
                , Line     = this.Line
                , Column   = this.Column};
        }
    }
}
