using ICSharpCode.NRefactory.Editor;
using OmniSharp.GotoImplementation;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.Solution;

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

        /// <summary>
        ///   Initialize a QuickFix pointing to the first line of the
        ///   given region in the given file.
        /// </summary>
        public static QuickFix ForFirstLineInRegion
            (DomRegion region, CSharpFile file) {

            return QuickFix.ForFirstLineInRegion
                (region, file.Document);
        }

        public static QuickFix ForFirstLineInRegion
            (DomRegion region, IDocument document) {
            return new QuickFix
                { FileName = region.FileName
                , Line     = region.BeginLine
                , Column   = region.BeginColumn

                // Note that we could display an arbitrary amount of
                // context to the user: ranging from one line to tens,
                // hundreds..
                , Text = document.GetText
                    ( offset: document.GetOffset(region.Begin)
                    , length: document.GetLineByNumber
                                (region.BeginLine).Length)
                    .Trim()};
        }
    }
}
