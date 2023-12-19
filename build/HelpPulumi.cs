using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;

namespace _build;

public class GetVariableOutput
{
    readonly AbsolutePath _stack;

    public static GetVariableOutput FromStack(AbsolutePath stack) => new GetVariableOutput(stack);

    private GetVariableOutput(AbsolutePath stack)
    {
        _stack = stack;
    }
    public string Named(string propName)
    {
        return PulumiTasks.PulumiStackOutput(_ => _
            .SetCwd(_stack)
            .SetPropertyName(propName)
            .EnableShowSecrets()
            .DisableProcessLogOutput())
            .StdToText();
    }
}
