using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ReSharperPlugin.Git;

public static class CommandRunner
{
    public static (string, string) RunGitCommand(string commandArgs, string workingDirectory)
    {
        if (!Directory.Exists(workingDirectory))
        {
            throw new InvalidOperationException("Not a valid directory");
        }
        StringBuilder gitOutput = new StringBuilder();
        StringBuilder gitError = new StringBuilder();
        Process getGitCommits = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = commandArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            }
        };
        getGitCommits.OutputDataReceived += (sender, args) =>
        {
            if (args.Data is not null)
            {
                gitOutput.AppendLine(args.Data);
            }
        };
        getGitCommits.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data is not null)
            {
                gitError.AppendLine(args.Data);
            }
        };
        getGitCommits.Start();
        getGitCommits.BeginOutputReadLine();
        getGitCommits.BeginErrorReadLine();
        getGitCommits.WaitForExit();
        if (getGitCommits.ExitCode != 0)
        {
            return (String.Empty, String.Empty);
        }
        return (gitOutput.ToString(), gitError.ToString());
    }
}