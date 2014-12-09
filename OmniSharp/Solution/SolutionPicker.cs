using System;
using OmniSharp.Solution;
using System.IO.Abstractions;

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

            return PickSolution(solutionPath);
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

