Git checker is loaded every time solution is opened - checks if solution is in git repository and if so loads last N commits(N given by user and 0 by default) and starts new Thread which looks for changes in settings(GitPluginSettings and GitPluginSettingsPage) and in
.git/logs/HEAD(for branch switch or new commit) and then invalidates the daemon
 Every time user opens a file new GitDaemonStage and GitDaemonStageProcess - which loads changes of an open file with git blame, if there were any in last N commits it creates GitHighlighting for each line that was affected.
