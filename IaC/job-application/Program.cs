using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.AppService.Outputs;
using Pulumi.Azure.Core;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    var azureConfig = new Config("azure"); // Pulumi.{stack}.yaml
    var azLocation = azureConfig.Require("location");
    var resourceType = "rg";
    var application = "heisconform"; // heiselberg-contact-form

    var environment = Deployment.Instance.StackName;

    // var instanceCount = "001";

    // Create an Azure Resource Group {resource-type}-{application}-{environment}-{az-region}-{instance-count}
    // {rg}-{heiselberg-contact-form}-{dev}-{northeurope}-{001}-{pulumi-id(AUTO)}
    var resourceGroupName = $"{resourceType}-{application}-{environment}-{azLocation}-";

    var resourceGroup = new ResourceGroup(resourceGroupName, new() { Location = azLocation });

    var storageAccount = new Pulumi.Azure.Storage.Account($"sa-{application}-{environment}", new()
    {
        ResourceGroupName = resourceGroup.Name,
        AccountReplicationType = "LRS", // Locally redundant storage
        AccountTier = "Standard",
        AccessTier = "Cool",
        EnableHttpsTrafficOnly = true,
        MinTlsVersion = "TLS1_2",
        AccountKind = "StorageV2",
        PublicNetworkAccessEnabled = true
    });

    var appServicePlan = new Pulumi.Azure.AppService.ServicePlan($"{application}-{environment}-", new()
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        OsType = "Linux",
        SkuName = "Y1", // pay-as-you-go
    });

    var linuxFunctionApp = new LinuxFunctionApp($"{application}-{environment}-", new()
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        StorageAccountName = storageAccount.Name,
        StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
        ServicePlanId = appServicePlan.Id,
        AuthSettings = new LinuxFunctionAppAuthSettingsArgs
        {
            Enabled = false,
            UnauthenticatedClientAction = "AllowAnonymous",
            TokenStoreEnabled = false
        },
        SiteConfig = new LinuxFunctionAppSiteConfigArgs
        {

        }
    });

    // https://www.pulumi.com/templates/static-website/azure/
    // https://github.com/pulumi/templates/blob/master/static-website-azure-csharp/Program.cs

    var website = new Pulumi.AzureNative.Storage.StorageAccountStaticWebsite("job-application", new()
    {
        ResourceGroupName = resourceGroup.Name,
        AccountName = storageAccount.Name,
        Error404Document = "404.html",
        IndexDocument = "index.html"

    });

    return new Dictionary<string, object?>
    {
        ["ResourceGroupId"] = resourceGroup.Id,
        ["ResourceGroupName"] = resourceGroup.Name,
        ["LinuxFunctionAppId"] = linuxFunctionApp.Id,
        ["LinuxFunctionAppName"] = linuxFunctionApp.Name,
        ["StorageAccountName"] = storageAccount.Name,
        ["StorageAccountKey"] = storageAccount.PrimaryAccessKey
    };
});
