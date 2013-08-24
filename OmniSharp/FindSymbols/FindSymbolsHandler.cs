using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.Common;
using OmniSharp.Solution;

namespace OmniSharp.FindSymbols
{
    public class FindSymbolsHandler
    {
        private readonly ISolution _solution;

        public FindSymbolsHandler(ISolution solution)
        {
            _solution = solution;
        }

        /// <summary>
        /// Find all symbols that only exist within the solution source tree
        /// </summary>
        public QuickFixResponse FindAllSymbols()
        {
            var types =
                from project in _solution.Projects
                from type in project.ProjectContent.GetAllTypeDefinitions()
                select type;

            var quickfixes = types.SelectMany(GetTypeAndItsMembers)
                .ToArray();

            return new QuickFixResponse(quickfixes);
        }

        private static IList<QuickFix> GetTypeAndItsMembers
            (IUnresolvedTypeDefinition t) {
            var typeLocation = new QuickFix(t);
            var typeMembersLocations = t.Members == null
                ? new QuickFix[]{}
                : t.Members.Select(m => new QuickFix(m));

            // Type name first, its members second
            var retval = new List<QuickFix> {typeLocation};
            retval.AddRange(typeMembersLocations);
            return retval;
        }

    }
}
