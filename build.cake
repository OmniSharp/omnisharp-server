/// Arguments ///
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = "./OmniSharp.sln";
var releaseZip = "OmniSharp.zip";

// Define directories
var projectName = "OmniSharp";
var binDir = projectName + "/bin/";
var binReleaseDir = binDir + configuration;
var objDir = projectName + "/obj/";
var releaseDir = "dist";

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
                        binDir, objDir, releaseDir });
                DeleteFile(releaseZip);
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

Task("Copy-Files")
    .Description("Copy files to release directory")
    .IsDependentOn("Build")
    .Does(() =>
            {
                CreateDirectory(releaseDir);
                CopyFiles(binReleaseDir + "/*.dll", releaseDir);
                CopyFiles(binReleaseDir + "/*.xml", releaseDir);
                CopyFileToDirectory(binReleaseDir + "/config.json", releaseDir);
                CopyFileToDirectory(binReleaseDir + "/OmniSharp.exe", releaseDir);
                CopyFileToDirectory(binReleaseDir + "/OmniSharp.exe.config", releaseDir);
            });

Task("Zip")
    .Description("Zip up distribution files for a release")
    .Does(() =>
            {
                var releaseFiles = GetFiles(releaseDir + "/*");
                Zip("./", releaseZip, releaseFiles);
            });

Task("Default")
    .IsDependentOn("Build");

Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Files")
    .IsDependentOn("Zip");

/// <summary>
///   Task Execution. Without a RunTarget method call no tasks will run.
///   Here we assume the target is passed in via an argument or set to default.
/// </summary>
RunTarget(target);
