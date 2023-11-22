using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    AbsolutePath IaCDir => RootDirectory / "IaC";

    AbsolutePath SourceDir => RootDirectory / "src";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
#region Infrastructure
    Target ProvisionInfrastructure => _ => _.Executes(GoProvisionInfrastructure);
    private void GoProvisionInfrastructure(){

    }
#endregion
#region Clean
    Target Clean => _ => _.Executes(GoClean);

    private void GoClean(){
        SourceDir.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
    }
#endregion
#region  Restore
    // first 'Clean' then 'GoRestore' then STOP. 
    Target AndRestore => _ => _.DependsOn(ProvisionInfrastructure, Clean).Executes(GoRestore);

    Target Restore => _ => _
        .Executes(GoRestore);
    // The work to be done. This makes it possible to run just one specific step at the time. Dependend upon the caller making sure the state is correct before calling.
    private void GoRestore(){

    }
#endregion

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });



}
