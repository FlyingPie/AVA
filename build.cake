#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=vswhere&version=2.6.7"

var configuration = Argument("configuration", "Release");
var output = Argument("output", "bin");
var platform = Argument("platform", PlatformTarget.x64);

var sln = "src/Ava.sln";

Task("Default").Does(() =>
{
	// CLEAN //
	CleanDirectory(output);

	// BUILD //
	MSBuild(sln, new MSBuildSettings
	{
		Configuration = configuration,
		MaxCpuCount = 0, // As many as available
		NodeReuse = false, // Required to prevent build task dll's from being locked
		PlatformTarget = platform,
		Restore = true,
		ToolPath = GetFiles(VSWhereLatest() + "/**/MSBuild.exe").FirstOrDefault()
	}.WithProperty("runtime", "win10-x64"));

	// TEST //
	VSTest("./bin/**/*.UnitTest.dll", new VSTestSettings
	{
		ToolPath = GetFiles(VSWhereLatest() + "/**/vstest.console.exe").FirstOrDefault()
	});

	// CLEANUP //
	DeleteFiles($"{output}/**/*.pdb");
	DeleteFiles($"{output}/**/*.winmd");
	DeleteFiles($"{output}/**/*.xml");

	DeleteFiles($"{output}/AVA/settings.json");

	System.IO.Directory.Delete($"{output}/AVA.Core", true);

	System.IO.Directory.Delete($"{output}/MUI", true);

	System.IO.Directory.Delete($"{output}/obj", true);
});

RunTarget("Default");
