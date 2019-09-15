var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// Repository-specific variables for build tasks
var solution = "./ngit.sln";
var buildNumber = EnvironmentVariable("build_number") ?? "0.0.5";
var artifactsDir = "./artifacts";
var projectToPublish = "./NGit/NGit.csproj";
var feedzKey = EnvironmentVariable("NUGET_FEEDZ_API_KEY");

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

Task("Pack-NuGet")
    .IsDependentOn("Publish")
    .Does(() => {
        NuGetPack("./NGit.nuspec", new NuGetPackSettings {
            Properties = {
                {"version", buildNumber},
                {"csharpcodeVersionFromPackagesConfig", "1.2.0"}
            }
        });
    });

Task("Push-NuGet")
    .IsDependentOn("Pack-Nuget")
    .Does(() => {
        if (!String.IsNullOrEmpty (feedzKey)) {
            Information("Have a feedz key so pushing package");

            DotNetCoreNuGetPush($"./Gearset.NGit.{buildNumber}.nupkg", new DotNetCoreNuGetPushSettings {
                Source = "https://f.feedz.io/gearsethq/gearset-ngit/nuget",
                ApiKey = feedzKey
            });
        } else {
            Information("No Feedz key so skipping package push");
        }
    });

Task("Default")
    .IsDependentOn("Push-NuGet");

RunTarget(target);