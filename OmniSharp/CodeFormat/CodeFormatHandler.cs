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
			var output = new CSharpFormatter (policy, options).Format (request.Buffer);
			return new CodeFormatResponse(output);
        }
    }
}
