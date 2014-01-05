using System;
using System.Diagnostics;
using System.IO;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.TinyIoc;
using MonoDevelop.Projects;
using OmniSharp.Common;
using OmniSharp.ProjectManipulation.AddReference;
using OmniSharp.Solution;

namespace OmniSharp
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly ISolution _solution;
        private readonly bool _verbose;

		readonly IFileSystem _fileSystem;

        public Bootstrapper(ISolution solution, IFileSystem fileSystem, bool verbose)
        {
			_fileSystem = fileSystem;
            _solution = solution;
            _verbose = verbose;
            JsonSettings.MaxJsonLength = int.MaxValue;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            if (PlatformService.IsUnix)
            {
                HelpService.AsyncInitialize();
            }
            
            pipelines.BeforeRequest.AddItemToStartOfPipeline(StopWatchStart);
            pipelines.AfterRequest.AddItemToEndOfPipeline(StopWatchStop);

            if (_verbose)
            {
                pipelines.BeforeRequest.AddItemToStartOfPipeline(LogRequest);
                pipelines.AfterRequest.AddItemToStartOfPipeline(LogResponse);
            }

            pipelines.OnError.AddItemToEndOfPipeline((ctx, ex) =>
                {
                    Console.WriteLine(ex);
                    return null;
                });
        }

        private Response LogRequest(NancyContext ctx)
        {
            Console.WriteLine("****** Request ******");
            var form = ctx.Request.Form;
            foreach (var field in form)
            {
                Console.WriteLine(field + " = " + form[field]);
            }
            return null;
        }

        private void LogResponse(NancyContext ctx)
        {
            Console.WriteLine("****** Response ******");

            var stream = new MemoryStream();
            ctx.Response.Contents.Invoke(stream);

            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                Console.WriteLine(content);
            }
        }

        private Response StopWatchStart(NancyContext ctx)
        {
            var stopwatch = new Stopwatch();
            ctx.Items["stopwatch"] = stopwatch;
            stopwatch.Start();
            return null;
        }

        private void StopWatchStop(NancyContext ctx)
        {
            var stopwatch = (Stopwatch) ctx.Items["stopwatch"];
            stopwatch.Stop();
            Console.WriteLine(ctx.Request.Path + " " + stopwatch.ElapsedMilliseconds + "ms");
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
			container.Register(_solution);
			container.Register(_fileSystem);
			container.RegisterMultiple<IReferenceProcessor>(new []{typeof(AddProjectReferenceProcessor), typeof(AddFileReferenceProcessor), typeof(AddGacReferenceProcessor)});			
        }
    }
}
