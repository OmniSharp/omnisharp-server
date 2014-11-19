using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using OmniSharp.Configuration;

namespace OmniSharp.Solution
{
    public class AspNet5Project : Project
    {
        public string AssemblyName { get; set; }

        private readonly Logger _logger;

        public AspNet5Project (ISolution solution, 
                               Logger logger, 
                               string folderPath, 
                               IFileSystem fileSystem) : base (fileSystem, logger)
        {
            _logger = logger;

            Files = new List<CSharpFile>();
            References = new List<IAssemblyReference>();

            this.ProjectContent = new CSharpProjectContent()
                .SetAssemblyName(AssemblyName)
                .AddAssemblyReferences(References);
        }

        public void AddFiles(IEnumerable<string> files)
        {
        	foreach (var file in files)
        	{
        		_logger.Debug("Loading " + file);
        		
        		var csFile = new CSharpFile(this, file);
        		Files.Add(csFile);

        		this.ProjectContent = this.ProjectContent
                    .AddOrUpdateFiles (csFile.ParsedFile);
        	}
        }
    }
}
