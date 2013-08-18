using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using OmniSharp.Solution;

namespace OmniSharp.Documentation
{
    public class DocumentationFetcher
    {
        public string GetDocumentation(IProject project, IEntity entity)
        {
            DocumentationComment documentationComment = null;
            if (entity.Documentation != null)
            {
                // Documentation from source code
                documentationComment = entity.Documentation;
            }
            else
            {
                if (entity.ParentAssembly.AssemblyName != null)
                {
                    IDocumentationProvider docProvider = 
                        XmlDocumentationProviderFactory.Get(project, entity.ParentAssembly.AssemblyName);

                    if (docProvider != null)
                    {
                        documentationComment = docProvider.GetDocumentation(entity);
                    }
                }
            }

            return documentationComment != null 
                ? DocumentationConverter.ConvertDocumentation(documentationComment.Xml.Text) 
                : null;
        }
    }
}
