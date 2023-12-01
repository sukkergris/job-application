﻿using Pulumi;
using Pulumi.Azure.AppService;
using Pulumi.Azure.Core;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    var secret = System.Environment.GetEnvironmentVariable("SECRET");

    var azureConfig = new Config("azure"); // Pulumi.{stack}.yaml
    var azLocation = azureConfig.Require("location");
    var resourceType = "rg";
    var application = "heisconform";

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

    var appServicePlan = new Pulumi.Azure.AppService.ServicePlan($"{application}-{environment}", new()
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        OsType = "Linux",
        SkuName = "F1",// Free
    });

    var linuxFunctionApp = new LinuxFunctionApp($"lifa-{application}-{environment}", new()
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        StorageAccountName = storageAccount.Name,
        StorageAccountAccessKey = storageAccount.PrimaryAccessKey,
        ServicePlanId = appServicePlan.Id,
        SiteConfig = null
    });

    // Export the connection string for the storage account
    return new Dictionary<string, object?>
    {
        // ["connectionString"] = storageAccount.PrimaryConnectionString
        ["ResourceGroupId"] = resourceGroup.Id, // Is getting parsed to output after a propper run
        ["Secret"] = secret // Is getting parsed to output even on pre-run
    };
});
