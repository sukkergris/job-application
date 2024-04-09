using Pulumi.AzureNative.Web.V20230101;
using Pulumi.AzureNative.Web.V20230101.Inputs;
using Pulumi;
using ASP = Pulumi.AzureNative.Web.V20230101.AppServicePlan;

namespace Infrastructure;

internal static class AppServicePlan
{
	public static ASP Create(Output<string> resourceGroupName, string appServicePlanName) => new ASP(appServicePlanName, new AppServicePlanArgs
	{
		ResourceGroupName = resourceGroupName,

		// Run on Linux
		Kind = "Linux",

		// Consumption plan SKU
		Sku = new SkuDescriptionArgs
		{
			Tier = "Dynamic",
			Name = "Y1"
		},

		// For Linux, you need to change the plan to have Reserved = true property.
		Reserved = true
	});

}

