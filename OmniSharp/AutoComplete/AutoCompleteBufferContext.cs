using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.Editor;
using OmniSharp.Parser;

namespace OmniSharp.AutoComplete {

    /// <summary>
    ///   Represents a buffer state in an editor and that state's
    ///   context in relation to the current solution. Can be used to
    ///   provide different context dependent completions to the user.
    /// </summary>
    public class AutoCompleteBufferContext {
        public AutoCompleteBufferContext
            ( AutoCompleteRequest request
            , BufferParser parser) {
            this.AutoCompleteRequest = request;
            this.BufferParser = parser;

            this.Document = new ReadOnlyDocument(request.Buffer ?? "");
            this.TextLocation = new TextLocation
                ( request.Line
                , request.Column - request.WordToComplete.Length);

            int cursorPosition = this.Document.GetOffset(this.TextLocation);
            //Ensure cursorPosition only equals 0 when editorText is empty, so line 1,column 1
            //completion will work correctly.
            cursorPosition = Math.Max(cursorPosition, 1);
            cursorPosition = Math.Min(cursorPosition, request.Buffer.Length);
            this.CursorPosition = cursorPosition;

            this.ParsedContent = this.BufferParser.ParsedContent(request.Buffer, request.FileName);
            this.ResolveContext = this.ParsedContent.UnresolvedFile.GetTypeResolveContext(this.ParsedContent.Compilation, this.TextLocation);

        }

        public AutoCompleteRequest      AutoCompleteRequest {get; set;}
        public ReadOnlyDocument         Document            {get; set;}
        public int                      CursorPosition      {get; set;}
        public TextLocation             TextLocation        {get; set;}
        public BufferParser             BufferParser        {get; set;}
        public ParsedResult             ParsedContent       {get; set;}
        public CSharpTypeResolveContext ResolveContext      {get; set;}

        public AstNode NodeCurrentlyUnderCursor {
            get {
                return this.ParsedContent.SyntaxTree.GetNodeAt(this.TextLocation);
            }
        }

    }

}
