using System;
using System.Linq;
using OmniSharp.Solution;
using System.IO.Abstractions;
using System.IO;

namespace OmniSharp
{
    public class SolutionPicker
    {
        readonly IFileSystem _fileSystem;

        Logger _logger;

        public SolutionPicker(IFileSystem fileSystem, Logger logger)
        {
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public ISolution LoadSolution(string solutionPath)
        {
            solutionPath = solutionPath.ApplyPathReplacementsForServer();
            var solution = PickSolution(solutionPath);
            solution.LoadSolution();
            AddAspNet5Projects(solutionPath, solution);
            return solution;
        }

        void AddAspNet5Projects(string solutionPath, ISolution solution)
        {
            var aspNet5Projects = Directory.EnumerateFiles(solutionPath, "project.json", SearchOption.AllDirectories);
            if (aspNet5Projects.Any())
            {
                var dth = new DesignTimeHostDemo.Program();
                dth.Go(solutionPath, _logger.Debug);
                   
                foreach (var projectFile in aspNet5Projects)
                {
                    _logger.Debug("Loading ASPNet 5 project - " + projectFile);
                    dth.RegisterProject(projectFile);
                    string projectPath = Path.GetDirectoryName(projectFile).TrimEnd(Path.DirectorySeparatorChar);
                    solution.Projects.Add(new AspNet5Project(solution, _logger, projectPath, _fileSystem));
                }
            }
        }

        ISolution PickSolution(string solutionPath)
        {
            if (_fileSystem.Directory.Exists(solutionPath))
            {
                var unitySolutions = GetUnitySolutions(solutionPath);
                if (unitySolutions.Length == 1)
                {
                    return GetSolution(unitySolutions[0]);
                }

                var slnFiles = _fileSystem.Directory.GetFiles(solutionPath, "*.sln");
                if (slnFiles.Length == 1)
                {
                    return GetSolution(slnFiles[0]);
                }

                return new CSharpFolder(solutionPath, _logger, _fileSystem);
            }
            return new MSBuildSolution(_fileSystem, solutionPath, _logger);
        }

        MSBuildSolution GetSolution(string solutionPath)
        {
            _logger.Debug("Found solution file - " + solutionPath);
            return new MSBuildSolution(_fileSystem, solutionPath, _logger);
        }

        string[] GetUnitySolutions(string solutionPath)
        {
            return _fileSystem.Directory.GetFiles(solutionPath, "*-csharp.sln");
        }
    }
}

