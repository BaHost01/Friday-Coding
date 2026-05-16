using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using Spectre.Console;

namespace FridayCodingIDE.Desktop.Services
{
    public static class AutoUpdater
    {
        private const string CurrentVersion = "v1.1.1";
        private const string RepoUrl = "https://api.github.com/repos/BaHost01/Friday-Coding/releases/latest";

        public static async Task CheckForUpdatesAsync(Action<string> onUpdateAvailable)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Friday-Coding-IDE");
                    
                    var response = await client.GetAsync(RepoUrl);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        AnsiConsole.MarkupLine("[bold yellow][[WARN]][/] GitHub API rate limit exceeded. Skipping update check.");
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                    
                    var content = await response.Content.ReadAsStringAsync();
                    var release = JObject.Parse(content);
                    string? latestTag = release["tag_name"]?.ToString();

                    if (!string.IsNullOrEmpty(latestTag) && latestTag != CurrentVersion)
                    {
                        AnsiConsole.MarkupLine($"[bold green][[UPDATE]][/] New version available: [yellow]{latestTag}[/]");
                        onUpdateAvailable?.Invoke(latestTag);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[bold blue][[INFO]][/] IDE is up to date.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                AnsiConsole.MarkupLine($"[bold yellow][[WARN]][/] Update check failed (Network): [grey]{ex.Message.EscapeMarkup()}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Unexpected error during update check.");
                AnsiConsole.WriteException(ex);
            }
        }
    }
}
