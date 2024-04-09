using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Nuke.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Bot.Builder.LanguageGeneration;
using System.Threading.Tasks;
using Serilog;
using System.Text.RegularExpressions;
using OneOf;
using OneOf.Types;

namespace _build;

public record AzureStorageAccount(string Name);
public record BlobName(string name);
public record File(string path);
public static class AzureBlobClientFactory
{
	public static BlobServiceClient Create(string storageAccountName, string accountKey)
	{
		var serviceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
		var storageSharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, accountKey);
		return new BlobServiceClient(serviceUri, storageSharedKeyCredential);
	}
	public static async Task<BlobContainerClient> GetWebBlobContainerClient(this BlobServiceClient bsc)
	{
		var web = "$web";
		var webContainerClient = bsc.GetBlobContainerClient(web);
		var exists = await webContainerClient.ExistsAsync();
		if (!exists)
		{
			throw new Exception($"The resource {web} should have been created using 'Pulumi'");
		}
		return webContainerClient;
	}
}
public class AzureStaticWebsiteDeployment
{
	readonly BlobContainerClient _webBlobContainerClient;
	public AzureStaticWebsiteDeployment(BlobContainerClient webBlobContainerClient)
	{
		_webBlobContainerClient = webBlobContainerClient;
	}

	public async Task SyncStaticWebsiteContentFiles(AbsolutePath frontendFilesPath)
	{
		var blobsInAzure = _webBlobContainerClient.GetBlobs()
			 .ToList();

		var localFrontendFiles = GetFileNames(root: frontendFilesPath, path: frontendFilesPath);

		var blobsToDelete = blobsInAzure.Where(blob => !localFrontendFiles.Any(pair => string.Equals(pair.Key.name.NormalizePath(), blob.Name.NormalizePath(), StringComparison.OrdinalIgnoreCase)))
			 .ToList();

		foreach (var blob in blobsToDelete)
		{
			Log.Debug($"Deleting blob: {blob.Name}");
			await _webBlobContainerClient.DeleteBlobIfExistsAsync(blob.Name);
		}

		foreach (var (blob, file) in localFrontendFiles.Where(pair => !pair.Key.name.Contains("_doc_")))
		{
			Log.Debug($"Create or overwrite blob: {blob.name} found here: {file.path}");
			await CreateOrOverwrite(file.path, blob);
		}

	}
	/// <summary>
	/// </summary>
	/// <param name="filePath">Path to file</param>
	/// <param name="blobName">Becomes the name of the blob</param>
	/// <returns></returns>
	private Task CreateOrOverwrite(AbsolutePath filePath, BlobName blobName)
	{
		var fileTypeResult = GetBlobHttpHeadersFrom(filePath);

		var headerResult = fileTypeResult.Match(
		  error => { Log.Error(error.message); return new Result<BlobHttpHeaders>(); },
		  unSupportedFileType => { Log.Error($"Unsupported filetype{unSupportedFileType.fileType}"); return new Result<BlobHttpHeaders>(); },
		  blobHttpHeaders => new Result<BlobHttpHeaders>(blobHttpHeaders));

		if (headerResult.Value is null)
		{
			return Task.CompletedTask;
		}
		return CreateOrOverwrite(filePath, headerResult.Value, blobName);
	}

	private static BlobHttpHeaders CreateBlobHttpHeaders(string contentType, string cacheControl) => new BlobHttpHeaders { ContentType = contentType, CacheControl = cacheControl };
	public async Task CreateOrOverwrite(AbsolutePath filePath, BlobHttpHeaders blobHttpHeaders, BlobName blobName)
	{
		BlobRequestConditions overwrite = null;
		var blobClient = _webBlobContainerClient.GetBlobClient(blobName.name);
		var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		await blobClient.UploadAsync(content: stream, httpHeaders: blobHttpHeaders, conditions: overwrite);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="root"></param>
	/// <param name="path"></param>
	/// <param name="existingFiles">Key: Blobname, Value: Absolute file path</param>
	/// <returns>Key: Blobname, Value: Absolute file path</returns>
	public static IReadOnlyDictionary<BlobName, File> GetFileNames(AbsolutePath root, AbsolutePath path)
	{

		var directlyAscendingFolders = Directory.GetDirectories(path);
		// Adding files found in the directly ascending folders
		var filesInDirectlyAscendingFolders = directlyAscendingFolders
			 .SelectMany(dir => GetFileNames(root, dir))
			 .ToDictionary(x => x.Key, x => x.Value);

		// Adding files found in the current path
		var filesInPath = Directory.GetFiles(path)
			 .Select(filePath => new KeyValuePair<BlobName, File>(new BlobName(FilePathToBlobName(root, filePath)), new File(filePath)))
			 .ToDictionary(x => x.Key, x => x.Value);

		// Merging dictionaries
		var mergedFiles = filesInPath.Concat(filesInDirectlyAscendingFolders).ToDictionary(x => x.Key, x => x.Value);

		return mergedFiles;
	}

	// Since the path contains the absolute value of the file path (eg. c://hello/world.txt) we must remove some part before exposing the files found in wwwroot.
	public static string FilePathToBlobName(string root, string path) => path.Remove(0, root.Length + 1); // remove starting char '/' in the blobname
	public static OneOf<Error, UnSupportedFileType, BlobHttpHeaders> GetBlobHttpHeadersFrom(string absolutePath)
	{
		// Use regex to match the file extension
		Regex regex = new Regex(@"\.([^.]+)$");

		Match match = regex.Match(absolutePath);

		// Check if a match is found
		if (match.Success)
		{
			var fileExtension = match.Groups[1].Value;
			return fileExtension switch
			{
				"html" => CreateBlobHttpHeaders("text/html", "no-store"),
				"css" => CreateBlobHttpHeaders("text/css", $"max-age={24 * 60 * 60}"),
				"js" => CreateBlobHttpHeaders("text/javascript", $"max-age={24 * 60 * 60}"),
				"png" => CreateBlobHttpHeaders("image/png", $"max-age={24 * 60 * 60}"),
				"ico" => CreateBlobHttpHeaders("image/x-icon", $"max-age={24 * 60 * 60}"),
				"svg" => CreateBlobHttpHeaders("image/svg+xml", $"max-age={24 * 60 * 60}"),
				"jpg" => CreateBlobHttpHeaders("image/jpeg", $"max-age={7 * 24 * 60 * 60}"),
				//"json" => new JsonFile(new FileType("json", "application/json", "no-store")),
				"txt" => CreateBlobHttpHeaders("text/plain", "no-store"),
				//"woff" => new WoffFile(new FileType("woff", "font/woff", "no-store")),
				//"woff2" => new Woff2(new FileType("woff2", "font/woff2", "no-store")),
				"ttf" => CreateBlobHttpHeaders("font/ttf", "public, max-age=31536000"),
				//"eot" => new EotFile(new FileType("eot", "application/vnd.ms-fontobject", "no-store")),
				//"otf" => new OtfFile(new FileType("otf", "font/otf", "no-store")),
				//"map" => new MapFile(new FileType("map", "application/json", "no-store")),
				_ => new UnSupportedFileType($"FileExtension: {fileExtension}"),
			};
		}
		else
		{
			return new Error($"Somehow the we couldn't make out what to do with the file (We couldn't find the extension): {absolutePath}");
		}
	}
}

public record Error(string message);
public record UnSupportedFileType(string fileType);
