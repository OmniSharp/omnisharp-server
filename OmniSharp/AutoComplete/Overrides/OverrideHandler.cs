using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.AutoComplete;
using OmniSharp.Common;
using OmniSharp.Parser;
using OmniSharp.Refactoring;
using OmniSharp.Solution;
using Omnisharp.AutoComplete.Overrides;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace OmniSharp.AutoComplete.Overrides {
    public class OverrideHandler {

        private readonly BufferParser _parser;

        public OverrideHandler(BufferParser parser) {
            _parser = parser;
        }

        /// <summary>
        ///   Returns the available overridable members in the given
        ///   request.
        /// </summary>
        public IEnumerable<GetOverrideTargetsResponse> GetOverrideTargets
            (AutoCompleteRequest request) {
            var overrideContext = new OverrideContext(request, this._parser);

            return overrideContext.OverrideTargets;
        }

        /// <summary>
        ///   Takes an editing context. Inserts an override
        ///   declaration of the chosen member in the context. Returns
        ///   the new context.
        /// </summary>
        public RunOverrideTargetResponse RunOverrideTarget
            (RunOverrideTargetRequest request) {
            var overrideContext = new OverrideContext(request, this._parser);
            var refactoringContext = OmniSharpRefactoringContext.GetContext
                (overrideContext.BufferParser, request);

            var memberToOverride = overrideContext.GetOverridableMembers()
                .First(ot => {
                    var memberSignature =
                     GetOverrideTargetsResponse.GetOverrideTargetName
                      (ot, overrideContext.CompletionContext.ResolveContext);

                    return memberSignature == request.WordToComplete;});

            var script = new OmniSharpScript(refactoringContext);
            var builder = new TypeSystemAstBuilder
                (new CSharpResolver
                 (overrideContext.CompletionContext.ResolveContext))
                 // Will generate a "throw new
                 // NotImplementedException();" statement in the
                 // bodies
                 {GenerateBody = true};


            var newEditorContents = runOverrideTargetWorker
                ( memberToOverride
                , script
                , builder
                , overrideContext.CompletionContext
                    .ResolveContext.CurrentTypeDefinition);

            // Set cursor to a reasonable location!
            //
            // Without this way the cursor is set to a location that
            // might be offset by the amount of lines that are
            // inserted in the overriding declaration.
            var newOffset = script.GetCurrentOffset
                (new TextLocation
                 (line: request.Line, column: request.Column));

            var newPosition = script.CurrentDocument.GetLocation(newOffset);

            return new RunOverrideTargetResponse
                ( fileName : request.FileName
                , buffer   : newEditorContents.Text
                , line     : newPosition.Line
                , column   : newPosition.Column);

        }

        /// <summary>
        ///   Creates an overriding declaration for the given IMember,
        ///   which must be IMethod, IProperty or IEvent. Inserts this
        ///   declaration with the given script.
        /// </summary>
        /// <remarks>
        ///   Alters the given script. Returns its CurrentDocument
        ///   property. Alters the given memberToOverride, adding
        ///   Modifiers.Override to its Modifiers as well as removing
        ///   Modifiers.Virtual.
        /// </remarks>
        /// <param name="currentType">
        ///   The type currently under cursor. This is expected to be
        ///   the class the member is to be overridden in.
        /// </param>
        IDocument runOverrideTargetWorker
            ( IMember              memberToOverride
            , OmniSharpScript      script
            , TypeSystemAstBuilder builder
            , ITypeDefinition      currentType) {

            var memberDeclaration = builder.ConvertEntity(memberToOverride);

            // Add override flag
            memberDeclaration.Modifiers |= Modifiers.Override;
            // Remove virtual flag
            memberDeclaration.Modifiers &= ~ Modifiers.Virtual;

            // TODO this inserts the overriding member to the very top
            // of the class and does not indent it properly
            script.InsertWithCursor
                ( operation  : "THIS IS AN OPERATION" // what shoud this be?
                , parentType : currentType
                , nodes      : memberDeclaration) ;

            return script.CurrentDocument;
        }

    }
}
