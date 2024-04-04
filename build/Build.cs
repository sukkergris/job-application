using Serilog;
using Nuke.Common;
using Nuke.Common.IO;
using Azure.Identity;
using Nuke.Common.CI.GitHubActions;
using Azure.Core;
using _build;
using System.Threading.Tasks;
using static BuildConfigurations;
using static ProjectPaths;

[GitHubActions("build-test-provision-deploy",
	 GitHubActionsImage.UbuntuLatest,
	 OnPushBranches = new[] { "main" },
	 ImportSecrets = new[] { nameof(PulumiAccessToken), nameof(PulumiStackName), nameof(PulumiOrganization) })
	 ]
class Build : NukeBuild
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	public static int Main() => Execute<Build>(x => x.AndDeploy);
	AbsolutePath PublishDir => SourceCodeDir / $"{heiselberg_mails}/bin/{Configuration}/{dotnetRuntime}/Publish";

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
	Target Clean => _ => _.Executes(NukeClean.Go);
	Target BuildFrontend => _ => _.Executes(NukeBuildFrontend.Go);
	Target AndRestore => _ => _.DependsOn(Clean).Executes(NukeRestore.Go);
	Target AndCompile => _ => _.DependsOn(AndRestore).Executes(() => NukeCompile.Go(Configuration, PublishDir));

	Target AndZip => _ => _.DependsOn(AndCompile).Executes(() => NukeZipAzureFunction.Go(PublishDir));
	Target AndDeploy => _ => _
		.DependsOn(AndZip)
		.DependsOn(IaC)
		.DependsOn(BuildFrontend)
			.OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(AzureFunctionConfig.FunctionAppName))
		.Executes(() => Log.Debug("READY TO DEPLOY!!!"));
	#endregion
	#region Infrastructure
	[Parameter("AZURE_SUBSCRIPTION_ID")]
	[Secret]
	readonly string AzureSubscriptionId;
	[Parameter("PULUMI_ACCESS_TOKEN")]
	[Secret]
	readonly string PulumiAccessToken;
	Target IaC => _ => _.Requires(() => AzureSubscriptionId).Requires(() => PulumiAccessToken).Requires(() => PulumiStackName).Requires(() => PulumiOrganization).Executes(() =>
	  {
		  (AzureFunctionConfig, AzureStorageAccount) = NukeInfrastructureAsCode.GoProvisionInfrastructure(
				PulumiOrganization,
				PulumiStackName,
				stackEnvironment,
				PulumiAccessToken,
				AzureSubscriptionId,
				AzureToken);
	  });
	#endregion

	#region  Restore
	// first 'Clean' then 'GoRestore' then STOP.
	Target Restore => _ => _
		 .Executes(NukeRestore.Go);
	#endregion
	#region Compile backend
	Target Compile => _ => _
		 .Executes(() => NukeCompile.Go(Configuration, PublishDir));
	#endregion
	#region Zip
	Target Zip => _ => _
		 .Executes(() => NukeZipAzureFunction.Go(PublishDir));
	#endregion
	#region Deploy
	Target Deploy => _ => _
		 .OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(AzureFunctionConfig.FunctionAppName))
		 .Executes(GoDeploy);
	private async Task GoDeploy()
	{
		Log.Debug("Now deploying the azure function using zip deploy");
		await ZipDeploy.ThisArtifact(ZipDir).ToAzureFunction(AzureFunctionConfig);

		Log.Debug("Now creating the static website");
		//var azCredential = new DefaultAzureCredential(includeInteractiveCredentials:false);
		//Log.Debug($"Logged in as: {}");

		var webBlobContainerClient = await AzureBlobClientFactory
			 .Create(AzureStorageAccount.Name, AzureFunctionConfig.StorageAccountKey)
			 .GetWebBlobContainerClient();

		var staticWebsite = new AzureStaticWebsiteDeployment(webBlobContainerClient);

		// Every thing goes to the $web container
		Log.Debug("Here comes the exiting part where we sync what ever is in the front-end dir to the azure $web blob container");

		await staticWebsite.SyncStaticWebsiteContentFiles(FrontEndDir / "wwwroot");
	}
	#endregion
	#region AzureTasks
	private string AzureToken;
	Target LoginToAzure => _ => _.DependentFor(IaC).Executes(() =>
	{
		// ProcessTasks.StartProcess("az", $"login --service-principal --username {AzureClientId} --password {AzureClientSecret} --tenant {AzureTenantId}", RootDirectory);
		var defaultAzCredential = new DefaultAzureCredential(includeInteractiveCredentials: false);
		var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
		var azAccessToken = defaultAzCredential.GetToken(tokenRequestContext);

		if (string.IsNullOrWhiteSpace(azAccessToken.Token))
		{
			Log.Debug("Could not acquire token based on 'DefaultAzureCredential()'");
		}
		else
		{
			Log.Debug("Azure toke acquired using 'DefaultAzureCredential'");
		}

		AzureToken = azAccessToken.Token;
	});
	#endregion
}