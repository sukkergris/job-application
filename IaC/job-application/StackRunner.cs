using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web.V20230101.Inputs;
using Pulumi.AzureNative.Web.V20230101;
using Pulumi;
using System.Collections.Generic;
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

		var storageAccount = new StorageAccount(storageAccountName, new StorageAccountArgs
		{
			ResourceGroupName = resourceGroup.Name,
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


		var appServicePlanName = $"{application}-{environment}-";
		var appServicePlan = new AppServicePlan(appServicePlanName, new AppServicePlanArgs
		{
			ResourceGroupName = resourceGroup.Name,
			// Run on Linux
			Kind = "Linux",
			// Consumption plan SKU
			Sku = new SkuDescriptionArgs
			{
				Tier = "Dynamic",
				Name = "Y1" // Pay as you go
			},
			// For Linux, you need to change the plan to have Reserved = true property.
			Reserved = true
		});

		#region ZombieCode


		//var functionAppName = $"{application}-{environment}-";
		//var linuxFA = new WebApp(functionAppName, new WebAppArgs {
		//   Kind = "FunctionApp"
		//   , ResourceGroupName = resourceGroup.Name
		//   , ServerFarmId = appServicePlan.Id
		//   , Location = resourceGroup.Location
		//   , SiteConfig = new SiteConfigArgs {
		//      AppSettings = new NameValuePairArgs[] {
		//         new NameValuePairArgs
		//         {
		//            Name = ""
		//            , Value = Getconnections
		//         }
		//      }
		//   }
		//});

		// var linuxFunctionApp = new LinuxFunctionApp(functionAppName, new()
		// {
		//     ResourceGroupName = resourceGroup.Name,
		//     Location = resourceGroup.Location,
		//     StorageAccountName = storageAccount.Name,
		//     StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
		//     ServicePlanId = appServicePlan.Id,
		//     AuthSettings = new LinuxFunctionAppAuthSettingsArgs
		//     {
		//         Enabled = false,
		//         UnauthenticatedClientAction = "AllowAnonymous",
		//         TokenStoreEnabled = false
		//     },
		//     SiteConfig = new LinuxFunctionAppSiteConfigArgs
		//     {

		//     }
		// });
		#endregion
		// https://www.pulumi.com/templates/static-website/azure/
		// https://github.com/pulumi/templates/blob/master/static-website-azure-csharp/Program.cs
		var staticWebsiteName = $"sa-sw-{application}-{environment}";
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


		var storageAccountConnectionString = GetStorageAccountConnectionString(resourceGroup.Name, storageAccount.Name);

		// Add function app
		var functionAppName = $"{application}-{environment}-";
		var linuxFunctionApp = new WebApp(functionAppName, new WebAppArgs
		{
			Kind = "FunctionApp",
			ResourceGroupName = resourceGroup.Name,
			ServerFarmId = appServicePlan.Id,
			Location = resourceGroup.Location,
			SiteConfig = new SiteConfigArgs
			{
				LinuxFxVersion = "DOTNET|6.0",

				AppSettings = new NameValuePairArgs[] {
				new NameValuePairArgs
				{
					Name = "FUNCTIONS_EXTENSION_VERSION"
					, Value ="~4"
				},
				new NameValuePairArgs {
					 Name = "FUNCTIONS_WORKER_RUNTIME"
					 , Value = "dotnet-isolated"
				},
				new NameValuePairArgs{
					 Name = "AzureWebJobsStorage"
					 , Value = storageAccountConnectionString
				},
				new NameValuePairArgs{
					 Name="SCM_DO_BUILD_DURING_DEPLOYMENT"
					 , Value = "0"
				},
				new NameValuePairArgs {
					 Name = "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
					 Value = storageAccountConnectionString
				},
				new NameValuePairArgs {
					 Name = "WEBSITE_CONTENTSHARE",
					 Value = appServicePlan.Name//application
				},
				new NameValuePairArgs {
					 Name = "WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED",
					 Value = "1"
				}
				}
			}
		});


		var back = new Dictionary<string, object?>
		{
			["ResourceGroupId"] = resourceGroup.Id,
			["ResourceGroupName"] = resourceGroup.Name,
			["LinuxFunctionAppId"] = linuxFunctionApp.Id,
			["LinuxFunctionAppName"] = linuxFunctionApp.Name,
			["StorageAccountName"] = storageAccount.Name,
			["StorageAccountKey"] = storageAccountConnectionString // storageAccount.PrimaryAccessKey,
																   //["StorageAccountDefaultEndpoint"] = storageAccount.PrimaryWebEndpoint
		};


		#region Assign output

		ResourceGroupId = resourceGroup.Id;
		ResourceGroupName = resourceGroup.Name;
		LinuxFunctionAppId = linuxFunctionApp.Id;
		LinuxFunctionAppName = linuxFunctionApp.Name;
		StorageAccountName = storageAccount.Name;
		StorageAccountKey = storageAccountConnectionString;

		#endregion
	}
	[Output] public Output<string> ResourceGroupName { get; set; }
	[Output] public Output<string> ResourceGroupId { get; set; }
	[Output] public Output<string> LinuxFunctionAppId { get; set; }
	[Output] public Output<string> LinuxFunctionAppName { get; set; }
	[Output] public Output<string> StorageAccountName { get; set; }
	[Output] public Output<string> StorageAccountKey { get; set; }

	private static Output<string> GetStorageAccountConnectionString(Input<string> resourceGroupName, Input<string> accountName)
	{
		// Retrieve the primary storage account key.
		var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
		{
			ResourceGroupName = resourceGroupName,
			AccountName = accountName
		});

		return storageAccountKeys.Apply(keys =>
		{
			var primaryStorageKey = keys.Keys[0].Value;

			// Build the connection string to the storage account.
			return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey}");
		});
	}
}

