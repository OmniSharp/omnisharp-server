
// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace OmniSharp.Solution
{
    public interface ISolution
    {
        bool Loaded { get; }
        List<IProject> Projects { get; }
        string FileName { get; }
        CSharpFile GetFile(string filename);
        IProject ProjectContainingFile(string filename);
        void Reload();
        void Terminate();
        void LoadSolution(string fileName);
        bool Terminated { get; }
    }


    public class CSharpSolution : ISolution
    {
        private OrphanProject _orphanProject;
        private Logger _logger;

        public List<IProject> Projects { get; private set; }

        public string FileName { get; private set; }

        public bool Terminated { get; set; }

        public bool Loaded { get; private set; }

        public CSharpSolution(Logger logger)
        {
            _logger = logger;
        }

        public void LoadSolution(string fileName)
        {
            Loaded = false;
            FileName = fileName;
            _orphanProject = new OrphanProject();
            Projects = new List<IProject>();
            Projects.Add(_orphanProject);

            var directory = Path.GetDirectoryName(fileName);
            var projectLinePattern =
                new Regex(
                    "Project\\(\"(?<TypeGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"");

            foreach (string line in File.ReadLines(fileName))
            {
                Match match = projectLinePattern.Match(line);
                if (match.Success)
                {
                    string typeGuid = match.Groups["TypeGuid"].Value;
                    string title = match.Groups["Title"].Value;
                    string location = Path.Combine(directory, match.Groups["Location"].Value).LowerCaseDriveLetter();
                    string guid = match.Groups["Guid"].Value;

                    switch (typeGuid.ToUpperInvariant())
                    {
                        case "{2150E333-8FDC-42A3-9474-1A3956D46DE8}": // Solution Folder
                                // ignore folders
                            break;
                        case "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}": // C# project
                            LoadProject(title, location, guid);
                            break;
                        default:
                                // Unity3D makes type GUID from the MD5 of title.
                            if (MD5(title) == typeGuid.Substring(1, typeGuid.Length - 2).ToLower().Replace("-", ""))
                            {
                                LoadProject(title, location, guid);
                            }
                            else
                            {
                                _logger.Debug("Project {0} has unsupported type {1}", location, typeGuid);
                            }
                            break;
                    }
                }
            }
            Loaded = true;
        }

        public void LoadProject(string title, string location, string id)
        {
            _logger.Debug("Loading project - {0}, {1}, {2}", title, location, id);
            Projects.Add(new CSharpProject(this, _logger, title, location, new Guid(id)));
        }

        public CSharpFile GetFile(string filename)
        {
            return (from project in Projects
                             from file in project.Files
                             where file.FileName.Equals(filename, StringComparison.InvariantCultureIgnoreCase)
                             select file).FirstOrDefault();
        }

        public IProject ProjectContainingFile(string filename)
        {
            _logger.Info("Looking for project containing file " + filename);
            var project = Projects.FirstOrDefault(p => p.Files.Any(f => f.FileName.Equals(filename, StringComparison.InvariantCultureIgnoreCase)));
            if (project == null)
            {
                var file = new FileInfo(filename);
                var directory = file.Directory;

                while (project == null && directory != null)
                {
                    var projectFiles = directory.GetFiles("*.csproj");
                    directory = directory.Parent;

                    if (projectFiles.Any())
                    {
                        foreach (var projectFile in projectFiles)
                        {
                            project = Projects.FirstOrDefault(p => projectFile.FullName.Contains(p.FileName));
                            if (project != null)
                            {
                                if (File.Exists(filename))
                                {
                                    project.Files.Add(new CSharpFile(project, filename));
                                }
                                else
                                {
                                    project.Files.Add(new CSharpFile(project, filename, ""));
                                }
                                break;
                            }
                        }
                    }
                }
            }

            project = project ?? _orphanProject;
            _logger.Info(filename + " belongs to " + project.FileName);
            return project;
        }

        public void Reload()
        {
            LoadSolution(FileName);
        }

        public void Terminate()
        {
            Terminated = true;
        }

        private static string MD5(string str)
        {
            var provider = new MD5CryptoServiceProvider();
            byte[] bytes = provider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(bytes).ToLower().Replace("-", "");
        }
    }
}
