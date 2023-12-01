using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;
using Nuke.Common.Utilities.Collections;
using Serilog;
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

    AbsolutePath IaC_Job_Application_Dir => RootDirectory / "IaC/job-application";

    AbsolutePath SourceCodeDir => RootDirectory / "src";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter("Environment to build - Default is 'dev'")]
    readonly string Environment = "dev";

    [Parameter("Pulumi Access Toke")]
    readonly string PULUMI_ACCESS_TOKEN = "GET FROM ENVIRONMENT VARIABLE OR GO HOME";
    readonly string pulumiStackEnvironment = "dev"; // #todo: Resolve depending on the environment

    readonly string pulumiStackName = "job-application"; // Found in ~/IaC/job-application/Pulumi.yaml #todo: Auto resolve from Pulumi.yaml

#region Infrastructure
    Target ProvisionInfrastructure => _ => _.Executes(GoProvisionInfrastructure);
    private void GoProvisionInfrastructure(){
            PulumiTasks.PulumiUp(_ => _
                .SetCwd(IaC_Job_Application_Dir)
                .SetStack($"{pulumiStackName}/{pulumiStackEnvironment}")
                .EnableSkipPreview()
                .SetProcessEnvironmentVariable("PULUMI_ACCESS_TOKEN",PULUMI_ACCESS_TOKEN));
            
    }
#endregion
#region Clean
    Target Clean => _ => _.Executes(GoClean);

    private void GoClean(){
        Log.Information("Cleaning up source code directories"); //Nuke Build Telemetry: https://nuke.build/docs/fundamentals/logging/
        SourceCodeDir.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
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
#region Compile
    Target AndCompile => _ => _.DependsOn(AndRestore).Executes(GoCompile);
    Target Compile => _ => _
        .Executes(GoCompile);
    private void GoCompile(){}
#endregion

}
