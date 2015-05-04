/// Arguments ///
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = "./OmniSharp.sln";
// Define directories.
var projectName = "OmniSharp";
var binDir = projectName + "/bin/";
var objDir = projectName + "/obj/";

/// Setup / Teardown ///
Setup(() =>
    {
        Information("Building {0}", solution);
    });


/// Tasks ///
Task("Clean")
.Description("Cleans the build and output directories")
.Does(() =>
    {
        CleanDirectories(new DirectoryPath[] {
                binDir, objDir});
    });

Task("Restore")
.Description("Restore project nuget dependencies")
.IsDependentOn("Clean")
.Does(() =>
    {
        NuGetRestore(solution, new NuGetRestoreSettings {
                Source = new List<string> {
                    "https://www.nuget.org/api/v2/"
                },
                ToolPath = ".nuget/nuget.exe"
            });
    });

Task("Build")
.Description("Build the solution")
.IsDependentOn("Restore")
.Does(() =>
    {
        MSBuild(solution, settings =>
                settings.SetConfiguration(configuration)
                .SetNodeReuse(false));
    });

Task("Default")
.IsDependentOn("Build");

/// <summary>
///   Task Execution. Without a RunTarget method call no tasks will run
/// </summary>
RunTarget(target);
