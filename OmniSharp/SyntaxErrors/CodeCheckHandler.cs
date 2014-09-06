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
            errors.AddRange(_syntaxErrorsHandler.FindSyntaxErrors(request));
            if (errors.Any())
            {
                return errors;
            }
            errors.AddRange(_semanticErrorsHandler.FindSemanticErrors(request));
            if (errors.Any())
            {
                return errors;
            }
            errors.AddRange(_codeIssuesHandler.GetCodeIssues(request));
			return errors;
        }
    }
}
