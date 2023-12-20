using Serilog;
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Pulumi;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Azure.Identity;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.AzureSignTool;
using Azure.Core;
using Pulumi.AzureNative.NetApp.V20210401.Inputs;
using _build;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

[GitHubActions("build-test-provision-deploy",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "main" },
    ImportSecrets = new[] { nameof(PulumiAccessToken), nameof(AzureClientSecret), nameof(AzureTenantId), nameof(AzureClientId), nameof(PulumiStackName), nameof(PulumiOrganization) })
    ]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.AndDeploy);

    #region Build Configurations
    readonly string dotnetVersion = "net6.0";
    readonly string dotnetRuntime = "linux-x64";
    readonly string heiselberg_mails = "Heiselberg.Mails";
    readonly string heiselberg_mails_csproj = "Heiselberg.Mails.csproj";
    #endregion
    #region Paths
    AbsolutePath IaC_Root_Dir => RootDirectory / "IaC";
    AbsolutePath SourceCodeDir => RootDirectory / "src";
    AbsolutePath PublishDir => SourceCodeDir / $"{heiselberg_mails}/bin/{Configuration}/{dotnetRuntime}/Publish";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath ZipDir => ArtifactsDir / $"app.zip";
    #endregion
    #region Environment Configurations
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    //[Parameter("Environment to build - Default is 'dev'")]
    //readonly string Environment = "dev";
    #endregion
    #region Dynamic variables
    AzureFunctionConfig AzureFunctionConfig;
    AzureStorageAccount AzureStorageAccount;
    #endregion
    #region Project config

    [Parameter("PULUMI_ORGANIZATION")]
    readonly string PulumiOrganization;

    [Parameter("PULUMI_STACK_NAME")]
    readonly string PulumiStackName; // Found in ~/IaC/job-application/Pulumi.yaml #todo: Auto resolve from Pulumi.yaml

    readonly string stackEnvironment = "dev"; // #todo: Resolve depending on the environment
    //WARNING!: string stackName => $"{PulumiOrganization}/{PulumiStackName}/{stackEnvironment}"; // NOT WORKING IN GITHUB ACTIONS
    #endregion
    #region Chained Targets
    Target Clean => _ => _.Executes(GoClean);
    Target AndRestore => _ => _.DependsOn(Clean).Executes(GoRestore);
    Target AndCompile => _ => _.DependsOn(AndRestore).Executes(GoCompile);
    Target AndZip => _ => _.DependsOn(AndCompile).Executes(GoZip);
    Target AndDeploy => _ => _.DependsOn(AndZip).DependsOn(IaC).OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(AzureFunctionConfig.FunctionAppName)).Executes(GoDeploy);
    #endregion
    #region Infrastructure
    [Parameter("AZURE_SUBSCRIPTION_ID")]
    [Secret]
    readonly string AzureSubscriptionId;
    [Parameter("PULUMI_ACCESS_TOKEN")]
    [Secret]
    readonly string PulumiAccessToken;
    Target IaC => _ => _.Requires(()=>AzureSubscriptionId).Requires(() => PulumiAccessToken).Requires(() => PulumiStackName).Requires(() => PulumiOrganization).Executes(() =>
    {
        (AzureFunctionConfig, AzureStorageAccount)  = GoProvisionInfrastructure();
    });
    private (AzureFunctionConfig azFunction, AzureStorageAccount azStorageAccount) GoProvisionInfrastructure()
    {
        string iacProjectFolder = PulumiStackName;

        string stackName = $"{PulumiOrganization}/{PulumiStackName}/{stackEnvironment}";

        PulumiTasks.PulumiUp(_ => _
            .SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN", PulumiAccessToken)
            .SetCwd(IaC_Root_Dir / PulumiStackName)
            .SetStack(stackName)
            .EnableSkipPreview()
               );

        var variableOutputs = GetVariableOutput.FromStack(IaC_Root_Dir / iacProjectFolder,stackName, PulumiAccessToken); // # iacProjectFolder == project folder name "job-application
        var resourceGroupId = variableOutputs.Named("ResourceGroupId");
        var linuxFunctionAppId = variableOutputs.Named("LinuxFunctionAppId");
        var linuxFunctionAppName = variableOutputs.Named("LinuxFunctionAppName");
        var resourceGroupName = variableOutputs.Named("ResourceGroupName");
        var storageAccountName = variableOutputs.Named("StorageAccountName");

        return (new AzureFunctionConfig(AzureSubscriptionId ,resourceGroupName,linuxFunctionAppName), new AzureStorageAccount(storageAccountName));
    }
    #endregion
    #region Clean
    private void GoClean()
    {
        Log.Information("Cleaning up source code directories"); //Nuke Build Telemetry: https://nuke.build/docs/fundamentals/logging/
        SourceCodeDir.GlobDirectories("**/bin", "**/obj").DeleteDirectories();

        //AbsolutePath.CreateOrCleanDirectory(ArtifactsDir); // #TODO: FIX - But I just cant find that namespace :/

        EnsureCleanDirectory(ArtifactsDir);
    }
    #endregion
    #region  Restore
    // first 'Clean' then 'GoRestore' then STOP.
    Target Restore => _ => _
        .Executes(GoRestore);
    private void GoRestore()
    {
        DotNetTasks.DotNetRestore(settings => settings
            .SetProjectFile(SourceCodeDir / $"{heiselberg_mails}/{heiselberg_mails_csproj}")
            .SetRuntime("linux-x64"));
    }
    #endregion
    #region Compile
    Target Compile => _ => _
        .Executes(GoCompile);
    private void GoCompile()
    {
        DotNetTasks.DotNetPublish(settings => settings
            .SetProject(SourceCodeDir / $"{heiselberg_mails}/Heiselberg.Mails.csproj")
            .SetConfiguration(Configuration)
            .EnableNoRestore() // We have already restored in 'GoRestore'
            .SetSelfContained(true)
            .SetRuntime(dotnetRuntime)
            .SetFramework(dotnetVersion)
            .SetOutput(PublishDir)
            );
    }
    #endregion
    #region Zip
    Target Zip => _ => _
        .Executes(GoZip);
    void GoZip()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ZipDir));
        System.IO.Compression.ZipFile.CreateFromDirectory(PublishDir, ZipDir);
    }
    #endregion
    #region Deploy

    Target Deploy => _ => _
        .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(AzureFunctionConfig.FunctionAppName))
        .Executes(GoDeploy);
    private async Task GoDeploy()
    {
        await ZipDeploy.ThisArtifact(ZipDir).ToAzureFunction(AzureFunctionConfig);
        var azCredential = new DefaultAzureCredential(includeInteractiveCredentials:false);
        var blobServiceClient = AzureBlobClientFactory.Create(AzureStorageAccount.Name, azCredential);
        var staticWebsite = new AzureStaticWebsiteDeployment(blobServiceClient);

        await staticWebsite.Upload("", "");

    }
    #endregion
    #region AzureTasks
    [Parameter("AZURE_CLIENT_ID")]
    [Secret]
    readonly string AzureClientId;
    [Parameter("AZURE_CLIENT_SECRET")]
    [Secret]
    readonly string AzureClientSecret;
    [Parameter("AZURE_TENANT_ID")]
    [Secret]
    readonly string AzureTenantId;
    Target LoginToAzure => _ => _.DependentFor(IaC)
                                    .Requires(() => AzureClientId)
                                    .Requires(() => AzureClientSecret)
                                    .Requires(() => AzureTenantId)
    .Executes(() =>
    {
        // ProcessTasks.StartProcess("az", $"login --service-principal --username {AzureClientId} --password {AzureClientSecret} --tenant {AzureTenantId}", RootDirectory);
        var azCredential = new DefaultAzureCredential(includeInteractiveCredentials:false);
        var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
        var azAccessToken = azCredential.GetToken(tokenRequestContext);

        if (string.IsNullOrWhiteSpace(azAccessToken.Token)){
            Log.Debug("Could not acquire token based on 'DefaultAzureCredential()'");
        }
        else
            Log.Debug("Azure toke acquired");
    });
    #endregion
}