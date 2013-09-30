using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace OmniSharp.CodeIssues
{
    public class CodeIssueProviders
    {
        public IEnumerable<ICodeIssueProvider> GetProviders()
        {
            var types = Assembly.GetAssembly(typeof(ICodeIssueProvider))
                                .GetTypes()
                                .Where(t => typeof(ICodeIssueProvider).IsAssignableFrom(t)
                                        && !t.IsAbstract);

            IEnumerable<ICodeIssueProvider> providers =
                types
                    .Where(type => !type.IsInterface && !type.ContainsGenericParameters) //TODO: handle providers with generic params 
                    .Select(type => (ICodeIssueProvider) Activator.CreateInstance(type));

            return providers;
        }
    }
}