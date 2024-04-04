using Nuke.Common.Tools.DotNet;

using static ProjectPaths;
using static BuildConfigurations;
public static class NukeRestore
{
	public static void Go() => DotNetTasks.DotNetRestore(settings => settings
		.SetProjectFile(SourceCodeDir / $"{heiselberg_mails}/{heiselberg_mails_csproj}")
		.SetRuntime(dotnetRuntime));
}
