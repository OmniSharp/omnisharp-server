using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO;
using System.Linq;
using DesignTimeHostDemo;

namespace OmniSharp.Solution
{
    public class CSharpFolder : ISolution
    {
        Logger _logger;
        AspNet5Project _project;
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
            _project = new AspNet5Project(this, _logger, FileName, _fileSystem);
            Loaded = true;

            var dth = new DesignTimeHostDemo.Program();
            var kreHome = Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".kre");

            _logger.Debug("KRE Home = " + kreHome);

            var defaultAlias = Path.Combine(kreHome, "alias", "default.alias");

            _logger.Debug("Using default alias = " + defaultAlias);

            var version = File.ReadAllText(defaultAlias).Trim();

            _logger.Debug("Using KRE version = " + version);

            // TODO: Make this work on windows

            var krePath = Path.Combine(kreHome, "packages", version);

            _logger.Debug("Using KRE at = " + krePath);

            dth.Go(krePath, FileName, val => _logger.Debug(val));
            dth.OnUpdateFileReference += OnUpdateFileReference;
            dth.OnUpdateSourceFileReference += OnUpdateSourceFileReference;
        }

        void OnUpdateSourceFileReference(IEnumerable<string> files)
        {
            _project.AddFiles(files);
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
