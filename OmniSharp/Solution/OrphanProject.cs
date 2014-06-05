using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;
using ICSharpCode.NRefactory.Editor;

namespace OmniSharp.Solution
{
    /// <summary>
    /// Placeholder that can be used for files that don't belong to a project.
    /// </summary>
    public class OrphanProject : IProject
    {
        public string Title { get; private set; }
        public List<CSharpFile> Files { get; private set; }
        public List<IAssemblyReference> References { get; set; }
        public IProjectContent ProjectContent { get; set; }
        public string FileName { get; private set; }
        public Guid ProjectId { get; private set; }

        public void AddReference(IAssemblyReference reference)
        {
            References.Add(reference);
            ProjectContent.AddAssemblyReferences (new[] { reference });
        }

        public void AddReference(string reference)
        {
            AddReference(CSharpProject.LoadAssembly(reference));
        }

        public void AddFile(string fileName)
        {
            var csharpFile = new CSharpFile (this, fileName);
            Files.Add (csharpFile);
            ProjectContent.AddOrUpdateFiles (new[] { csharpFile.ParsedFile });
        }

        public OrphanProject()
        {
            Title = "Orphan Project";
            Files = new List<CSharpFile>();

            ProjectId = Guid.NewGuid();

            References = new List<IAssemblyReference>();
            FileName = "OrphanProject";
            string mscorlib = CSharpProject.FindAssembly("mscorlib");
            Console.WriteLine(mscorlib);
            ProjectContent = new CSharpProjectContent()
                .SetAssemblyName("OrphanProject")
                .AddAssemblyReferences(CSharpProject.LoadAssembly(mscorlib));
        }

        private CSharpFile GetFile(string fileName, string source)
        {
            var file = Files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            if (file == null)
            {
                file = new CSharpFile(this, fileName, source);
                Files.Add (file);

                this.ProjectContent
                    .AddOrUpdateFiles(file.ParsedFile);
            }
            return file;
        }

        public void UpdateFile(string fileName,string source)
        {
            var file = GetFile (fileName, source);
            file.Content = new StringTextSource(source);
            file.Parse(this, fileName, source);
        }

        public CSharpParser CreateParser()
        {
            return new CSharpParser();
        }

        public XDocument AsXml()
        {
            throw new NotImplementedException();
        }

        public void Save(XDocument project)
        {
            throw new NotImplementedException();
        }

    }
}
