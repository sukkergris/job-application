using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

namespace _build;
public record AzureFunctionAppPublishingCredentials(string PublishingUserName, string PublishingPassword);

public record AzureFunctionConfig(string SubscriptionId, string ResourceGroupName, string FunctionAppName, string AzAccessToken, string StorageAccountKey);
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


        var publishingCredentials = await GetCredentials(config);

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

        Log.Debug($"Zip deploy upload resp status code: {uploadResp.StatusCode}");

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
    private async Task<AzureFunctionAppPublishingCredentials> GetCredentials(AzureFunctionConfig config)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.AzAccessToken);

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
