using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;
using Pulumi.AzureAD;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    // Static project specific configuirations
    var resourceType = "rg";
    var application = "heisconform"; // heiselberg-contact-form

    // Stack specific configurations
    var azureConfig = new Pulumi.Config("azure"); // Pulumi.{stack}.yaml
    var azLocation = azureConfig.Require("location");
    var environment = Deployment.Instance.StackName;

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

    var appServicePlan = new ServicePlan($"{application}-{environment}-", new()
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

    return new Dictionary<string, object?>
    {
        ["ResourceGroupId"] = resourceGroup.Id,
        ["ResourceGroupName"] = resourceGroup.Name,
        ["LinuxFunctionAppId"] = linuxFunctionApp.Id,
        ["LinuxFunctionAppName"] = linuxFunctionApp.Name,
        ["StorageAccountName"] = storageAccount.Name,
        ["StorageAccountKey"] = storageAccount.PrimaryAccessKey,
        ["StorageAccountDefaultEndpoint"] = storageAccount.PrimaryWebEndpoint
    };
});

