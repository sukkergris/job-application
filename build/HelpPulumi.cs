using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;
using Pulumi;
using System;

namespace _build;

public class GetVariableOutput
{
    readonly AbsolutePath _stackPath;
    readonly string _stackName;
    readonly string _pulumiAccessToken;
    readonly string _ARM_USE_OIDC;

    public static GetVariableOutput FromStack(AbsolutePath stack,string stackName, string pulumiAccessToken, string ARM_USE_OIDC) => new GetVariableOutput(stack,stackName, pulumiAccessToken, ARM_USE_OIDC);

    private GetVariableOutput(AbsolutePath stackPath,string stackName,string pulumiAccessToken, string ARM_USE_OIDC)
    {
        _stackPath = stackPath;
        _stackName = stackName;
        _pulumiAccessToken = pulumiAccessToken ?? throw new System.ArgumentNullException();
        _ARM_USE_OIDC = ARM_USE_OIDC;
    }
    public string Named(string propName, Action<string> logger)
    {
        logger($"StackPaht: {_stackPath}");
        logger($"StackName: {_stackName}");
        // #TODO: Figure out why this hack is necessary
        PulumiTasks.PulumiStackSelect(_=>_.SetCwd(_stackPath).SetStackName(_stackName));

        return PulumiTasks.PulumiStackOutput(_ => _
            .SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN",_pulumiAccessToken)
            .SetProcessEnvironmentVariable("ARM_USE_OIDC", _ARM_USE_OIDC)
            .SetCwd(_stackPath)
            .SetPropertyName(propName)
            .EnableShowSecrets()
            .DisableProcessLogOutput())
            .StdToText();
    }
}
