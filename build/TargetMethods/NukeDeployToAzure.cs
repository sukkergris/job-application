using static ProjectPaths;
using System.Threading.Tasks;

namespace _build;

public static class NukeDeployToAzure
{
	public static async Task Execute(AzureFunctionConfig config, AzureStorageAccount account)
	{

		await ZipDeploy.ThisArtifact(ZipDir).ToAzureFunctions(config);

		var webBlobContainerClient = await AzureBlobClientFactory
				.Create(account.Name, config.StorageAccountKey)
				.GetWebBlobContainerClient();

		await new AzureStaticWebsiteDeployment(webBlobContainerClient)
			.SyncStaticWebsiteContentFiles(FrontEndDir / "wwwroot");

	}
}
