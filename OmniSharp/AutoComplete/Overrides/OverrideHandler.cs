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
            (Request request) {
            var overrideContext = new OverrideContext(request, this._parser);

            return overrideContext.OverrideTargets;
        }

        /// <summary>
        ///   Takes an editing context. Inserts an override
        ///   declaration of the chosen member in the context. Returns
        ///   the new context.
        /// </summary>
        /// <remarks>
        ///   The text editor cursor stays in the same line and column
        ///   it originally was.
        /// </remarks>
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

                    return memberSignature == request.OverrideTargetName;});

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
                    .ResolveContext.CurrentTypeDefinition
                , request);

            return new RunOverrideTargetResponse
                ( fileName : request.FileName
                , buffer   : newEditorContents.Text
                , line     : request.Line
                , column   : request.Column);

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
            , ITypeDefinition      currentType
            , Request              request) {

            var memberDeclaration = builder.ConvertEntity(memberToOverride);

            // Add override flag
            memberDeclaration.Modifiers |= Modifiers.Override;
            // Remove virtual flag
            memberDeclaration.Modifiers &= ~ Modifiers.Virtual;

            // TODO make indentation beautiful. This implementation
            // does not take the surrounding level of indentation into
            // account at all. The user has to manually indent the
            // overriding member.
            script.CurrentDocument.Insert
                ( offset: script.CurrentDocument.GetOffset
                  (new TextLocation
                   (line: request.Line, column: request.Column))
                , text: memberDeclaration
                  .GetText(FormattingOptionsFactory.CreateEmpty()));

            return script.CurrentDocument;
        }

    }
}
