using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;
using System;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
    var secret = Environment.GetEnvironmentVariable("SECRET");

    var azureConfig = new Pulumi.Config("azure"); // Pulumi.{stack}.yaml
    var azLocation = azureConfig.Require("location"); 
    var resourceType = "rg";
    var application = "heiselberg-contact-form";
    

    var environment = Pulumi.Deployment.Instance.StackName;
    Log.Debug(secret);
    var instanceCount = "001";

    // Create an Azure Resource Group {resource-type}-{application}-{environment}-{az-region}-{instance-count}
    // {rg}-{heiselberg-contact-form}-{dev}-{northeurope}-{001}-{pulumi-id(AUTO)}
    var resourceGroupName = $"{resourceType}-{application}-{environment}-{azLocation}-{instanceCount}-";

    var resourceGroup = new Pulumi.Azure.Core.ResourceGroup(resourceGroupName, new() { Location = azLocation }); 

    // Create an Azure Storage Account
   //  var storageAccount = new Account("storage", new AccountArgs
    // {
        // ResourceGroupName = resourceGroup.Name,
        // AccountReplicationType = "LRS",
        // AccountTier = "Standard"
    // });

    

    // Export the connection string for the storage account
    return new Dictionary<string, object?>
    {
        // ["connectionString"] = storageAccount.PrimaryConnectionString
        ["ResourceGroupId"] = resourceGroup.Id // Is getting parsed to output after a propper run
        , ["Secret"] = secret // Is getting parsed to output even on pre-run
    };
});
