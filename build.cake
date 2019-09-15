var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// Repository-specific variables for build tasks
var solution = "./ngit.sln";
var buildNumber = EnvironmentVariable("build_number") ?? "0.0.5";
var artifactsDir = "./artifacts";
var projectToPublish = "./NGit/NGit.csproj";

// Shared build tasks that hopefully should be copy-pastable
Information($"Running on TeamCity: {TeamCity.IsRunningOnTeamCity}");
Information($"Building: {buildNumber}");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
});


Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetCoreRestore();
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        // DotNetCoreTest(solution);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(solution, new DotNetCoreBuildSettings
        {
           NoIncremental = true,
           Configuration = configuration,
           ArgumentCustomization = args => args.Append($"/p:Version={buildNumber}")
        });
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
            {
                Configuration = configuration,
                OutputDirectory = artifactsDir
            });
    });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);