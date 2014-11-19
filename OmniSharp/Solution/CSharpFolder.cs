using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using DesignTimeHostDemo;
using System.Text.RegularExpressions;

namespace OmniSharp.Solution
{
    public class CSharpFolder : ISolution
    {
        Logger _logger;
        CSharpProject _project;
        IFileSystem _fileSystem;

        public CSharpFolder(string folder, Logger logger, IFileSystem fileSystem)
        {
            _logger = logger;
            FileName = folder;
            _fileSystem = fileSystem;
        }

        public string FileName { get; private set; }
        public bool Terminated { get; private set; }
        public bool Loaded { get; private set; }

        public void LoadSolution()
        {
            Loaded = false;
            _project = new CSharpProject(this, _logger, FileName, _fileSystem);
            Loaded = true;
            var dth = new DesignTimeHostDemo.Program();
            var paths = Environment.GetEnvironmentVariable("PATH").Split(':');
            var kreBinPath = paths.FirstOrDefault(path => path.Contains ("packages") && path.Contains ("KRE"));
//            var activeKreOutput = ShellWrapper.GetShellOuput("kvm", "list | grep default");
//            var components = Regex.Split(activeKreOutput, @"\s{1,}");
            //*    1.0.0-beta2-10709    Mono    ~/.kre/packages      default
            // /Users/jason/.kre/packages/KRE-Mono.1.0.0-beta2-10709/bin
            dth.Go(kreBinPath.Remove(kreBinPath.Length - 3), FileName);
            dth.OnUpdateFileReference += OnUpdateFileReference;
        }

        void OnUpdateFileReference(object sender, FileReferenceEventArgs e)
        {
            _project.AddReference(e.Reference);
        }

        public CSharpFile GetFile(string filename)
        {
            return (from file in _project.Files
                     where file.FileName.Equals(filename, StringComparison.InvariantCultureIgnoreCase)
                     select file).FirstOrDefault();
        }

        public IProject ProjectContainingFile(string filename)
        {
            return _project;
        }

        public void Reload()
        {
            LoadSolution();
        }

        public void Terminate()
        {
            Terminated = true;
        }

        public List<IProject> Projects
        {
            get
            {
                return new List<IProject> { _project };
            }
        }

    }
    
}
