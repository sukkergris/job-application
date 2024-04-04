using Nuke.Common.IO;

using static Nuke.Common.NukeBuild;
public static class ProjectPaths
{
	public static AbsolutePath IaC_Root_Dir => RootDirectory / "IaC";
	public static AbsolutePath SourceCodeDir => RootDirectory / "src";
	public static AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
	public static AbsolutePath ZipDir => ArtifactsDir / "app.zip";
	public static AbsolutePath FrontEndDir => SourceCodeDir / "Frontend-elm";
}
