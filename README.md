# Git for Rider and ReSharper

[![Rider](https://img.shields.io/jetbrains/plugin/v/RIDER_PLUGIN_ID.svg?label=Rider&colorB=0A7BBB&style=for-the-badge&logo=rider)](https://plugins.jetbrains.com/plugin/RIDER_PLUGIN_ID)
[![ReSharper](https://img.shields.io/jetbrains/plugin/v/RESHARPER_PLUGIN_ID.svg?label=ReSharper&colorB=0A7BBB&style=for-the-badge&logo=resharper)](https://plugins.jetbrains.com/plugin/RESHARPER_PLUGIN_ID)
#   G i t P l u g i n F o r R e s h a r p e r 
 
 Git checker is loaded every time solution is opened - checks if solution is in git repository and if so loads last N commits(N given by user and 0 by default) and starts new Thread which looks for changes in settings(GitPluginSettings and GitPluginSettingsPage) and in .git/logs/HEAD(for branch switch or new commit) and then invalidates the daemon
 Every time user opens a file new GitDaemonStage and GitDaemonStageProcess - which loads changes of an open file with git blame, if there were any in last N commits it creates GitHighlighting for each line that was affected.
