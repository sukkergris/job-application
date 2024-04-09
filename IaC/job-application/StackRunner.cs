using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi;
using System.Linq;
public class StackRunner : Stack
{
	public StackRunner()
	{
		// Static project specific configuirations
		var rg = "rg";
		var application = "heisconform"; // heiselberg-contact-form

		// Stack specific configurations
		var azureConfig = new Pulumi.Config("azure"); // Pulumi.{stack}.yaml
		var azLocation = azureConfig.Require("location");
		var environment = Pulumi.Deployment.Instance.StackName; // #TODO: Figure out the difference -> 'Deployment' is an ambiguous reference between 'Pulumi.Deployment' and 'Pulumi.AzureNative.Resources.Deployment'

		var resourceGroupName = $"{rg}-{application}-{environment}-{azLocation}-";

		var resourceGroup = new ResourceGroup(resourceGroupName, new() { Location = azLocation });
		var sa = "sa"; // Storage Account
		var storageAccountName = $"{sa}{application}{environment}".Trim(); //  Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only.
																								 // https://www.pulumi.com/registry/packages/azure-native/api-docs/storage/storageaccount/

		var storageAccount = Infrastructure.StorageAccount.Create(resourceGroup.Name, storageAccountName);
		var appServicePlanName = $"{application}-{environment}-";
		var appServicePlan = Infrastructure.AppServicePlan.Create(resourceGroup.Name, appServicePlanName);

		// https://www.pulumi.com/templates/static-website/azure/
		// https://github.com/pulumi/templates/blob/master/static-website-azure-csharp/Program.cs
		// var staticWebsiteName = $"sa-sw-{application}-{environment}";
		var website = new StorageAccountStaticWebsite(application, new()
		{
			ResourceGroupName = resourceGroup.Name,
			AccountName = storageAccount.Name,
			Error404Document = "index.html",
			IndexDocument = "index.html"
		});

		var cdnProfile = new Pulumi.AzureNative.Cdn.Profile($"{application}-", new Pulumi.AzureNative.Cdn.ProfileArgs
		{
			Location = resourceGroup.Location,
			ProfileName = $"cdn-heis-sw",
			ResourceGroupName = resourceGroup.Name,
			Sku = new Pulumi.AzureNative.Cdn.Inputs.SkuArgs
			{
				Name = "Standard_Microsoft" // https://www.pulumi.com/registry/packages/azure-native/api-docs/cdn/profile/#sku
			},
		});

		var storageAccountKeys = GetStorageAccountKeys(resourceGroup.Name, storageAccount.Name);
		var typeSafeStorageAccountKeys = storageAccountKeys.AsTypeSafe();

		var storageAccountConnectionString1 = GenerateStorageAccountConnectionString1(storageAccount.Name, typeSafeStorageAccountKeys);
		// Add function app
		var functionAppName = $"{application}-{environment}-";
		var linuxFunctionApp = Infrastructure.WebApp.Create(
			resourceGroup.Name
			, appServicePlan.Id
			, resourceGroup.Location
			, storageAccountConnectionString1
			, appServicePlan.Name
			, functionAppName);

		#region Assign output

		LinuxFunctionAppId = linuxFunctionApp.Id;
		LinuxFunctionAppName = linuxFunctionApp.Name;
		ResourceGroupId = resourceGroup.Id;
		ResourceGroupName = resourceGroup.Name;
		StorageAccountKey1 = typeSafeStorageAccountKeys.Apply(x => x.Key1.Key1);
		StorageAccountName = storageAccount.Name;

		#endregion
	}
	[Output] public Output<string> ResourceGroupName { get; set; }
	[Output] public Output<string> ResourceGroupId { get; set; }
	[Output] public Output<string> LinuxFunctionAppId { get; set; }
	[Output] public Output<string> LinuxFunctionAppName { get; set; }
	[Output] public Output<string> StorageAccountName { get; set; }
	[Output] public Output<string> StorageAccountKey1 { get; set; }

	private static Output<ListStorageAccountKeysResult> GetStorageAccountKeys(Input<string> resourceGroupName, Input<string> accountName)
	{
		// Retrieve the primary storage account key.
		Output<ListStorageAccountKeysResult> storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
		{
			ResourceGroupName = resourceGroupName,
			AccountName = accountName
		});
		return storageAccountKeys;
	}
	private static Output<string> GenerateStorageAccountConnectionString1(Input<string> accountName, Output<StorageAccessKeys> storageAccountKeys) =>
		storageAccountKeys.Apply(keys =>
		{
			// Build the connection string to the storage account.
			return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={keys.Key1.Key1}");
		});

}
public record StorageAccessKey(string Key1, string creationTime, string permissions);
public record StorageAccessKeys(StorageAccessKey Key1, StorageAccessKey Key2);
public static class AzureOutputExtensions
{
	public static Output<StorageAccessKeys> AsTypeSafe(this Output<ListStorageAccountKeysResult> output) =>
		output.Apply(keys =>
		{
			var listOfKeys = keys.Keys;
			var length = listOfKeys.Length;
			if (length != 2)
			{
				var errorMessage = $"Found {length} keys. Expected 2";
				Log.Error(errorMessage);
				throw new System.Exception(errorMessage);
			}

			var key1 = listOfKeys.First(key => string.Equals(key.KeyName, "key1", System.StringComparison.OrdinalIgnoreCase));

			var key2 = listOfKeys.First(key => string.Equals(key.KeyName, "key2", System.StringComparison.OrdinalIgnoreCase));

			return new StorageAccessKeys(
				new StorageAccessKey(key1.Value, key1.CreationTime, key1.Permissions)
				, new StorageAccessKey(key2.Value, key2.CreationTime, key2.Permissions));
		});
}

