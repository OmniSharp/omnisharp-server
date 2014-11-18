using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.DesignTimeHost.Models;
using Microsoft.Framework.DesignTimeHost.Models.IncomingMessages;
using Microsoft.Framework.DesignTimeHost.Models.OutgoingMessages;
using Newtonsoft.Json.Linq;

namespace DesignTimeHostDemo
{
    public class FileReferenceEventArgs : EventArgs
    {
        public string Reference { get; private set; }

        public FileReferenceEventArgs(string reference)
        {
            Reference = reference;
        }
    }

    public class Program
    {
        public delegate void FileReferenceUpdateHandler(object sender, FileReferenceEventArgs e);
        public event FileReferenceUpdateHandler OnUpdateFileReference;

        public void Go(string runtimePath, string applicationRoot)
        {
            var hostId = Guid.NewGuid().ToString();
            var port = 1334;

            // Show runtime output
            var showRuntimeOutput = true;

            // this can be k10
            var activeTargetFramework = "net45";

            StartRuntime(runtimePath, applicationRoot, hostId, port, showRuntimeOutput, () =>
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Loopback, port));

                var networkStream = new NetworkStream(socket);

                Console.WriteLine("Connected");

                var mapping = new Dictionary<int, string>();
                var queue = new ProcessingQueue(networkStream);

                queue.OnReceive += m =>
                {
                    // Get the project associated with this message
                    var projectPath = mapping[m.ContextId];

                    Console.WriteLine(m.MessageType);
                    // This is where we can handle messages and update the
                    // language service
                    if (m.MessageType == "References")
                    {
                        // References as well as the dependency graph information
                         var val = m.Payload.ToObject<ReferencesMessage>();
                         foreach(var reference in val.FileReferences)
                         {
                            var args = new FileReferenceEventArgs(reference);
                            OnUpdateFileReference(this, args);
                            Console.WriteLine(reference);
                         }
                    }
                    else if (m.MessageType == "Diagnostics")
                    {
                        // Errors and warnings
                        var val = m.Payload.ToObject<DiagnosticsMessage>();
                        foreach(var error in val.Errors)
                        {
                            Console.WriteLine(error);
                        }

                        foreach(var warning in val.Warnings)
                        {
                            Console.WriteLine(warning);
                        }
                    }
                    else if (m.MessageType == "Configurations")
                    {
                        // Configuration and compiler options
                        // var val = m.Payload.ToObject<ConfigurationsMessage>();
                    }
                    else if (m.MessageType == "Sources")
                    {
                        // The sources to feed to the language service
                        // var val = m.Payload.ToObject<SourcesMessage>();
                    }
                };

                // Start the message channel
                queue.Start();

                var solutionPath = applicationRoot;
                var watcher = new FileWatcher(solutionPath);
                var projects = new Dictionary<string, int>();
                int contextId = 0;

                foreach (var projectFile in Directory.EnumerateFiles(solutionPath, "project.json", SearchOption.AllDirectories))
                {
                    string projectPath = Path.GetDirectoryName(projectFile).TrimEnd(Path.DirectorySeparatorChar);

                    // Send an InitializeMessage for each project
                    var initializeMessage = new InitializeMessage
                    {
                        ProjectFolder = projectPath,
                        TargetFramework = activeTargetFramework
                    };

                    // Create a unique id for this project
                    int projectContextId = contextId++;

                    // Create a mapping from path to contextid and back
                    projects[projectPath] = projectContextId;
                    mapping[projectContextId] = projectPath;

                    // Initialize this project
                    queue.Post(new Message
                    {
                        ContextId = projectContextId,
                        MessageType = "Initialize",
                        Payload = JToken.FromObject(initializeMessage),
                        HostId = hostId
                    });

                    // Watch the project.json file
                    watcher.WatchFile(Path.Combine(projectPath, "project.json"));
                    watcher.WatchDirectory(projectPath, ".cs");

                    // Watch all directories for cs files

                    foreach (var cs in Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories))
                    {
                        watcher.WatchFile(cs);
                    }

                    foreach (var d in Directory.GetDirectories(projectPath, "*.*", SearchOption.AllDirectories))
                    {
                        watcher.WatchDirectory(d, ".cs");
                    }
                }

                // When there's a file change
                watcher.OnChanged += changedPath =>
                {
                    foreach (var project in projects)
                    {
                        // If the project changed
                        if (changedPath.StartsWith(project.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            queue.Post(new Message
                            {
                                ContextId = project.Value,
                                MessageType = "FilesChanged",
                                HostId = hostId
                            });
                        }
                    }
                };
            });

//            Console.WriteLine("Process Q to exit");
//
//            while (true)
//            {
//                var ki = Console.ReadKey(true);
//
//                if (ki.Key == ConsoleKey.Q)
//                {
//                    break;
//                }
//            }
        }

        private static void StartRuntime(string runtimePath,
                                         string applicationPath,
                                         string hostId,
                                         int port,
                                         bool verboseOutput,
                                         Action onStart)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(runtimePath, "bin", "klr"),
                Arguments = String.Format(@"{0} {1} {2} {3}",
                                          Path.Combine(runtimePath, "bin", "lib", "Microsoft.Framework.DesignTimeHost", "Microsoft.Framework.DesignTimeHost.dll"),
                                          port,
                                          Process.GetCurrentProcess().Id,
                                          hostId),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = applicationPath
            };

            Console.WriteLine(psi.FileName + " " + psi.Arguments);
            Environment.SetEnvironmentVariable ("KRE_APPBASE", applicationPath);

            var kreProcess = Process.Start(psi);
            kreProcess.BeginOutputReadLine();
            kreProcess.BeginErrorReadLine();
            kreProcess.OutputDataReceived += (sender, e) =>
            {
                if (verboseOutput)
                {
                    Console.WriteLine(e.Data);
                }
            };

            // Wait a little bit for it to conncet before firing the callback
            Thread.Sleep(1000);

            if (kreProcess.HasExited)
            {
                Console.WriteLine("Child process failed with {0}", kreProcess.ExitCode);
                return;
            }

            kreProcess.EnableRaisingEvents = true;
            kreProcess.Exited += (sender, e) =>
            {
                Console.WriteLine("Process crash trying again");

                Thread.Sleep(1000);

                StartRuntime(runtimePath, applicationPath, hostId, port, verboseOutput, onStart);
            };

            onStart();
        }

        private static Task ConnectAsync(Socket socket, IPEndPoint endPoint)
        {
            return Task.Factory.FromAsync((cb, state) => socket.BeginConnect(endPoint, cb, state), ar => socket.EndConnect(ar), null);
        }
    }
}
