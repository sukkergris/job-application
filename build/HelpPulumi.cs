using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;

namespace _build;

public class GetVariableOutput
{
    readonly AbsolutePath _stack;
    readonly string _pulumiAccessToken;

    public static GetVariableOutput FromStack(AbsolutePath stack, string pulumiAccessToken) => new GetVariableOutput(stack, pulumiAccessToken);

    private GetVariableOutput(AbsolutePath stack,string pulumiAccessToken)
    {
        _stack = stack;
        _pulumiAccessToken = pulumiAccessToken;
    }
    public string Named(string propName)
    {
        return PulumiTasks.PulumiStackOutput(_ => _
            .SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN",_pulumiAccessToken)
            .SetCwd(_stack)
            .SetPropertyName(propName)
            .EnableShowSecrets()
            .DisableProcessLogOutput())
            .StdToText();
    }
}
