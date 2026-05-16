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
            try
            {
                var mgr = new UpdateManager(new GithubSource(RepoUrl, null, false));
                
                // Check for new version
                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                {
                    AnsiConsole.MarkupLine("[bold blue][[INFO]][/] IDE is up to date (Velopack).");
                    return;
                }

                AnsiConsole.MarkupLine($"[bold green][[UPDATE]][/] New version found: [yellow]{newVersion.TargetFullRelease.Version}[/]");
                onUpdateAvailable?.Invoke(newVersion.TargetFullRelease.Version.ToString());

                // Download new version
                AnsiConsole.MarkupLine("[bold blue][[INFO]][/] Downloading update in background...");
                await mgr.DownloadUpdatesAsync(newVersion);
                
                AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Update downloaded. It will be applied on next restart.");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold yellow][[WARN]][/] Velopack update check failed. Skipping.");
                // We don't write exception here to avoid cluttering if it's just a network/rate limit issue
            }
        }
    }
}