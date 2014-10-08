using System.Collections.Generic;
using System.Linq;
using OmniSharp.CodeIssues;
using OmniSharp.Common;
using OmniSharp.SemanticErrors;

namespace OmniSharp.SyntaxErrors
{
    public class CodeCheckHandler
    {
        readonly SyntaxErrorsHandler _syntaxErrorsHandler;
        readonly CodeIssuesHandler _codeIssuesHandler;
        readonly SemanticErrorsHandler _semanticErrorsHandler;

        public CodeCheckHandler(SyntaxErrorsHandler syntaxErrorsHandler, CodeIssuesHandler codeIssuesHandler, SemanticErrorsHandler semanticErrorsHandler)
        {
            _semanticErrorsHandler = semanticErrorsHandler;
            _codeIssuesHandler = codeIssuesHandler;
            _syntaxErrorsHandler = syntaxErrorsHandler;
        }

        public IEnumerable<QuickFix> CodeCheck(Request request)
        {
            var errors = new List<QuickFix>();

            var syntaxErrors =
                _syntaxErrorsHandler.FindSyntaxErrors(request)
                    .Errors.Select(
                        x => new QuickFix {Column = x.Column, FileName = x.FileName, Line = x.Line, Text = x.Message});
            errors.AddRange(syntaxErrors);

            var semanticErrors =
                _semanticErrorsHandler.FindSemanticErrors(request)
                    .Errors.Select(
                        x => new QuickFix {Column = x.Column, FileName = x.FileName, Line = x.Line, Text = x.Message});
            errors.AddRange(semanticErrors);

            var codeErrors = _codeIssuesHandler.GetCodeIssues(request).QuickFixes;
            errors.AddRange(codeErrors);

            return errors;
        }
    }
}
