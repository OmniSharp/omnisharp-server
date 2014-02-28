using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using NDesk.Options;
using Nancy.Hosting.Self;
using OmniSharp.Solution;
using OmniSharp.Common;

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
            bool verbose = false;

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
                            "v|verbose", "Output debug information",
                            v => verbose = v != null
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

            StartServer(solutionPath, clientPathMode, port, verbose);
            
        }

        private static void StartServer(string solutionPath, string clientPathMode, int port, bool verbose)
        {
            try
            {
                Configuration.ConfigurationLoader.Load(clientPathMode);

                var solution = new CSharpSolution();
                Console.CancelKeyPress +=
                    (sender, e) =>
                        {
                            solution.Terminated = true;
                            Console.WriteLine("Ctrl-C pressed");
                            e.Cancel = true;
                        };

                var nancyHost = new NancyHost(new Bootstrapper(solution, new NativeFileSystem(), verbose), new HostConfiguration{RewriteLocalhost=false}, new Uri("http://localhost:" + port));

                nancyHost.Start();
                Console.WriteLine("OmniSharp server is listening");
                solution.LoadSolution(solutionPath.ApplyPathReplacementsForServer());
                Console.WriteLine("Solution has finished loading");
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
					Console.Error.WriteLine("Detected an OmniSharp instance already running on port " + port + ". Press a key.");
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
