using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;

namespace OmniSharp.GotoRegion {

    public class GetDirectivesASTVisitor : DepthFirstAstVisitor {
        public IList<PreProcessorDirective> Directives =
            new List<PreProcessorDirective>();

        public override void VisitPreProcessorDirective
            (PreProcessorDirective preProcessorDirective) {
            Directives.Add(preProcessorDirective);
        }
        #region testesteetsets

        #endregion testesteetsets

    }
}
