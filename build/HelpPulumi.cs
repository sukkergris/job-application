using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;

namespace _build;

public class GetVariableOutput
{
    readonly AbsolutePath _stackPath;
    readonly string _stackName;
    readonly string _pulumiAccessToken;

    public static GetVariableOutput FromStack(AbsolutePath stack,string stackName, string pulumiAccessToken) => new GetVariableOutput(stack,stackName, pulumiAccessToken);

    private GetVariableOutput(AbsolutePath stackPath,string stackName,string pulumiAccessToken)
    {
        _stackPath = stackPath;
        _stackName = stackName;
        _pulumiAccessToken = pulumiAccessToken;
    }
    public string Named(string propName)
    {
        // #TODO: Figure out why this hack is necessary
        PulumiTasks.PulumiStackSelect(_=>_.SetCwd(_stackPath).SetStackName(_stackName));

        return PulumiTasks.PulumiStackOutput(_ => _
            .SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN",_pulumiAccessToken)
            .SetCwd(_stackPath)
            .SetPropertyName(propName)
            .EnableShowSecrets()
            .DisableProcessLogOutput())
            .StdToText();
    }
}
