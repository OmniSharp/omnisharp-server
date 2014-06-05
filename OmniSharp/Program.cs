using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            string solutionPath = null;
            string clientPathMode = null;

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
                            "h|help", "show this message and exit",
                            h => showHelp = h != null
                        },
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

            StartServer(solutionPath, clientPathMode, port, verbosity);
            
        }

        private static void StartServer(string solutionPath, string clientPathMode, int port, Verbosity verbosity)
        {
            
            var logger = new Logger(verbosity);
            try
            {
                Configuration.ConfigurationLoader.Load(clientPathMode);

                ISolution solution;
                if(Directory.Exists(solutionPath))
                {
                    solution = new CSharpFolder(logger);
                }
                else
                {
                    solution = new CSharpSolution(logger);
                }

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
                solution.LoadSolution(solutionPath.ApplyPathReplacementsForServer());
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

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: omnisharp -s /path/to/sln [-p PortNumber]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
