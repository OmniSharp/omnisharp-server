using System.Collections.Generic;
using System.Linq;
using OmniSharp.Common;
using OmniSharp.Solution;

namespace OmniSharp.FindProjects
{
    public class FindProjectsHandler
    {
        readonly ISolution _solution;
        public FindProjectsHandler(ISolution solution)
        {
            _solution = solution;
        }

        public QuickFixResponse FindAllProjects()
        {
            
            var quickfixes = _solution.Projects.OrderBy(x => x.Title).Select(t => new QuickFix
                {
                    Text = t.Title,
                    FileName = t.FileName
                });

            return new QuickFixResponse(quickfixes);
        }
        
    }
}
