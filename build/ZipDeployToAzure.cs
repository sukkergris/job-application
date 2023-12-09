using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;

namespace _build;

public static class ZipDeployToAzure
{ public static async Task Now()
    {

        // https://learn.microsoft.com/en-us/azure/governance/resource-graph/first-query-rest-api
        var azCredential = new DefaultAzureCredential();
        var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
        var azAccessToken = await azCredential.GetTokenAsync(tokenRequestContext);
        var managementClient = 
    }
}
