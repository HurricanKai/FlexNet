#addin "Cake.Incubator"
#tool "nuget:?package=NUnit.ConsoleRunner"

var target = Argument("target", "default");
var configuration = Argument("configuration", "release");

var solution = File("./FlexNet.sln");

Task("buildLibary").Does(() =>
{
     var settings = new DotNetCoreBuildSettings
     {
         Framework = "netstandard2.0",
         Configuration = "Release",
         OutputDirectory = "./artifacts/"
     };

	 DotNetCoreBuild("./FlexNet.Core/FlexNet.Core.csproj", settings);
	 DotNetCoreBuild("./FlexNet.Core.DefaultAccessors/FlexNet.Core.DefaultAccessors.csproj", settings);
	 DotNetCoreBuild("./Templates/SimpleTCP/FlexNet.Templates.SimpleTCP.csproj", settings);
	 DotNetCoreBuild("./Builders/ExpressionDelegateBuilder/FlexNet.Builders.ExpressionDelegateBuilder.csproj", settings);
});

Task("buildTests")
  .IsDependentOn("buildLibary")
  .Does(() =>
  {
    var settings = new DotNetCoreBuildSettings
	{
		Framework = "netcoreapp2.1",
		Configuration = "Release",
		OutputDirectory = "./artifacts/"
	};

	 DotNetCoreBuild("./FlexNet.Core.Tests/FlexNet.Core.Tests.csproj", settings);
	 DotNetCoreBuild("./FlexNet.Core.DefaultAccessors.Tests/FlexNet.Core.DefaultAccessors.Tests.csproj", settings);
	 DotNetCoreBuild("./Builders/ExpressionDelegateBuilder.Tests/FlexNet.Builders.ExpressionDelegateBuilder.Tests.csproj", settings);
  });

Task("testLibary")
  .IsDependentOn("buildLibary")
  .IsDependentOn("buildTests")
  .Does(() =>
  {
     NUnit3(new [] { 
		"./artifacts/FlexNet.Core.Tests.dll",
		"./artifacts/FlexNet.Core.DefaultAccessors.Tests.dll",
		"./artifacts/FlexNet.Builders.ExpressionDelegateBuilder.Tests.dll",
	 });
  });

Task("clean").Does(() =>
{
	DeleteDirectory("./artifacts/", new DeleteDirectorySettings {
		Recursive = true,
		Force = true
	});
});


Task("default")
  .IsDependentOn("clean")
  .IsDependentOn("buildLibary"); // Tests not rn, use VS

RunTarget(target);