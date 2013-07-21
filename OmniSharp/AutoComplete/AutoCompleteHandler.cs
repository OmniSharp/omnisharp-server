using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.Editor;
using OmniSharp.Parser;

namespace OmniSharp.AutoComplete
{
    public class AutoCompleteHandler

    {
        private readonly BufferParser _parser;
        private readonly Logger _logger;

        public AutoCompleteHandler(BufferParser parser, Logger logger)
        {
            _parser = parser;
            _logger = logger;
        }

        public IEnumerable<ICompletionData> CreateProvider(AutoCompleteRequest request)
        {
            request.Column = request.Column - request.WordToComplete.Length;

            var completionContext = new BufferContext
                (request, _parser);

            var partialWord = request.WordToComplete;

            ICompletionContextProvider contextProvider = new DefaultCompletionContextProvider
                (completionContext.Document, completionContext.ParsedContent.UnresolvedFile);
            var engine = new CSharpCompletionEngine
                ( completionContext.Document
                , contextProvider
                , new CompletionDataFactory(partialWord)
                , completionContext.ParsedContent.ProjectContent
                , completionContext.ResolveContext)
                {
                    EolMarker = Environment.NewLine
                };

            _logger.Debug("Getting Completion Data");

            IEnumerable<ICompletionData> data = engine.GetCompletionData(completionContext.CursorPosition, true);
            _logger.Debug("Got Completion Data");
            return data.Where(d => d != null && d.CompletionText.IsValidCompletionFor(partialWord))
                       .FlattenOverloads()
                       .RemoveDupes()
					   .OrderByDescending(d => d.CompletionText.IsValidCompletionStartsWithExactCase(partialWord))
					   .ThenByDescending(d => d.CompletionText.IsValidCompletionStartsWithIgnoreCase(partialWord))
					   .ThenByDescending(d => d.CompletionText.IsCamelCaseMatch(partialWord))
					   .ThenByDescending(d => d.CompletionText.IsSubsequenceMatch(partialWord))
                       .ThenBy(d => d.CompletionText);
        }
    }
}
