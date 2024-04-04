using Nuke.Common.Tools.Npm;
using Serilog;

using static ProjectPaths;
public static class NukeBuildFrontend
{
	public static void Go()
	{
		Log.Debug($"Now compiling elm");
		NpmTasks.Npm("ci", workingDirectory: FrontEndDir);
		NpmTasks.Npm("run compile", workingDirectory: FrontEndDir);
	}
}
