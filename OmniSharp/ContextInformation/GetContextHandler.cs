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

            var bufferContext = new BufferContext(request, _parser);
			var node = bufferContext.NodeCurrentlyUnderCursor;

            TypeDeclaration type = null;
			NamespaceDeclaration namespaceDeclaration = null;
			if(node != null)
			{
				var method = (MethodDeclaration) node.AncestorsAndSelf.FirstOrDefault(n => n is MethodDeclaration);
				if (method != null)
					methodName = method.Name;
				type = (TypeDeclaration)node.AncestorsAndSelf.FirstOrDefault(n => n is TypeDeclaration);
				namespaceDeclaration = (NamespaceDeclaration)node.AncestorsAndSelf.FirstOrDefault(n => n is NamespaceDeclaration);
			}


            if (type == null)
			{
				var tree = bufferContext.ParsedContent.SyntaxTree;
				type = (TypeDeclaration)tree.DescendantsAndSelf.FirstOrDefault(n => n is TypeDeclaration);
				namespaceDeclaration = (NamespaceDeclaration)tree.DescendantsAndSelf.FirstOrDefault(n => n is NamespaceDeclaration);
			}

			typeName = type.Name;
            if (namespaceDeclaration != null)
                typeName = namespaceDeclaration.FullName + "." + typeName;

            var project = _solution.ProjectContainingFile(request.FileName);
            var directory = new FileInfo(project.FileName).Directory.FullName;
			
            var assemblyName = Path.Combine(directory, "bin", "Debug", project.ProjectContent.FullAssemblyName + ".dll");

            return new GetContextResponse
                {
                    MethodName = methodName, 
                    TypeName = typeName, 
                    AssemblyName = "\"" + assemblyName + "\""
                };
        }
    }
}
