using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace FridayCodingIDE.Samples
{
    /// <summary>
    /// Example of how to use Velopack for automatic updates in this IDE.
    /// </summary>
    public class VelopackExample
    {
        public async Task UpdateApp()
        {
            // 1. Initialize the UpdateManager with your GitHub repo
            var mgr = new UpdateManager(new GithubSource("https://github.com/BaHost01/Friday-Coding", null, false));

            // 2. Check for updates
            var newVersion = await mgr.CheckForUpdatesAsync();
            if (newVersion == null)
            {
                Console.WriteLine("No updates found.");
                return;
            }

            // 3. Download the update
            Console.WriteLine($"Downloading version {newVersion.TargetFullRelease.Version}...");
            await mgr.DownloadUpdatesAsync(newVersion);

            // 4. Apply the update (usually on next restart)
            Console.WriteLine("Update downloaded! Restart to apply.");
        }
    }
}