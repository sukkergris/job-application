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

    public static GetVariableOutput FromStack(AbsolutePath stack, string stackName) => new GetVariableOutput(stack, stackName);

    private GetVariableOutput(AbsolutePath stackPath, string stackName)
    {
        _stackPath = stackPath;
        _stackName = stackName;
    }
    public string Named(string propName)
    {
        // logger($"StackPaht: {_stackPath}");
        // logger($"StackName: {_stackName}");
        // #TODO: Figure out why this hack is necessary
        // Serilog.Log.Debug($"Looking for propName: {propName}");
        PulumiTasks.PulumiStackSelect(_ => _.SetCwd(_stackPath).SetStackName(_stackName));

        return PulumiTasks.PulumiStackOutput(_ => _
            //.SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN",_pulumiAccessToken)
            //.SetProcessEnvironmentVariable("ARM_USE_OIDC", _ARM_USE_OIDC)
            .SetCwd(_stackPath)
            .SetPropertyName(propName)
            .EnableShowSecrets()
            .DisableProcessLogOutput())
            .StdToText();
    }
}
