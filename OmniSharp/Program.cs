using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using System.Threading;
using Nancy.Hosting.Self;
using NDesk.Options;
using OmniSharp.Common;
using OmniSharp.Solution;

namespace OmniSharp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            bool showHelp = false;
            string solutionPath = Directory.GetCurrentDirectory();
            string clientPathMode = null;

            // Determine the default location for the server side config.json file.
            // The user may override this with a command line option if they want.
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configLocation = Path.Combine(executableLocation, "config.json");

            var port = 2000;
            Verbosity verbosity = Verbosity.Debug;

            var options = new OptionSet
                    {
                        {
                            "s|solution=", "The path to the solution file",
                            s => solutionPath = s
                        },
                        {
                            "p|port=", "Port number to listen on",
                            (int p) => port = p
                        },
                        {
                            "c|client-path-mode=", "The path mode of the client (Cygwin, Windows or Unix)",
                            c => clientPathMode = c
                        },
                        {
                            "v|verbose=", "Output debug information (Quiet, Debug, Verbose)",
                            v => verbosity = v != null 
                                                ? (Verbosity)Enum.Parse(typeof(Verbosity), v)
                                                : Verbosity.Debug
                        },
                        {
                            "h|help", "Show this message and exit",
                            h => showHelp = h != null
                        },
                        {
                            "config", "The path to the server config.json file",
                            path => configLocation = path
                        }
                    };
           

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'omnisharp --help' for more information.");
                return;
            }

            showHelp |= solutionPath == null;

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            StartServer(solutionPath, clientPathMode, port, verbosity, configLocation);
            
        }


        static void StartServer(
            string solutionPath,
            string clientPathMode,
            int port,
            Verbosity verbosity,
            string configLocation)
        {
            
            var logger = new Logger(verbosity);
            try
            {
                Configuration.ConfigurationLoader.Load(
                        configLocation: configLocation, clientMode: clientPathMode);

                var solution = LoadSolution(solutionPath, logger);
                logger.Debug("Using solution path " + solutionPath);
                logger.Debug("Using config file " + configLocation);

                Console.CancelKeyPress +=
                    (sender, e) =>
                        {
                            solution.Terminate();
                            Console.WriteLine("Ctrl-C pressed");
                            e.Cancel = true;
                        };
                var nancyHost = new NancyHost(new Bootstrapper(
                                                solution, 
                                                new NativeFileSystem(), 
                                                logger), 
                                                new HostConfiguration{RewriteLocalhost=false}, 
                                                new Uri("http://localhost:" + port));

                nancyHost.Start();
                logger.Debug("OmniSharp server is listening");
                solution.LoadSolution();
                logger.Debug("Solution has finished loading");
                while (!solution.Terminated)
                {
                    Thread.Sleep(1000);
                }
                
                Console.WriteLine("Quit gracefully");
                nancyHost.Stop();
            }
            catch(Exception e)
            {
				if(e is SocketException || e is HttpListenerException)
				{
					logger.Error("Detected an OmniSharp instance already running on port " + port + ". Press a key.");
					Console.ReadKey();
					return;
				}
				throw;
            }
        }

        static ISolution LoadSolution(string solutionPath, Logger logger)
        {
            solutionPath = solutionPath.ApplyPathReplacementsForServer();
            ISolution solution;
            if (Directory.Exists(solutionPath))
            {
                var slnFiles = Directory.GetFiles(solutionPath, "*.sln");
                Console.WriteLine(slnFiles.Length);
                if (slnFiles.Length == 1)
                {
                    solutionPath = slnFiles[0];
                    logger.Debug("Found solution file - " + solutionPath);
                    solution = new CSharpSolution(solutionPath, logger);
                }
                else
                {
                    solution = new CSharpFolder(solutionPath, logger);
                }
            }
            else
            {
                solution = new CSharpSolution(solutionPath, logger);
            }
            return solution;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: omnisharp [-s /path/to/sln] [-p PortNumber]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
