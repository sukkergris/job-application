using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Nuke.Common.IO;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _build;

public record AzureStorageAccount(string Name);

public static class AzureBlobClientFactory
{
    public static BlobServiceClient Create(string storageAccountName,string accountKey)
    {
        var serviceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
        var storageSharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, accountKey);
        return new BlobServiceClient(serviceUri,storageSharedKeyCredential);
    }
    public static async Task< BlobContainerClient> GetWebBlobContainerClient( this BlobServiceClient bsc)
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
    public async Task SyncStaticWebsiteBaseFiles(AbsolutePath pathToBaseFiles)
    {
        var index_html = "index.html"; // #todo: Move to config - maybe
        await CreateOrOverwrite(pathToBaseFiles / index_html, index_html);

        var _404_html = "404.html"; // #todo: Move to config - maybe
        await CreateOrOverwrite(pathToBaseFiles / _404_html, _404_html);
    }

    public async Task SyncStaticWebsiteContentFiles(AbsolutePath frontendFiles)
    {
        Log.Debug("List existing blobs");
        var blobsInAzure = _webBlobContainerClient.GetBlobs();

        var filesInFrontendProject = Directory.GetDirectories(frontendFiles);

        
    }
    /// <summary>
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <param name="blobName">Becomes the name of the blob</param>
    /// <returns></returns>
    private async Task CreateOrOverwrite(AbsolutePath filePath, string blobName)
    {
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "text/html" };
        BlobRequestConditions overwrite = null;
        var blobClient = _webBlobContainerClient.GetBlobClient(blobName);
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        await blobClient.UploadAsync(content: stream, httpHeaders: blobHttpHeaders, conditions: overwrite);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="root"></param>
    /// <param name="path"></param>
    /// <param name="files">Key: Blobname, Value: Absolute file path</param>
    /// <returns></returns>
    private static IReadOnlyDictionary<string, string> GetFileNames(string root, string path, IReadOnlyDictionary<string, string> files)
    {
        Dictionary<string, string> filesInDirs = new Dictionary<string, string>(files);
        foreach (var dir in Directory.GetDirectories(path))
        {
            var filesInDir = GetFileNames(root, dir, filesInDirs);
            foreach (var file in filesInDir)
            {
                filesInDirs.Add(file.Key, file.Value);
            }
        }
        var filesInPath = Directory.GetFiles(path).Select(x => new KeyValuePair<string, string>(x.Remove(0, root.Length), x));
        foreach (var file in filesInPath)
        {
            filesInDirs.Add(file.Key, file.Value);
        }
        return filesInDirs;
    }
}
