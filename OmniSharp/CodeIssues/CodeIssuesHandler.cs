using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using OmniSharp.CodeActions;
using OmniSharp.Common;
using OmniSharp.Parser;
using OmniSharp.Refactoring;
using OmniSharp.Solution;

namespace OmniSharp.CodeIssues
{
    public class CodeIssuesHandler
    {
        private readonly ISolution _solution;
        private readonly BufferParser _bufferParser;

        public CodeIssuesHandler(ISolution solution, BufferParser bufferParser)
        {
            _solution = solution;
            _bufferParser = bufferParser;
        }

        public QuickFixResponse GetCodeIssues(Request req)
        {
            var doc = _solution.GetFile(req.FileName).Document;
            var actions = GetContextualCodeActions(req);
            return new QuickFixResponse(actions.Select(a => new QuickFix
                {
                    Column = a.Start.Column,
                    Line = a.Start.Line,
                    FileName = "",
                    Text = GetLine(doc, a) + "(" + a.Description.Replace("'", "''") + ")"
                }));
        }

        private string GetLine(StringBuilderDocument document, CodeIssue issue)
        {
            var text = document.GetText
                (offset: document.GetOffset(issue.Start.Line, column: 0)
                , length: document.GetLineByNumber
                            (issue.Start.Line).Length)
                .Trim();
            return text;
        }

        public RunCodeIssuesResponse RunCodeIssue(RunCodeActionRequest req)
        {
            var issues = GetContextualCodeActions(req).ToList();
            if(req.CodeAction > issues.Count)
                return new RunCodeIssuesResponse();

            CodeIssue issue = issues[req.CodeAction];
            var context = OmniSharpRefactoringContext.GetContext(_bufferParser, req);
            
            using (var script = new OmniSharpScript(context))
            {
                issue.Actions.FirstOrDefault().Run(script);
            }

            return new RunCodeIssuesResponse {Text = context.Document.Text};
        }

        private IEnumerable<CodeIssue> GetContextualCodeActions(Request req)
        {
            var refactoringContext = OmniSharpRefactoringContext.GetContext(_bufferParser, req);

            var actions = new List<CodeIssue>();
            var providers = new CodeIssueProviders().GetProviders();
            foreach (var provider in providers)
            {
                try
                {
                    var codeIssues = provider.GetIssues(refactoringContext);
                    actions.AddRange(codeIssues);
                }
                catch (Exception)
                {
                }
                
            }
            return actions;
        }
    }
}