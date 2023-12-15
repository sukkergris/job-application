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

[GitHubActions("build-test-provision-deploy", GitHubActionsImage.UbuntuLatest, OnPushBranches = new[] { "main" })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Clean);

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

    [Parameter("Environment to build - Default is 'dev'")]
    readonly string Environment = "dev";
    #endregion
    #region Static names - magic strings

    [Parameter("PULUMI_ORGANIZATION")]
    readonly string PulumiOrganization;

    [Parameter("PULUMI_STACKNAME")]
    readonly string stack; // Found in ~/IaC/job-application/Pulumi.yaml #todo: Auto resolve from Pulumi.yaml

    readonly string stackEnvironment = "dev"; // #todo: Resolve depending on the environment
    string stackName => $"{PulumiOrganization}/{stack}/{stackEnvironment}";
    #endregion
    #region Chained Targets
    Target Clean => _ => _.Executes(GoClean);
    Target AndRestore => _ => _.DependsOn(Clean).Executes(GoRestore);
    Target AndCompile => _ => _.DependsOn(AndRestore).Executes(GoCompile);
    Target AndZip => _ => _.DependsOn(AndCompile).Executes(GoZip);
    Target AndDeploy => _ => _.DependsOn(AndZip).DependsOn(IaC).Executes(GoDeploy);
    #endregion
    #region Infrastructure
    [Parameter("PULUMI_ACCESS_TOKEN")]
    readonly string PulumiAccessToken;
    Target IaC => _ => _.Requires(() => PulumiAccessToken).Executes(GoProvisionInfrastructure);
    private void GoProvisionInfrastructure()
    {
        PulumiTasks.PulumiStackSelect(_ => _.SetCwd(IaC_Root_Dir / stack).SetStackName(stackName));

        PulumiTasks.PulumiUp(_ => _
            .SetCwd(IaC_Root_Dir / stack)
            .SetStack(stackName)
            .EnableSkipPreview()
            .SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN", PulumiAccessToken));
    }
    #endregion
    #region Clean
    private void GoClean()
    {
        Log.Information("Cleaning up source code directories"); //Nuke Build Telemetry: https://nuke.build/docs/fundamentals/logging/
        SourceCodeDir.GlobDirectories("**/bin", "**/obj").DeleteDirectories();

        //AbsolutePath.CreateOrCleanDirectory(ArtifactsDir);
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
        .Executes(GoDeploy);
    private void GoDeploy()
    {

    }
    #endregion
    #region AzureTasks
    [Parameter("AZURE_CLIENT_ID")]
    readonly string AzureClientID;
    [Parameter("AZURE_CLIENT_SECRET")]
    readonly string AzureClientSecret;
    [Parameter("AZURE_TENANT_ID")]
    readonly string AzureTenantId;
    Target LoginToAzure => _ => _.Requires(() => AzureClientID).Requires(() => AzureClientSecret).Requires(() => AzureTenantId).Executes(() =>
    {
        ProcessTasks.StartProcess("az", $"login --service-principal --username {AzureClientID} --password {AzureClientSecret} --tenant {AzureTenantId}", RootDirectory);
        var azCredential = new DefaultAzureCredential();
        var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
        var azAccessToken = azCredential.GetToken(tokenRequestContext);
        Log.Debug("azAccessToken", azAccessToken.Token);
    });
    #endregion
}
