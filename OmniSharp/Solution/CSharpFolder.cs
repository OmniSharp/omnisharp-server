using System;
using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Solution
{
    public class CSharpFolder : ISolution
    {
        Logger _logger;
        CSharpProject _project;
        string _folder;

        public CSharpFolder(Logger logger)
        {
            _logger = logger;
        }

        public bool Terminated { get; private set; }

        public void LoadSolution(string folder)
        {
            _folder = folder;
            Loaded = false;
            _project = new CSharpProject(this, _logger, folder);
            Loaded = true;
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
            LoadSolution(_folder);
        }

        public void Terminate()
        {
            Terminated = true;
        }

        public bool Loaded { get; private set; }

        public List<IProject> Projects
        {
            get
            {
                return new List<IProject> { _project };
            }
        }

        public string FileName { get; private set; }
    }
    
}
