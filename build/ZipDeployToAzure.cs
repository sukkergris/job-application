using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;

namespace _build;

public static class ZipDeploy
{
    public static AzureFunction ThisArtifact(Uri artifact)
    {
        return new AzureFunction(artifact);
    }
    public  class AzureFunction
    {
        public AzureFunction(Uri artifact)
        {
            _artifact = artifact;
        }
        readonly Uri _artifact;
        public static async Task ToAzureFunction()
        {
            // https://learn.microsoft.com/en-us/azure/governance/resource-graph/first-query-rest-api
            var azCredential = new DefaultAzureCredential();
            var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
            var azAccessToken = await azCredential.GetTokenAsync(tokenRequestContext);
            var managementClient = 
    }
    }
}
