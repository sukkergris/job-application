using Nuke.Common.IO;
using static ProjectPaths;
using static BuildConfigurations;
using Nuke.Common.Tools.DotNet;
public static class NukeCompile
{
	public static void Go(Configuration configuration, AbsolutePath publishDir)
	{
		DotNetTasks.DotNetPublish(settings => settings
						.SetProject(SourceCodeDir / $"{heiselberg_mails}/{heiselberg_mails_csproj}")
						.SetConfiguration(configuration)
						.EnableNoRestore() // We have already restored in 'GoRestore'
						.SetSelfContained(true)
						.SetRuntime(dotnetRuntime)
						.SetFramework(dotnetVersion)
						.SetOutput(publishDir)
						);
	}
}
