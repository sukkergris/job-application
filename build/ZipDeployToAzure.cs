using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Newtonsoft.Json.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

namespace _build;
public record AzureFunctionAppPublishingCredentials(string PublishingUserName, string PublishingPassword);
public record AzureFunctionConfig(string SubscriptionId, string ResourceGroupName, string FunctionAppName);
public static class ZipDeploy
{
    public static MyAzureFunction ThisArtifact(AbsolutePath artifactPath)
    {
        return new MyAzureFunction(artifactPath);
    }
}
public class MyAzureFunction
{
    public MyAzureFunction(AbsolutePath artifactPath)
    {
        _artifactPath = artifactPath;
    }
    readonly AbsolutePath _artifactPath;
    public async Task ToAzureFunction(AzureFunctionConfig config)
    {
        // https://learn.microsoft.com/en-us/azure/governance/resource-graph/first-query-rest-api
        var azCredential = new AzureCliCredential(); // new DefaultAzureCredential(); // new AzureCliCredential();
        var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
        var azAccessToken = await azCredential.GetTokenAsync(tokenRequestContext);
        if (string.IsNullOrEmpty(azAccessToken.Token))
        {
            Log.Error("Azure token not acquired. Try to login using: 'az login'");
            Assert.Fail("Azure token not acquired");
        }
        Log.Debug("Azure token acquired");

        var publishingCredentials = await GetCredentials(config, azAccessToken);

        await Publish(config, publishingCredentials);
    }

    private async Task Publish(AzureFunctionConfig config, AzureFunctionAppPublishingCredentials credentials)
    {
        using var httpClient = new HttpClient();

        var base64Auth = Convert.ToBase64String(Encoding.Default.GetBytes($"{credentials.PublishingUserName}:{credentials.PublishingPassword}"));

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

        await using var package = File.OpenRead(_artifactPath);

        var streamContent = new StreamContent(package);

        var uploadResp = await httpClient.PostAsync($"https://{config.FunctionAppName}.scm.azurewebsites.net/api/zipdeploy", streamContent);

        Log.Debug(uploadResp.StatusCode.ToString());

        if (!uploadResp.IsSuccessStatusCode)
        {
            //var reqMsg = await uploadResp.RequestMessage.Content.ReadAsStringAsync();

            //var respMsg = await uploadResp.Content.ReadAsStringAsync();

            //Log.Error($"Azure function not deployed: {respMsg}");
            Log.Error($"Request message: {uploadResp.StatusCode}");
            Assert.Fail("Azure function not deployed");
        }
        Log.Debug("Azure function deployed");
    }
    private async Task<AzureFunctionAppPublishingCredentials> GetCredentials(AzureFunctionConfig config, AccessToken token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);

        var getPublishingCredentials = $"https://management.azure.com/subscriptions/{config.SubscriptionId}/resourceGroups/{config.ResourceGroupName}/providers/Microsoft.Web/sites/{config.FunctionAppName}/config/publishingcredentials/list?api-version=2019-08-01";
        var response = await httpClient.PostAsync(getPublishingCredentials, null);
        var json = await response.Content.ReadAsStringAsync();
        Log.Information(json);
        var msg = $"Publishing credentials for {config.SubscriptionId}/{config.ResourceGroupName}/{config.FunctionAppName}";
        if (!response.IsSuccessStatusCode)
        {
            var contentMsg = await response.Content.ReadAsStringAsync();
            Log.Error(contentMsg);
            Assert.Fail($"Couldn't acquire publishing credentials for {msg}");
        }

        Assert.True(response.IsSuccessStatusCode, $"Publishing credentials for {msg} acquired");
        var publishingCredentials = JObject.Parse(json)["properties"];

        var publishingUserName = (string)publishingCredentials["publishingUserName"];
        var publishingPassword = (string)publishingCredentials["publishingPassword"];
        return new AzureFunctionAppPublishingCredentials(publishingUserName, publishingPassword);
    }
}
