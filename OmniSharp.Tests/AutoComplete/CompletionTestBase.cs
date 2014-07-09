using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Completion;

namespace OmniSharp.Tests.AutoComplete
{
    public class CompletionTestBase
    {
        protected IEnumerable<string> DisplayTextFor(string input, bool includeImportableTypes = false)
        {
            return CompletionsDataFor(input, includeImportableTypes).Select(c => c.DisplayText);
        }

        protected IEnumerable<string> CompletionsFor(string input, bool includeImportableTypes = false)
        {
            return CompletionsDataFor(input, includeImportableTypes).Select(c => c.CompletionText);
        }

        protected IEnumerable<ICompletionData> CompletionsDataFor(string input, bool includeImportableTypes = false)
        {
            return new CompletionsSpecBase().GetCompletions(input, includeImportableTypes);
        }
    }
}
