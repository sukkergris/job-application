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
        var startingPoint =new Dictionary<BlobName, File>();
        var localFrontendFiles = GetFileNames(root: frontendFilesPath, path: frontendFilesPath, startingPoint);

        var blobsToDelete = blobsInAzure.Where(blob => !localFrontendFiles.Any(pair => string.Equals( pair.Key.name.NormalizePath(),blob.Name.NormalizePath(),StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (var blob in blobsToDelete)
        {
            Log.Debug($"Deleting blob: {blob.Name}");
            await _webBlobContainerClient.DeleteBlobIfExistsAsync(blob.Name);
        }

        foreach (var (blob,file) in localFrontendFiles.Where( pair => !pair.Key.name.Contains("_doc_")))
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
    private async Task CreateOrOverwrite(AbsolutePath filePath, BlobName blobName)
    {
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "text/html", CacheControl = "no-store" };
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
    /// <param name="files">Key: Blobname, Value: Absolute file path</param>
    /// <returns>Key: Blobname, Value: Absolute file path</returns>
    public static IReadOnlyDictionary<BlobName, File> GetFileNames(AbsolutePath root, AbsolutePath path, IReadOnlyDictionary<BlobName, File> files)
    {
        // Create starting point for this iteration. Adding preveiously found files to the list.
        Dictionary<BlobName, File> filesInDirs = new Dictionary<BlobName, File>(files);
        
        // Now diving into every folder found in this 'path'
        foreach (var dir in Directory.GetDirectories(path))
        {
            // Finding files in assending folder
            IReadOnlyDictionary<BlobName, File> filesInDir = GetFileNames(root, dir, filesInDirs);

            foreach (var file in filesInDir)
            {
                filesInDirs.Add(file.Key, file.Value);
            }
        }

        // Finally adding files found in the current 'path'
        var filesInPath = Directory.GetFiles(path)
            .Select(path => new KeyValuePair<BlobName, File>(new BlobName(FilePathToBlobName(root,path)), new File(path)));

        foreach (var file in filesInPath)
        {
            filesInDirs.Add(file.Key, file.Value);
        }

        return filesInDirs;
    }

    // Since the path contains the absolute value of the file path (eg. c://hello/world.txt) we must remove some part before exposing the files found in wwwroot.
    public static string FilePathToBlobName(string root, string path) => path.Remove(0, root.Length + 1); // remove starting char '/' in the blobname
}
