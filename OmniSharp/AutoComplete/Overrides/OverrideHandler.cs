using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;
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

            // TODO
            // Try looking at NRefactory source to see whether
            // something in that library knows how to create an
            // EntityDeclaration (abstract, derives from
            // AstNode. These can probably be formatted back to text.)
            var memberToOverride =  overrideContext.GetOverridableMembers()
                .First(ot => ot.ToString() == request.WordToComplete);;

            Console.WriteLine("refactoringContext=" + refactoringContext);
            Console.WriteLine("memberToOverride=" + memberToOverride);

            var script = new OmniSharpScript(refactoringContext);
            var newEditorContents = runOverrideTargetWorker
                (memberToOverride, script);
            Console.WriteLine("newEditorContents=" + newEditorContents);

            // // From CreateMethodDeclarationAction
            // // TODO there are a bunch of promising looking static
            // //      methods in that class.
            // script.InsertWithCursor
            //     ( "THIS IS AN OPERATION"
            //     , ICSharpCode.NRefactory.CSharp.Refactoring.Script.InsertPosition.After
            //     , new[] {methodDeclaration});

            // return new RunOverrideTargetResponse
            //     ( fileName : script.CurrentDocument.FileName
            //     , buffer   : script.CurrentDocument.Text);

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
        ///   property.
        /// </remarks>
        IDocument runOverrideTargetWorker
            ( IMember         memberToOverride
            , OmniSharpScript script) {

            if (memberToOverride is IMethod) {
                var methodDeclaration = GetOverrideDeclarationForMethod
                    ((IMethod) memberToOverride);
                script.InsertWithCursor
                    ( "THIS IS AN OPERATION"
                    , Script.InsertPosition.After
                    , new[] {methodDeclaration});
            }
            else if (memberToOverride is IProperty) {
                var propertyDeclaration = GetOverrideDeclarationForProperty
                    ((IProperty) memberToOverride);
                script.InsertWithCursor
                    ( "THIS IS AN OPERATION"
                    , Script.InsertPosition.After
                    , new[] {propertyDeclaration});
            }
            else if (memberToOverride is IEvent) {
                var eventDeclaration = GetOverrideDeclarationForEvent
                    ((IEvent) memberToOverride);
                script.InsertWithCursor
                    ( "THIS IS AN OPERATION"
                    , Script.InsertPosition.After
                    , new[] {eventDeclaration});
            }
            else
                throw new NotSupportedException
                    ("Can only override IMethod / IProperty / IEvent");

            return script.CurrentDocument;
        }

        static MethodDeclaration GetOverrideDeclarationForMethod
            (IMethod iMethod) {

            var invocationExpression = new InvocationExpression();
                // ( methodName    : "base." + iMethod.Name
                // , typeArguments : iMethod.TypeParameters
                // , arguments     : iMethod.Parameters);

            MethodDeclaration methodDeclaration = new MethodDeclaration()
                { ReturnType = AstType.Create(iMethod.ReturnType.FullName)
                , Name       = iMethod.Name
                , Body       = new BlockStatement
                    // TODO call base.foo() with same arguments
                    {invocationExpression}};

            //methodDeclaration.Parameters.AddRange(iMethod.Parameters);
            // if (isStatic)
            //     methodDeclaration.Modifiers |= Modifiers.Static;

            return methodDeclaration;
        }

        static PropertyDeclaration GetOverrideDeclarationForProperty
            (IProperty iProperty) {
            throw new NotImplementedException();
        }

        static EventDeclaration GetOverrideDeclarationForEvent
            (IEvent iEvent) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Returns the type currently under the cursor in the given
        ///   Request.
        /// </summary>
        public static OverrideContext GetCurrentType(Request request) {
            throw new NotImplementedException();
        }


    }
}
