using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using OmniSharp.Common;
using OmniSharp.Solution;

namespace OmniSharp.SyntaxErrors
{
    public class SyntaxErrorsHandler
    {
        public SyntaxErrorsResponse FindSyntaxErrors(Request request)
        {
            var syntaxTree = new CSharpParser().Parse(request.Buffer, request.FileName);

            var filename = request.FileName.ApplyPathReplacementsForClient();

            var errors = syntaxTree.Errors.Select(error => new Error
                {
                    Message = error.Message.Replace("'", "''"),
                    Column = error.Region.BeginColumn,
                    Line = error.Region.BeginLine,
                    FileName = filename
                });

            return new SyntaxErrorsResponse {Errors = errors};
        }
    }
}
