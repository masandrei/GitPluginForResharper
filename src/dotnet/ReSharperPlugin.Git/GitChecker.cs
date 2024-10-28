using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.Util;

namespace ReSharperPlugin.Git;

[SolutionComponent]
public class GitChecker
{
    private readonly ISolution _solution;
    private readonly string _gitAbsolutePath;
    private readonly ISettingsStore _store;
    private int _lastCommits;
    public static Dictionary<string, string> LastCommitHashesAndMessages { get; private set; }

    public GitChecker(ISolution solution, IDaemon daemon, ISettingsStore store, Lifetime lifetime)
    {
        _solution = solution;
        LastCommitHashesAndMessages = new Dictionary<string, string>();
        _store = store;
        if (TryGetGitRepositoryPath(out _gitAbsolutePath))
        {
            Thread GitThread = new Thread(() =>
            {
                MonitorGitChanges(daemon, lifetime);
                Thread.Sleep(Timeout.Infinite);
            });
            GitThread.IsBackground = true;
            GitThread.Start();
        }
    }

    private void MonitorGitChanges(IDaemon daemon, Lifetime lifetime)
    {
        var lastCommitsContext = _store.BindToContextLive(lifetime, ContextRange.ApplicationWide)
            .GetValueProperty(lifetime, (GitPluginSettings settings) => settings.LastGitCommits);
        _lastCommits = lastCommitsContext.Value;
        RefreshHighlighting(daemon);
        lastCommitsContext.Change.Advise_HasNew(lifetime, () =>
        {
            _lastCommits = lastCommitsContext.Value;
            RefreshHighlighting(daemon);
        });
                
        FileSystemWatcher fsw = new FileSystemWatcher();
        fsw.Path = Path.Combine(_gitAbsolutePath, ".git", "logs");
        fsw.Filter = "HEAD";
        fsw.NotifyFilter = NotifyFilters.LastWrite;

        fsw.Changed += (sender, args) => RefreshHighlighting(daemon);
        fsw.EnableRaisingEvents = true;
    }
    private void RefreshHighlighting(IDaemon daemon)
    {
        LastCommitHashesAndMessages = GetLastCommitsHashes();
        daemon.Invalidate("Git changes");
    }
    private bool TryGetGitRepositoryPath(out string path)
    {
        (string gitOutput, string gitError) = CommandRunner.RunGitCommand("rev-parse --show-toplevel", _solution.SolutionDirectory.ToString());
        if (!gitError.IsNullOrEmpty())
        {
            path = null;
            return false;
        }
        
        path = gitOutput.TrimEnd('\n', '\r').Replace('/', Path.DirectorySeparatorChar);
        return Directory.Exists(path);
    }

    private Dictionary<string, string> GetLastCommitsHashes()
    {
        (string gitOutput, string gitError) = CommandRunner.RunGitCommand($"log -n {_lastCommits} --pretty=format:\"%H %s\"", _solution.SolutionDirectory.ToString());
        if (!gitError.IsNullOrEmpty())
        {
            Console.WriteLine("The error has occured while loading commits");
            return null;
        }
        
        const string pattern = @"^([a-f0-9]{7,40})\s+(.+)$";
        Regex hashAndMessageRegex = new Regex(pattern, RegexOptions.Multiline);
        Dictionary<string, string> temp = new Dictionary<string, string>();
        foreach (Match hashAndMessageEntry in hashAndMessageRegex.Matches(gitOutput))
        {
            string commitHash = hashAndMessageEntry.Groups[1].Value;
            string commitMessage = hashAndMessageEntry.Groups[2].Value;
            temp.Add(commitHash, commitMessage);
        }
        return temp;
    }
    
}