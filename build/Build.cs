using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[TeamCity(TeamCityAgentPlatform.Unix,
    NonEntryTargets = new[] {nameof(Restore)},
    ExcludedTargets = new[] {nameof(Clean)},
    VcsTriggeredTargets = new[] {nameof(Compile), nameof(Up)})]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter("Check to wipe the database data")]
    readonly bool WipeDatabaseData;

    [Solution] readonly Solution Solution;
    [PathExecutable("docker-compose")] readonly Tool DockerCompose;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    //AbsolutePath TestsDirectory => RootDirectory / "tests";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            //TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Down => _ => _
        .Executes(() =>
        {
            DockerCompose(WipeDatabaseData ? "down --volumes" : "down", SourceDirectory);
        });

    Target Up => _ => _
        .DependsOn(Down)
        .Executes(() =>
        {
            DockerCompose("up --build -d", SourceDirectory);
        });
}
