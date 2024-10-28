using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReSharperPlugin.Git;

[DaemonStage]
public class GitDaemonStage : CSharpDaemonStageBase
{
    protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings,
        DaemonProcessKind processKind, ICSharpFile file)
    {
        return new GitDaemonStageProcess(process, file);
    }
}