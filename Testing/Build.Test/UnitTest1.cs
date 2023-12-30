using Azure.Core;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;

namespace Build.Test;

public class UnitTest1
{
    private static readonly string[] scopes = new[] { "https://management.azure.com/.default" };

    [Fact]
    public async Task Test1()
    {
        var defaultAzureCredential = new DefaultAzureCredential();

        var tokenRequestContext = new TokenRequestContext(scopes);
        var azAccessToken = await defaultAzureCredential.GetTokenAsync(tokenRequestContext);
        var storageAccountName = "saheisconformdevadc76bfe";
        var serviceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
        //var bsc = new BlobServiceClient(serviceUri, defaultAzureCredential);

        var sskc = new StorageSharedKeyCredential(storageAccountName, "gaN1wDO5ueqpxiOTwefxpyuHsPYkCbJ64TWuITdNZPk5bEvhffRSLIZ26ik2o4+zlaSy2/AkfSUf+AStC7r80A==");
        var bsc = new BlobServiceClient(serviceUri, sskc);
        var web = "$web";
        var webContainerClient = bsc.GetBlobContainerClient(web);
        var exists = await webContainerClient.ExistsAsync();
        if (!exists)
        {
            throw new Exception($"The resource {web} should have been created using 'Pulumi'");
        }

        var items = webContainerClient.GetBlobs();
        var names = string.Join(", ", items.Select(x=>x.Name));
        //Console.WriteLine(string.Join(",",items));

        var blobClient = webContainerClient.GetBlobClient("index.html");

        var index_html = "C:/private/private-job-application-app/src/AzureStaticWebsite/index.html";

        await blobClient.UploadAsync(index_html);


    }
}