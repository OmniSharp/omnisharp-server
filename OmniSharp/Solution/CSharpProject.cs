using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using Mono.Cecil;
using ICSharpCode.NRefactory.Editor;
using OmniSharp.Configuration;

namespace OmniSharp.Solution
{
    public class Project : IProject
    {
        Logger _logger;
        IFileSystem _fileSystem;

        public Project (IFileSystem fileSystem, Logger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public CompilerSettings CompilerSettings { get; set; }
        static ConcurrentDictionary<string, IUnresolvedAssembly> assemblyDict = new ConcurrentDictionary<string, IUnresolvedAssembly>(Platform.FileNameComparer);

        public void UpdateFile(string fileName,string source)
        {
            var file = GetFile (fileName, source);
            file.Content = new StringTextSource(source);
            file.Parse(this, fileName, source);

            this.ProjectContent = this.ProjectContent
                .AddOrUpdateFiles(file.ParsedFile);
        }

        private CSharpFile GetFile(string fileName, string source)
        {
            var file = Files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            if (file == null)
            {
                file = new CSharpFile(this, fileName, source);
                Files.Add (file);

                this.ProjectContent = this.ProjectContent
                    .AddOrUpdateFiles(file.ParsedFile);
            }
            return file;
        }

        public CSharpParser CreateParser()
        {
            return new CSharpParser(CompilerSettings);
        }

        public XDocument AsXml()
        {
            return XDocument.Load(FileName);
        }

        public void AddReference(IAssemblyReference reference)
        {
            References.Add(reference);
            ProjectContent = ProjectContent.AddAssemblyReferences(References);
        }

        public void AddReference(string reference)
        {
            try
            {
                References.Add(LoadAssembly(reference));
                ProjectContent = ProjectContent.AddAssemblyReferences(References);
            }
            catch(BadImageFormatException)
            {
                // Ignore native dlls
                _logger.Error(reference + " is a native dll");
            }
        }


        public IUnresolvedAssembly LoadAssembly(string assemblyFileName)
        {
            if (!_fileSystem.File.Exists(assemblyFileName))
            {
                throw new FileNotFoundException("Assembly does not exist!", assemblyFileName);
            }
            return assemblyDict.GetOrAdd(assemblyFileName, file => new CecilLoader().LoadAssemblyFile(file));
        }





        public void Save(XDocument project)
        {
            project.Save(FileName);
        }

        public string FindAssembly (string evaluatedInclude)
        {
            throw new NotImplementedException ();
        }

        public IProjectContent ProjectContent {
            get {
                throw new NotImplementedException ();
            }
            set {
                throw new NotImplementedException ();
            }
        }

        public string Title { get; protected set; }

        public string FileName { get; protected set; }

        public List<CSharpFile> Files { get; protected set; }

        public List<IAssemblyReference> References { get; set; }

        public Guid ProjectId { get; protected set; }
    }

    public class CSharpProject : Project
    {
        public static readonly string[] AssemblySearchPaths =
        {
            //Windows Paths
            @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5",
            @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0",
            @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5",
            @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5",
            @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0",
            @"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.5",
            @"C:\Windows\Microsoft.NET\Framework\v2.0.50727",
            @"C:\Program Files (x86)\Microsoft ASP.NET\ASP.NET Web Pages\v2.0\Assemblies",
            @"C:\Program Files (x86)\Microsoft ASP.NET\ASP.NET Web Pages\v1.0\Assemblies",
            @"C:\Program Files (x86)\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies",
            @"C:\Program Files (x86)\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies",
            @"C:\Program Files\Microsoft ASP.NET\ASP.NET Web Pages\v2.0\Assemblies",
            @"C:\Program Files\Microsoft ASP.NET\ASP.NET Web Pages\v1.0\Assemblies",
            @"C:\Program Files\Microsoft ASP.NET\ASP.NET MVC 4\Assemblies",
            @"C:\Program Files\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies",
            @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v4.5",
            @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v4.0",
            @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v2.0",
            @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\ReferenceAssemblies\v2.0",
            @"C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE\PublicAssemblies",
            @"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v4.5",
            @"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v4.0",
            @"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v2.0",
            @"C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\ReferenceAssemblies\v2.0",
            @"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\PublicAssemblies",

            //Unix Paths
            @"/usr/local/lib/mono/4.5",
            @"/usr/local/lib/mono/4.0",
            @"/usr/local/lib/mono/3.5",
            @"/usr/local/lib/mono/2.0",
            @"/usr/lib/mono/4.5",
            @"/usr/lib/mono/4.0",
            @"/usr/lib/mono/3.5",
            @"/usr/lib/mono/2.0",
            @"/opt/mono/lib/mono/4.5",
            @"/opt/mono/lib/mono/4.0",
            @"/opt/mono/lib/mono/3.5",
            @"/opt/mono/lib/mono/2.0",

            //OS X Paths
            @"/Library/Frameworks/Mono.Framework/Libraries/mono/4.5",
            @"/Library/Frameworks/Mono.Framework/Libraries/mono/4.0",
            @"/Library/Frameworks/Mono.Framework/Libraries/mono/3.5",
            @"/Library/Frameworks/Mono.Framework/Libraries/mono/2.0",
            @"~/.kpm/packages"
        };
        private readonly ISolution _solution;

        public string FileName { get; private set; }

        public string AssemblyName { get; set; }

        public Guid ProjectId { get; private set; }

        public string Title { get; private set; }

        public IProjectContent ProjectContent { get; set; }

        public List<CSharpFile> Files { get; private set; }

        private readonly Logger _logger;

        IFileSystem _fileSystem;

        public CSharpProject (ISolution solution, 
                             Logger logger, 
                             string folderPath, 
                             IFileSystem fileSystem) : base (fileSystem, logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _solution = solution;

            Files = new List<CSharpFile>();
            References = new List<IAssemblyReference>();

            DirectoryInfoBase folder;

            try
            {
                folder = _fileSystem.DirectoryInfo.FromDirectoryName(folderPath);
            }
            catch(DirectoryNotFoundException)
            {
                logger.Error("Directory not found - " + folderPath);
                return;
            }

            var files = folder.GetFiles("*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                _logger.Debug("Loading " + file.FullName);
                Files.Add(new CSharpFile(this, file.FullName));
            }

            this.ProjectContent = new CSharpProjectContent()
                .SetAssemblyName(AssemblyName)
                .AddAssemblyReferences(References)
                .AddOrUpdateFiles(Files.Select(f => f.ParsedFile));

            AddMsCorlib();
            AddReference(LoadAssembly(FindAssembly("System.Core")));
            AddAllKpmPackages();

            var dlls = folder.GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (var dll in dlls)
            {
                _logger.Debug("Loading assembly " + dll.FullName);
                AddReference(dll.FullName);
            }
        }

        public CSharpProject(ISolution solution,
                             IFileSystem fileSystem,
                             Logger logger, 
                             string title, 
                             string fileName, 
                             Guid id) : base(fileSystem, logger)
        {
            _logger = logger;
            _solution = solution;
            Title = title;
            FileName = fileName.ForceNativePathSeparator();
            ProjectId = id;
            Files = new List<CSharpFile>();
            Microsoft.Build.Evaluation.Project project;

            try
            {
                project = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch(DirectoryNotFoundException)
            {
                logger.Error("Directory not found - " + FileName);
                return;
            }

            AssemblyName = project.GetPropertyValue("AssemblyName");

            SetCompilerSettings(project);

            AddCSharpFiles(project);

            References = new List<IAssemblyReference>();
            this.ProjectContent = new CSharpProjectContent()
                .SetAssemblyName(AssemblyName)
                .AddOrUpdateFiles(Files.Select(f => f.ParsedFile));

            AddMsCorlib();

            bool hasSystemCore = false;
            foreach (var item in project.GetItems("Reference"))
            {
                var assemblyFileName = GetAssemblyFileNameFromHintPath(project, item);
                //If there isn't a path hint or it doesn't exist, try searching
                if (assemblyFileName == null)
                    assemblyFileName = FindAssembly(item.EvaluatedInclude);

                //If it isn't in the search paths, try the GAC
                if (assemblyFileName == null && PlatformService.IsWindows)
                    assemblyFileName = FindAssemblyInNetGac(item.EvaluatedInclude);

                if (assemblyFileName != null)
                {
                    if (_fileSystem.Path.GetFileName(assemblyFileName).Equals("System.Core.dll", StringComparison.OrdinalIgnoreCase))
                        hasSystemCore = true;

                    _logger.Debug("Loading assembly " + item.EvaluatedInclude);
                    try
                    {
                        AddReference(LoadAssembly(assemblyFileName));
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                    }

                }
                else
                    _logger.Error("Could not find referenced assembly " + item.EvaluatedInclude);
            }
            if (!hasSystemCore && FindAssembly("System.Core") != null)
                AddReference(LoadAssembly(FindAssembly("System.Core")));


            AddProjectReferences(project);
        }

        private void AddAllKpmPackages()
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var folder = _fileSystem.DirectoryInfo.FromDirectoryName(_fileSystem.Path.Combine(userDir, ".kpm", "packages"));

            if(folder.Exists)
            {
                var dlls = folder.GetFiles("*.dll", SearchOption.AllDirectories);
                foreach (var dll in dlls)
                {
                    _logger.Debug(dll.FullName);

                    AddReference(dll.FullName);


                }
            }
        }

        string GetAssemblyFileNameFromHintPath(Microsoft.Build.Evaluation.Project p, Microsoft.Build.Evaluation.ProjectItem item)
        {
            string assemblyFileName = null;
            if (item.HasMetadata("HintPath"))
            {
                assemblyFileName = _fileSystem.Path.Combine(p.DirectoryPath, item.GetMetadataValue("HintPath")).ForceNativePathSeparator();
                _logger.Info("Looking for assembly from HintPath at " + assemblyFileName);
                if (!_fileSystem.File.Exists(assemblyFileName))
                {
                    _logger.Info("Did not find assembly from HintPath");
                    assemblyFileName = null;
                }
            }
            return assemblyFileName;
        }

        void SetCompilerSettings(Microsoft.Build.Evaluation.Project p)
        {
            CompilerSettings = new CompilerSettings
            {
                AllowUnsafeBlocks = GetBoolProperty(p, "AllowUnsafeBlocks") ?? false,
                CheckForOverflow = GetBoolProperty(p, "CheckForOverflowUnderflow") ?? false
            };
            string[] defines = p.GetPropertyValue("DefineConstants")
                .Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string define in defines)
                CompilerSettings.ConditionalSymbols.Add(define);

            var config = ConfigurationLoader.Config;
            foreach (var define in config.Defines)
                CompilerSettings.ConditionalSymbols.Add(define);
        }

        void AddMsCorlib()
        {
            string mscorlib = FindAssembly("mscorlib");
            if (mscorlib != null)
                AddReference(LoadAssembly(mscorlib));
            else
                _logger.Error("Could not find mscorlib");
        }

        void AddCSharpFiles(Microsoft.Build.Evaluation.Project p)
        {
            foreach (var item in p.GetItems("Compile"))
            {
                try
                {
                    string path = _fileSystem.Path.Combine(p.DirectoryPath, item.EvaluatedInclude).ForceNativePathSeparator();

                    if (_fileSystem.File.Exists(path))
                    {
                        string file = _fileSystem.FileInfo.FromFileName(path).FullName;
                        _logger.Debug("Loading " + file);
                        Files.Add(new CSharpFile(this, file));
                    }
                    else
                    {
                        _logger.Error("File does not exist - " + path);
                    }
                }
                catch (NullReferenceException e)
                {
                    _logger.Error(e);
                }
            }
        }

        void AddProjectReferences(Microsoft.Build.Evaluation.Project p)
        {
            foreach (Microsoft.Build.Evaluation.ProjectItem item in p.GetItems("ProjectReference"))
            {
                var projectName = item.GetMetadataValue("Name");
                var referenceGuid = Guid.Parse(item.GetMetadataValue("Project"));
                _logger.Debug("Adding project reference {0}, {1}", projectName, referenceGuid);
                AddReference(new ProjectReference(_solution, projectName, referenceGuid));
            }
        }

        public List<IAssemblyReference> References { get; set; }



        public override string ToString()
        {
            return string.Format("[CSharpProject AssemblyName={0}]", AssemblyName);
        }




        public string FindAssembly(string evaluatedInclude)
        {

            if (evaluatedInclude.IndexOf(',') >= 0)
                evaluatedInclude = evaluatedInclude.Substring(0, evaluatedInclude.IndexOf(','));

            string directAssemblyFile = (evaluatedInclude + ".dll").ForceNativePathSeparator();
            if (_fileSystem.File.Exists(directAssemblyFile))
                return directAssemblyFile;

            foreach (string searchPath in AssemblySearchPaths)
            {
                string assemblyFile = _fileSystem.Path.Combine(searchPath, evaluatedInclude + ".dll").ForceNativePathSeparator();
                if (_fileSystem.File.Exists(assemblyFile))
                    return assemblyFile;
            }
            return null;
        }

        string FindAssemblyInNetGac(string evaluatedInclude)
        {
            try
            {
                AssemblyNameReference assemblyNameReference = AssemblyNameReference.Parse(evaluatedInclude);
                return GacInterop.FindAssemblyInNetGac(assemblyNameReference);
            }
            catch (TypeInitializationException)
            {
                _logger.Debug("Fusion not available - cannot get {0} from the gac.", evaluatedInclude);
                return null;
            }
        }

        static bool? GetBoolProperty(Microsoft.Build.Evaluation.Project p, string propertyName)
        {
            string val = p.GetPropertyValue(propertyName);
            if (val.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (val.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;
            return null;
        }
    }
}
