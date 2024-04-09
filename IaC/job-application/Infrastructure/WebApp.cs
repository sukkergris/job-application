using Pulumi.AzureNative.Web.V20230101;
using Pulumi.AzureNative.Web.V20230101.Inputs;
using Pulumi;
using WA = Pulumi.AzureNative.Web.V20230101.WebApp;
namespace Infrastructure;

internal static class WebApp
{
	public static WA Create(
		Output<string> resourceGroupName
		, Output<string> appServicePlanId
		, Output<string> resourceGroupLocation
		, Output<string> storageAccountConnectionString
		, Output<string> appServicePlanName
		, string functionAppName)

		=> new WA(functionAppName, new WebAppArgs
		{
			Kind = "FunctionApp",
			ResourceGroupName = resourceGroupName,
			ServerFarmId = appServicePlanId,
			Location = resourceGroupLocation,
			SiteConfig = new SiteConfigArgs
			{
				LinuxFxVersion = "DOTNET|6.0",

				AppSettings = new NameValuePairArgs[] {
					new NameValuePairArgs
					{
						Name = "FUNCTIONS_EXTENSION_VERSION"
					, Value = "~4"
					},
					new NameValuePairArgs {
						Name = "FUNCTIONS_WORKER_RUNTIME"
					 , Value = "dotnet-isolated"
					},
					new NameValuePairArgs
					{
						Name = "AzureWebJobsStorage"
					 ,
						Value = storageAccountConnectionString
					},
					new NameValuePairArgs
					{
						Name = "SCM_DO_BUILD_DURING_DEPLOYMENT"
					 ,
						Value = "0"
					},
					new NameValuePairArgs
					{
						Name = "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
						Value = storageAccountConnectionString
					},
					new NameValuePairArgs
					{
						Name = "WEBSITE_CONTENTSHARE",
						Value = appServicePlanName//application
					},
					new NameValuePairArgs
					{
						Name = "WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED",
						Value = "1"
					}
				}
			}
		});
}

