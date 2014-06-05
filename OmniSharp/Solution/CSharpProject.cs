using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using Mono.Cecil;
using ICSharpCode.NRefactory.Editor;

namespace OmniSharp.Solution
{
    public class CSharpProject : IProject
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
            @"/Library/Frameworks/Mono.Framework/Libraries/mono/2.0"
        };
        private readonly ISolution _solution;

        public string FileName { get; private set; }

        public string AssemblyName { get; set; }

        public Guid ProjectId { get; private set; }

        public string Title { get; private set; }

        public IProjectContent ProjectContent { get; set; }

        public List<CSharpFile> Files { get; private set; }

        private CompilerSettings _compilerSettings;
        private readonly Logger _logger;
		
        public CSharpProject(ISolution solution, Logger logger, string folderPath)
        {
            _logger = logger;
            _solution = solution;

            Files = new List<CSharpFile>();
            References = new List<IAssemblyReference>();

            var folder = new DirectoryInfo(folderPath);
            var files = folder.EnumerateFiles("*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                _logger.Debug("Loading " + file.FullName);
                Files.Add(new CSharpFile(this, file.FullName));
            }

            var dlls = folder.EnumerateFiles("*.dll", SearchOption.AllDirectories);
            foreach (var dll in dlls)
            {
                Console.WriteLine(dll.FullName);
                AddReference(dll.FullName);
            }

            AddMsCorlib();
            AddReference(LoadAssembly(FindAssembly("System.Core")));
            this.ProjectContent = new CSharpProjectContent()
                .SetAssemblyName(AssemblyName)
                .AddAssemblyReferences(References)
                .AddOrUpdateFiles(Files.Select(f => f.ParsedFile));
        }

        public CSharpProject(ISolution solution, Logger logger, string title, string fileName, Guid id)
        {
            _logger = logger;
            _solution = solution;
            Title = title;
            FileName = fileName.ForceNativePathSeparator();
            ProjectId = id;
            Files = new List<CSharpFile>();

            var p = new Microsoft.Build.Evaluation.Project(FileName);
            AssemblyName = p.GetPropertyValue("AssemblyName");

            SetCompilerSettings(p);

            AddCSharpFiles(p);

            References = new List<IAssemblyReference>();
            AddMsCorlib();

            bool hasSystemCore = false;
            foreach (var item in p.GetItems("Reference"))
            {
                var assemblyFileName = GetAssemblyFileNameFromHintPath(p, item);
                //If there isn't a path hint or it doesn't exist, try searching
                if (assemblyFileName == null)
                    assemblyFileName = FindAssembly(item.EvaluatedInclude);

                //If it isn't in the search paths, try the GAC
                if (assemblyFileName == null && PlatformService.IsWindows)
                    assemblyFileName = FindAssemblyInNetGac(item.EvaluatedInclude);

                if (assemblyFileName != null)
                {
                    if (Path.GetFileName(assemblyFileName).Equals("System.Core.dll", StringComparison.OrdinalIgnoreCase))
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
                    _logger.Debug("Could not find referenced assembly " + item.EvaluatedInclude);
            }
            if (!hasSystemCore && FindAssembly("System.Core") != null)
                AddReference(LoadAssembly(FindAssembly("System.Core")));

            AddProjectReferences(p);

            this.ProjectContent = new CSharpProjectContent()
                .SetAssemblyName(AssemblyName)
                .AddAssemblyReferences(References)
                .AddOrUpdateFiles(Files.Select(f => f.ParsedFile));
        }

        string GetAssemblyFileNameFromHintPath(Microsoft.Build.Evaluation.Project p, Microsoft.Build.Evaluation.ProjectItem item)
        {
            string assemblyFileName = null;
            if (item.HasMetadata("HintPath"))
            {
                assemblyFileName = Path.Combine(p.DirectoryPath, item.GetMetadataValue("HintPath")).ForceNativePathSeparator();
                _logger.Info("Looking for assembly from HintPath at " + assemblyFileName);
                if (!File.Exists(assemblyFileName))
                {
                    _logger.Info("Did not find assembly from HintPath");
                    assemblyFileName = null;
                }
            }
            return assemblyFileName;
        }

        void SetCompilerSettings(Microsoft.Build.Evaluation.Project p)
        {
            _compilerSettings = new CompilerSettings
            {
                AllowUnsafeBlocks = GetBoolProperty(p, "AllowUnsafeBlocks") ?? false,
                CheckForOverflow = GetBoolProperty(p, "CheckForOverflowUnderflow") ?? false
            };
            string[] defines = p.GetPropertyValue("DefineConstants")
                                .Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string define in defines)
                _compilerSettings.ConditionalSymbols.Add(define);
        }

        void AddMsCorlib()
        {
            string mscorlib = FindAssembly("mscorlib");
            if (mscorlib != null)
                AddReference(LoadAssembly(mscorlib));
            else
                _logger.Debug("Could not find mscorlib");
        }

        void AddCSharpFiles(Microsoft.Build.Evaluation.Project p)
        {
            foreach (var item in p.GetItems("Compile"))
            {
                try
                {
                    string path = Path.Combine(p.DirectoryPath, item.EvaluatedInclude).ForceNativePathSeparator();
                   
                    if (File.Exists(path))
                    {
                        string file = new FileInfo(path).FullName;
                        _logger.Debug("Loading " + file);
                        Files.Add(new CSharpFile(this, file));
                    }
                    else
                    {
                        _logger.Debug("File does not exist - " + path);
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

        public void AddReference(IAssemblyReference reference)
        {
            References.Add(reference);
        }

        public void AddReference(string reference)
        {
            References.Add(LoadAssembly(reference));
        }

        public CSharpFile GetFile(string fileName)
        {
            return Files.Single(f => f.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public void UpdateFile(string fileName, string source)
        {
            var file = GetFile(fileName);
            file.Content = new StringTextSource(source);
            file.Parse(this, fileName, source);
        }

        public CSharpParser CreateParser()
        {
            return new CSharpParser(_compilerSettings);
        }

        public XDocument AsXml()
        {
            return XDocument.Load(FileName);
        }

        public void Save(XDocument project)
        {
            project.Save(FileName);
        }
        
        public override string ToString()
        {
            return string.Format("[CSharpProject AssemblyName={0}]", AssemblyName);
        }

        static ConcurrentDictionary<string, IUnresolvedAssembly> assemblyDict = new ConcurrentDictionary<string, IUnresolvedAssembly>(Platform.FileNameComparer);

        public static IUnresolvedAssembly LoadAssembly(string assemblyFileName)
        {
            if (!File.Exists(assemblyFileName))
            {
                throw new FileNotFoundException("Assembly does not exist!", assemblyFileName);
            }
            return assemblyDict.GetOrAdd(assemblyFileName, file => new CecilLoader().LoadAssemblyFile(file));
        }

        public static string FindAssembly(string evaluatedInclude)
        {

            if (evaluatedInclude.IndexOf(',') >= 0)
                evaluatedInclude = evaluatedInclude.Substring(0, evaluatedInclude.IndexOf(','));
            
            string directAssemblyFile = (evaluatedInclude + ".dll").ForceNativePathSeparator();
            if (File.Exists(directAssemblyFile))
                return directAssemblyFile;

            foreach (string searchPath in AssemblySearchPaths)
            {
                string assemblyFile = Path.Combine(searchPath, evaluatedInclude + ".dll").ForceNativePathSeparator();
                if (File.Exists(assemblyFile))
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
