using System.Threading;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using OmniSharp.CodeActions;
using OmniSharp.Common;
using OmniSharp.Parser;

namespace OmniSharp.Refactoring
{
    public class OmniSharpRefactoringContext : RefactoringContext
    {
        private readonly IDocument _document;
        private readonly TextLocation _location;
        private readonly TextLocation _selectionStart;
        private readonly TextLocation _selectionEnd;

        public OmniSharpRefactoringContext(IDocument document, CSharpAstResolver resolver)
            : this(document, resolver, new TextLocation(1,1))
        {
        }

        private OmniSharpRefactoringContext(IDocument document, CSharpAstResolver resolver, TextLocation location) 
            : base(resolver, CancellationToken.None)
        {
            _document = document;
            _location = location;
        }

        private OmniSharpRefactoringContext(IDocument document, CSharpAstResolver resolver, TextLocation location, TextLocation selectionStart, TextLocation selectionEnd) 
            : base(resolver, CancellationToken.None)
        {
            _document = document;
            _location = location;
            _selectionStart = selectionStart;
            _selectionEnd = selectionEnd;
        }

        public static OmniSharpRefactoringContext GetContext(BufferParser bufferParser, Request request)
        {
            var q = bufferParser.ParsedContent(request.Buffer, request.FileName);
            var resolver = new CSharpAstResolver(q.Compilation, q.SyntaxTree, q.UnresolvedFile);
            var doc = new StringBuilderDocument(request.Buffer);
            var location = new TextLocation(request.Line, request.Column);
            OmniSharpRefactoringContext refactoringContext;
            if(request is CodeActionRequest)
            {
                var car = request as CodeActionRequest;
                if(car.SelectionStartColumn.HasValue)
                {
                    var startLocation
                        = new TextLocation(car.SelectionStartLine.Value, car.SelectionStartColumn.Value);
                    var endLocation
                        = new TextLocation(car.SelectionEndLine.Value, car.SelectionEndColumn.Value);

                    refactoringContext = new OmniSharpRefactoringContext(doc, resolver, location, startLocation, endLocation);
                }
                else
                {
                    refactoringContext = new OmniSharpRefactoringContext(doc, resolver, location);
                }
            }
            else
            {
                refactoringContext = new OmniSharpRefactoringContext(doc, resolver, location);
            }
            return refactoringContext;
        }

        public IDocument Document { get { return _document; } }

        public override int GetOffset(TextLocation location)
        {
            return _document.GetOffset(location);
        }
        
        public override TextLocation SelectionStart { get { return _selectionStart; } }
        public override TextLocation SelectionEnd { get { return _selectionEnd; } }

        public override IDocumentLine GetLineByOffset(int offset)
        {
            return _document.GetLineByOffset(offset);
        }

        public override TextLocation GetLocation(int offset)
        {
            return _location;
        }

        public override string GetText(int offset, int length)
        {
            return _document.GetText(offset, length);
        }

        public override string GetText(ISegment segment)
        {
            return _document.GetText(segment);
        }

        public override TextLocation Location
        {
            get { return _location; }
        }
    }
}
