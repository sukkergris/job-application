using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using SA = Pulumi.AzureNative.Storage.StorageAccount;
namespace Infrastructure;

internal static class StorageAccount
{
	/// <summary>
	/// use name from recently created resource group object eg. resourceGroup.Name.
	/// Don't use hardcoded values - unless your know what you are doing
	/// </summary>
	/// <param name="resourceGroupName"></param>
	/// <returns></returns>
	public static SA Create(Output<string> resourceGroupName, string storageAccountName)
	// https://www.pulumi.com/registry/packages/azure-native/api-docs/storage/storageaccount/
	=> new SA(storageAccountName, new StorageAccountArgs
	{
		ResourceGroupName = resourceGroupName,
		AllowBlobPublicAccess = true,
		AccessTier = AccessTier.Cool,
		EnableHttpsTrafficOnly = true,
		MinimumTlsVersion = MinimumTlsVersion.TLS1_2,
		Sku = new SkuArgs
		{
			Name = SkuName.Standard_LRS, // Locally redundant
		},
		Kind = Kind.StorageV2
	});
}

