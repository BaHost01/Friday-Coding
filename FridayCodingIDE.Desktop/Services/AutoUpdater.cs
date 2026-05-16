using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;
using Spectre.Console;

namespace FridayCodingIDE.Desktop.Services
{
    public static class AutoUpdater
    {
        private const string RepoUrl = "https://github.com/BaHost01/Friday-Coding";

        public static async Task CheckForUpdatesAsync(Action<string> onUpdateAvailable)
        {
            // Only check for updates if we are running as a deployed app
            // Velopack sources often throw if running from 'dotnet run' or generic bin/
            try
            {
                var mgr = new UpdateManager(new GithubSource(RepoUrl, null, false));
                
                if (!mgr.IsInstalled)
                {
                    AnsiConsole.MarkupLine("[bold blue][[INFO]][/] Running in portable/debug mode. Update check skipped.");
                    return;
                }

                // Check for new version
                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                {
                    AnsiConsole.MarkupLine("[bold blue][[INFO]][/] IDE is up to date.");
                    return;
                }

                AnsiConsole.MarkupLine($"[bold green][[UPDATE]][/] New version found: [yellow]{newVersion.TargetFullRelease.Version}[/]");
                onUpdateAvailable?.Invoke(newVersion.TargetFullRelease.Version.ToString());

                // Download new version
                AnsiConsole.MarkupLine("[bold blue][[INFO]][/] Downloading update in background...");
                await mgr.DownloadUpdatesAsync(newVersion);
                
                AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Update downloaded. It will be applied on next restart.");
            }
            catch (Exception)
            {
                // Silence update errors in local runs unless explicitly requested
            }
        }
    }
}