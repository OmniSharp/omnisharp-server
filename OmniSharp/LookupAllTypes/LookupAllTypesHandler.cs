using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using OmniSharp.Parser;
using OmniSharp.Solution;

namespace OmniSharp.LookupAllTypes
{
    public class LookupAllTypesHandler
    {
        private readonly ISolution _solution;

        public LookupAllTypesHandler(ISolution solution)
        {
            _solution = solution;
        }

        string GetAllTypesAsString()
        {
            var types = new HashSet<string>();

            foreach (var project in _solution.Projects)
            {
                AddProjectTypes(project, types);
            }

            // This causes a conflict with the vim keyword 'contains'
            types.Remove("Contains");

            return types.Aggregate("", (current, type) => current + type + " ");
        }

        private void AddProjectTypes(IProject project, HashSet<string> types)
        {
            foreach (var reference in project.References)
            {
                var unresolvedRef = reference as IUnresolvedAssembly;

                if (unresolvedRef != null)
                {
                    foreach (var def in unresolvedRef.GetAllTypeDefinitions())
                    {
                        types.Add(def.Name);
                    }
                }
            }

            foreach (var type in project.ProjectContent.GetAllTypeDefinitions())
            {
                types.Add(type.Name);
            }
        }

        public LookupAllTypesResponse GetLookupAllTypesResponse(LookupAllTypesRequest request)
        {
            return new LookupAllTypesResponse { AllTypes = GetAllTypesAsString() };
        }
    }
}
