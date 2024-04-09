using _build;
using Nuke.Common.Tools.Pulumi;
using Serilog;
using static ProjectPaths;
public static class NukeInfrastructureAsCode
{
	public static (AzureFunctionConfig azFunction, AzureStorageAccount azStorageAccount) GoProvisionInfrastructure(string pulumiOrganization,
		string pulumiStackName,
		string stackEnvironment,
		string pulumiAccessToken,
		string azureSubscriptionId,
		string azureToken)
	{
		string iacProjectFolder = pulumiStackName;

		string stackName = $"{pulumiOrganization}/{pulumiStackName}/{stackEnvironment}";
		var resp = PulumiTasks.PulumiUp(_ => _
			 //.SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN", PulumiAccessToken)
			 //.SetProcessEnvironmentVariable("ARM_USE_OIDC", ARM_USE_OIDC)
			 .SetCwd(IaC_Root_Dir / pulumiStackName)
			 .SetStack(stackName)
			 .EnableSkipPreview()
				 );

		var variableOutputs = GetVariableOutput.FromStack(IaC_Root_Dir / iacProjectFolder, stackName); // # iacProjectFolder == project folder name "job-application
		var resourceGroupId = variableOutputs.Named("ResourceGroupId");
		var linuxFunctionAppId = variableOutputs.Named("LinuxFunctionAppId");
		var linuxFunctionAppName = variableOutputs.Named("LinuxFunctionAppName");
		var resourceGroupName = variableOutputs.Named("ResourceGroupName");
		var storageAccountName = variableOutputs.Named("StorageAccountName");
		var storageAccountKey1 = variableOutputs.Named("StorageAccountKey1");

		return (new AzureFunctionConfig(azureSubscriptionId, resourceGroupName, linuxFunctionAppName, azureToken, storageAccountKey1), new AzureStorageAccount(storageAccountName));
	}
}

