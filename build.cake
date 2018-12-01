#tool "nuget:?package=GitReleaseNotes"
#tool "nuget:?package=GitVersion.CommandLine"
#addin "nuget:?package=Cake.Incubator"

var target = Argument("target", "Default");
var outputDir = "./artifacts/";

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(".");
    });

GitVersion versionInfo = null;
Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });        
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild("./FlexNet.Core/FlexNet.Core.csproj");
		DotNetCoreBuild("./FlexNet.Core.DefaultAccessors/FlexNet.Core.DefaultAccessors.csproj");
		DotNetCoreBuild("./Templates/SimpleTCP/FlexNet.Templates.SimpleTCP.csproj");
		DotNetCoreBuild("./Builders/ExpressionDelegateBuilder/FlexNet.Builders.ExpressionDelegateBuilder.csproj");
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest("./FlexNet.Core.Tests/FlexNet.Core.Tests.csproj");
		DotNetCoreTest("./Builders/ExpressionDelegateBuilder.Tests/FlexNet.Builders.ExpressionDelegateBuilder.Tests.csproj");
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            ArgumentCustomization = args=> args.Append(" --include-symbols /p:PackageVersion=" + versionInfo.NuGetVersion),
            OutputDirectory = outputDir,
            NoBuild = true
        };

        DotNetCorePack("./FlexNet.Core/FlexNet.Core.csproj", settings);
		DotNetCorePack("./FlexNet.Core.DefaultAccessors/FlexNet.Core.DefaultAccessors.csproj", settings);
		DotNetCorePack("./Templates/SimpleTCP/FlexNet.Templates.SimpleTCP.csproj", settings);
		DotNetCorePack("./Builders/ExpressionDelegateBuilder/FlexNet.Builders.ExpressionDelegateBuilder.csproj", settings);

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*"))
                AppVeyor.UploadArtifact(file.FullPath);
        }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);