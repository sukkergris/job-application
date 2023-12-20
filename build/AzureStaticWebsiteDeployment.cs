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
}
public class AzureStaticWebsiteDeployment
{
    readonly BlobServiceClient _blobServiceClient;
    readonly static string web = "$web";
    public AzureStaticWebsiteDeployment(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    public async Task Upload(string filePath,string fileName)
    {
        var containerClient =  _blobServiceClient.GetBlobContainerClient(web);
        var exists = await containerClient.ExistsAsync();
        if (!exists)
        {
            throw new Exception($"The resource {web} should have been created using 'Pulumi'");
        }
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(filePath);
    }
}
