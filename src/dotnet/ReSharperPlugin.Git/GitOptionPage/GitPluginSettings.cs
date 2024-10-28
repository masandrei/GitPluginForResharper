using JetBrains.Application.Settings;
using JetBrains.Application.Settings.WellKnownRootKeys;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Util.Caches;

namespace ReSharperPlugin.Git;

[SettingsKey(Parent:typeof(EnvironmentSettings), Description:"Git ReSharper plugin settings")]
public class GitPluginSettings
{
    [SettingsEntry(0, "Number of last git commits to show")]
    public int LastGitCommits { get; set; }
}