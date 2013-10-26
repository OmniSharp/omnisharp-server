using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using OmniSharp.AutoComplete;
using OmniSharp.Common;
using OmniSharp.Parser;
using OmniSharp.Solution;

namespace OmniSharp.ContextInformation
{
    public class GetContextHandler
    {
        private readonly ISolution _solution;
        private readonly BufferParser _parser;

        public GetContextHandler(ISolution solution, BufferParser parser)
        {
            _solution = solution;
            _parser = parser;
        }

        public GetContextResponse GetContextResponse(Request request)
        {
            string methodName = null;
            string typeName = null;

            var node = new BufferContext(request, _parser).NodeCurrentlyUnderCursor;
            var method = (MethodDeclaration) node.AncestorsAndSelf.FirstOrDefault(n => n is MethodDeclaration);
            if (method != null)
                methodName = method.Name;

            var type = (TypeDeclaration)node.AncestorsAndSelf.FirstOrDefault(n => n is TypeDeclaration);
            var namespaceDeclaration = (NamespaceDeclaration)node.AncestorsAndSelf.FirstOrDefault(n => n is NamespaceDeclaration);

            if (type != null)
                typeName = type.Name;

            if (namespaceDeclaration != null)
                typeName = namespaceDeclaration.FullName + "." + typeName;

            var project = _solution.ProjectContainingFile(request.FileName);
            var directory = new FileInfo(project.FileName).Directory.GetDirectories("bin/Debug").First().FullName;
            var assemblyName = Path.Combine(directory, project.ProjectContent.FullAssemblyName + ".dll");

            return new GetContextResponse
                {
                    MethodName = methodName, 
                    TypeName = typeName, 
                    AssemblyName = "\"" + assemblyName + "\""
                };
        }
    }
}