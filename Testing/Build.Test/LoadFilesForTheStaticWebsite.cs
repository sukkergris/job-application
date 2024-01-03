using _build;
using Microsoft.Extensions.Configuration;

namespace Build.Test;

public class LoadFilesForTheStaticWebsite
{
    [Fact]
    public async Task Test()
    {
        var confiBuilder = new ConfigurationBuilder();

        confiBuilder
            .AddEnvironmentVariables()
            .AddJsonFile("integrationtest.json", optional: false);
        var config = confiBuilder.Build();

        var storageAccountName = config.GetSection("AzureBlob:StorageAccountName").Value;
        var storageAccountKey1 = config.GetSection("AzureBlob:StorageAccountKey1").Value;

        var webBlobContainerClient = await AzureBlobClientFactory
           .Create(storageAccountName, storageAccountKey1)
           .GetWebBlobContainerClient();

        var staticWebsite = new AzureStaticWebsiteDeployment(webBlobContainerClient);

        await staticWebsite.SyncStaticWebsiteContentFiles(@"C:\private\private-job-application-app\src\Frontend-elm\wwwroot");
    }
}
