#tool nuget:?package=NuGet.CommandLine&version=6.2.1
#tool nuget:?package=GitVersion.Tool&version=5.10.3

var target = Argument("target", "Build-Test-Package");
var gitHubToken = Argument("gitHubToken", EnvironmentVariable("GITHUB_TOKEN") ?? null);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    IEnumerable<DirectoryPath> binDirectories;
    IEnumerable<DirectoryPath> objDirectories;

    try
    {
        DotNetClean("../AzureFunctionsOpenIDConnectAuthSample.sln");
    }
    catch { }

    binDirectories = GetDirectories("../**/bin").Where(d => !d.FullPath.Contains("node_modules"));
    CleanDirectories(binDirectories);

    objDirectories = GetDirectories("../**/obj").Where(d => !d.FullPath.Contains("node_modules"));
    CleanDirectories(objDirectories);
});

Task("Build")
    .Does(() =>
{
    GitVersion(new GitVersionSettings()
    {
        ArgumentCustomization = args => args.Prepend("/updateprojectfiles"),
        WorkingDirectory = ".."
    });

    DotNetBuild("../AzureFunctionsOpenIDConnectAuthSample.sln");
});

Task("Test")
    .Does(() =>
{
    DotNetTest("../AzureFunctionsOpenIDConnectAuthSample.sln", new DotNetTestSettings
    {
        NoBuild = true,
    });
});

Task("Package")
    .Does(() =>
{
    DotNetPack("../OidcApiAuthorization/OidcApiAuthorization.csproj");
});

Task("Push")
    .Does(() =>
{
    var settings = new DotNetNuGetPushSettings
    {
        Source = "https://nuget.pkg.github.com/zbeer/index.json",
        ApiKey = gitHubToken
    };

    var packageFilePath = GetFiles("..\\OidcApiAuthorization\\bin\\**\\*.nupkg").Single();

    DotNetNuGetPush(packageFilePath, settings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Build-Test-Package")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);