using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;

namespace OmniSharp.Configuration
{
    public class OmniSharpConfiguration
    {
        public OmniSharpConfiguration()
        {
            PathReplacements = new List<PathReplacement>();
            IgnoredCodeIssues = new List<string>();
            TextEditorOptions = new TextEditorOptions ();
            CSharpFormattingOptions = FormattingOptionsFactory.CreateAllman();
        }

        public IEnumerable<PathReplacement> PathReplacements { get; set; }
        public IEnumerable<string> IgnoredCodeIssues { get; set; }
        public TextEditorOptions TextEditorOptions { get; set; }
        public TestCommands TestCommands { get; set; }
        public CSharpFormattingOptions CSharpFormattingOptions { get; set; }
        public bool? UseCygpath { get; set; }
        public PathMode? ClientPathMode { get; set; }
        public PathMode? ServerPathMode { get; set; }

        public class PathReplacement
        {
            public string From { get; set; }
            public string To { get; set; }
        }
    }
}
