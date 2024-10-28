using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;

namespace ReSharperPlugin.Git;

public class GitDaemonStageProcess : IDaemonStageProcess
{
    public IDaemonProcess DaemonProcess { get; }
    private readonly ICSharpFile _file;

    public GitDaemonStageProcess(IDaemonProcess process, ICSharpFile file)
    {
        DaemonProcess = process;
        _file = file;
    }
    
    public void Execute(Action<DaemonStageResult> committer)
    {
        if (GitChecker.LastCommitHashesAndMessages.Count == 0)
        {
            committer(new DaemonStageResult(new List<HighlightingInfo>()));
            return;
        }

        var sourceFilePath = _file.GetSourceFile().GetLocation().FullPath;
        var ( gitOutput,  gitError) = CommandRunner.RunGitCommand($"blame -l {sourceFilePath}", _file.GetSourceFile().GetLocation().Directory.ToString());
        if (!gitError.IsNullOrEmpty())
        {
            Console.WriteLine("Error");
            return;
        }
        List<HighlightingInfo> highlightingInfos = new List<HighlightingInfo>();
        var gitBlameEntries = gitOutput.Split('\n');
        for(int lineCount = 0; lineCount < gitBlameEntries.Length && GitHighlighting.TaskLimit > 0; lineCount++)
        {
            string line = gitBlameEntries[lineCount];
            string commitHash = line.Split(' ')[0];
            if (GitChecker.LastCommitHashesAndMessages.TryGetValue(commitHash, out string commitMessage))
            {
                GitHighlighting tempHighlighting = new GitHighlighting(commitMessage, lineCount, _file.GetSourceFile().Document);
                highlightingInfos.Add(new HighlightingInfo(tempHighlighting.CalculateRange(), tempHighlighting));
            }
        }

        if (highlightingInfos.Count != 0)
        {
            committer(new DaemonStageResult(highlightingInfos));
        }
    }
}