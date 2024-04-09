using Azure.Core;
using Azure.Identity;
using Nuke.Common;
using Serilog;
using System.Threading.Tasks;

namespace _build
{
    public static class AzureLogin
    {
        public static async Task<AccessToken> GetAzureAccessTokenFromDefaultAzureCredentials()
        {
            // https://learn.microsoft.com/en-us/azure/governance/resource-graph/first-query-rest-api
            var azCredential = new DefaultAzureCredential(); // new DefaultAzureCredential(); // new AzureCliCredential();
            var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
            var azAccessToken = await azCredential.GetTokenAsync(tokenRequestContext);
            if (string.IsNullOrEmpty(azAccessToken.Token))
            {
                Log.Error("Azure token not acquired. Try to login using: 'az login'");
                Assert.Fail("Azure token not acquired");
            }
            Log.Debug("Azure token acquired");
            return azAccessToken;
        }
    }
}
