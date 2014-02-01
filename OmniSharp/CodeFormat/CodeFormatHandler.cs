using System;
using ICSharpCode.NRefactory.CSharp;
namespace OmniSharp.CodeFormat
{
    public class CodeFormatHandler
    {
        public CodeFormatResponse Format(CodeFormatRequest request)
        {
            var options = new TextEditorOptions();
            options.EolMarker = Environment.NewLine;
            options.WrapLineLength = 80;
            options.TabsToSpaces = request.ExpandTab;
            var policy = FormattingOptionsFactory.CreateAllman();
            policy.BlankLinesAfterUsings = 1;
            policy.BlankLinesBetweenTypes = 1;
            policy.BlankLinesBetweenMembers = 1;
            var formatter = new CSharpFormatter(policy, options);
            formatter.FormattingMode = FormattingMode.Intrusive;
            var output = formatter.Format(request.Buffer);
            return new CodeFormatResponse(output);
        }
    }
}
