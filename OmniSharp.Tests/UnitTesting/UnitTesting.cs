using System;
using OmniSharp.Common;
using OmniSharp.ContextInformation;
using OmniSharp.Parser;

namespace OmniSharp.Tests.UnitTesting
{
    public static class CreateContextLookup
    {
        public static GetContextResponse GetContextInformation(this string editorText)
        {
            int cursorOffset = editorText.IndexOf("$", StringComparison.Ordinal);
            var cursorPosition = TestHelpers.GetLineAndColumnFromIndex(editorText, cursorOffset);
            editorText = editorText.Replace("$", "");

            var solution = new FakeSolution();
            var project = new FakeProject();
            project.AddFile(editorText);
            solution.Projects.Add(project);

            var handler = new GetContextHandler(solution, new BufferParser(solution));
            var request = new Request
            {
                Buffer = editorText,
                FileName = "myfile",
                Line = cursorPosition.Line,
                Column = cursorPosition.Column,
            };

            return handler.GetContextResponse(request);
        }
    }
}
