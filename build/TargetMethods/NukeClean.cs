using Nuke.Common.IO;
using Serilog;

using static ProjectPaths;
public static class NukeClean
{
	public static void Go()
	{
		Log.Information("Cleaning up source code directories"); //Nuke Build Telemetry: https://nuke.build/docs/fundamentals/logging/
		SourceCodeDir.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
		ArtifactsDir.CreateOrCleanDirectory();

	}
}
