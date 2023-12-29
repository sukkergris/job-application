using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace _build;

public record AzureStorageAccount(string Name);

public static class AzureBlobClientFactory
{
    public static BlobServiceClient Create(string storageAccountName,DefaultAzureCredential credential)
    {
        var serviceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
        return new BlobServiceClient(serviceUri,credential);
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
    /// <summary>
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <param name="blobName">Becomes the name of the blob</param>
    /// <returns></returns>
    public async Task Upload(string filePath,string blobName)
    {
        var blobClient = _webBlobContainerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(filePath); // #todo: Log response
    }
}
