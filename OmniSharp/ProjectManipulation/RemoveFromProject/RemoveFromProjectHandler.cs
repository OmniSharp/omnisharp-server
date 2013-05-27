using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using OmniSharp.Solution;

namespace OmniSharp.ProjectManipulation.RemoveFromProject
{
    public class RemoveFromProjectHandler
    {
        readonly ISolution _solution;
        private readonly XNamespace _msBuildNameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public RemoveFromProjectHandler(ISolution solution)
        {
            _solution = solution;
        }

        public void RemoveFromProject(RemoveFromProjectRequest request)
        {
            var relativeProject = _solution.ProjectContainingFile(request.FileName);

            if (relativeProject == null || relativeProject is OrphanProject)
            {
                throw new ProjectNotFoundException(string.Format("Unable to find project relative to file {0}", request.FileName));
            }

            var project = relativeProject.AsXml();

            var relativeFileName = request.FileName.Replace(relativeProject.FileName.Substring(0, relativeProject.FileName.LastIndexOf(Path.DirectorySeparatorChar) + 1), "")
                .Replace("/", @"\");

            var compilationNodes = GetCompilationNodes(project).ToList();

            var fileNode = compilationNodes.FirstOrDefault(n => n.Attribute("Include").Value.Equals(relativeFileName, StringComparison.InvariantCultureIgnoreCase));

            if (fileNode != null)
            {
                project.CompilationNodes().Where(n => n.Attribute("Include").Value.Equals(relativeFileName, StringComparison.InvariantCultureIgnoreCase)).Remove();
                
                relativeProject.Save(project);
            }
        }

        private IEnumerable<XElement> GetCompilationNodes(XDocument project)
        {
            return project.Element(_msBuildNameSpace + "Project")
                          .Elements(_msBuildNameSpace + "ItemGroup")
                          .Elements(_msBuildNameSpace + "Compile");
        }
    }

    public static class XDocumentExtensions
    {
        private static readonly XNamespace _msBuildNameSpace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public static IEnumerable<XElement> CompilationNodes(this XDocument document)
        {
            return document.Element(_msBuildNameSpace + "Project")
                          .Elements(_msBuildNameSpace + "ItemGroup")
                          .Elements(_msBuildNameSpace + "Compile");
        }
    }
}
