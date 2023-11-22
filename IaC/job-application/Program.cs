using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    var azureConfig = new Pulumi.Config("azure"); // Pulumi.{stack}.yaml
    var azLocation = azureConfig.Require("location"); 
    var resourceType = "rg";
    var application = "heiselberg-contact-form";

    var environment = Pulumi.Deployment.Instance.StackName;

    var instanceCount = "001";

    // Create an Azure Resource Group {resource-type}-{application}-{environment}-{az-region}-{instance-count}
    // {rg}-{heiselberg-contact-form}-{dev}-{northeurope}-{001}-{pulumi-id(AUTO)}
    var resourceGroupName = $"{resourceType}-{application}-{environment}-{azRegion}-{instanceCount}-";

    var resourceGroup = new Pulumi.Azure.Core.ResourceGroup(resourceGroupName, new() { Location = azRegion }); 

    // Create an Azure Storage Account
    var storageAccount = new Account("storage", new AccountArgs
    {
        ResourceGroupName = resourceGroup.Name,
        AccountReplicationType = "LRS",
        AccountTier = "Standard"
    });

    // Export the connection string for the storage account
    return new Dictionary<string, object?>
    {
        ["connectionString"] = storageAccount.PrimaryConnectionString
    };
});
