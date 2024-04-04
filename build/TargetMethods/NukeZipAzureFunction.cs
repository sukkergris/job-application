using Nuke.Common.IO;
using System.IO;
using static ProjectPaths;
public static class NukeZipAzureFunction
{
	public static void Go(AbsolutePath publishDir)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(ZipDir));
		System.IO.Compression.ZipFile.CreateFromDirectory(publishDir, ZipDir);
	}
}
