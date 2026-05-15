using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;

namespace FridayCodingIDE.Desktop.Services
{
    public static class AutoUpdater
    {
        private const string CurrentVersion = "v1.0.0";
        private const string RepoUrl = "https://api.github.com/repos/BaHost01/Friday-Coding/releases/latest";

        public static async Task CheckForUpdatesAsync(Action<string> onUpdateAvailable)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Friday-Coding-IDE");
                    var response = await client.GetStringAsync(RepoUrl);
                    var release = JObject.Parse(response);
                    string? latestTag = release["tag_name"]?.ToString();

                    if (!string.IsNullOrEmpty(latestTag) && latestTag != CurrentVersion)
                    {
                        onUpdateAvailable?.Invoke(latestTag);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update check failed: {ex.Message}");
            }
        }
    }
}
