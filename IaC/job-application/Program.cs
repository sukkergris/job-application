using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.AppService.Outputs;
using Pulumi.Azure.Core;
using Pulumi.Azure.Network;
using Pulumi.Azure.Network.Outputs;
using Pulumi.AzureNative.Network;
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


    //var domain = new Pulumi.AzureNative.DomainRegistration.Domain($"young-heiselberg",new Pulumi.AzureNative.DomainRegistration.DomainArgs
    //{

    //});

    //var dnsZoneName = "heiselberg";
    //var dnsZone = new Pulumi.Azure.Dns.Zone(dnsZoneName, new Pulumi.Azure.Dns.ZoneArgs { 
    //    Name = dnsZoneName,
    //    ResourceGroupName = resourceGroup.Name
    //});

    //var record = new Pulumi.Azure.Dns.CNameRecord("dimmer", new Pulumi.Azure.Dns.CNameRecordArgs {

    //});

    var staticIp = new  PublicIp("young-heiselberg-ip", new PublicIpArgs
    {
        ResourceGroupName = resourceGroup.Name
    });

    var cdnProfile = new Pulumi.AzureNative.Cdn.Profile($"{application}-",new Pulumi.AzureNative.Cdn.ProfileArgs {
        Location = resourceGroup.Location,
        ProfileName = $"cdn-heis-sw",
        ResourceGroupName = resourceGroup.Name,
        Sku = new Pulumi.AzureNative.Cdn.Inputs.SkuArgs
        {
            Name = "Standard_Microsoft" // https://www.pulumi.com/registry/packages/azure-native/api-docs/cdn/profile/#sku
        }
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
