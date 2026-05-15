using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using FridayCodingIDE.Desktop.Services;
using FridayCodingIDE.Services;
using Newtonsoft.Json.Linq;
using AvaloniaWebView;

namespace FridayCodingIDE.Desktop
{
    public partial class MainWindow : Window
    {
        private ProjectManager _projectManager = new ProjectManager();

        public MainWindow()
        {
            InitializeComponent();
            
            // WebView initialization
            MainWebView.NavigationCompleted += (s, e) => UpdateFileList();
            MainWebView.WebMessageReceived += OnMessageReceived;

            // Load UI
            string uiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "UI", "index.html");
            if (File.Exists(uiPath))
            {
                MainWebView.Url = new Uri("file://" + uiPath);
            }

            // Auto Update Check
            Task.Run(() => AutoUpdater.CheckForUpdatesAsync(OnUpdateAvailable));
        }

        private void OnMessageReceived(object? sender, object e)
        {
            try
            {
                string? message = e.ToString(); 
                // Using reflection to get Message property if it exists
                var prop = e.GetType().GetProperty("Message");
                if (prop != null) message = prop.GetValue(e)?.ToString();
                
                var msg = JObject.Parse(message ?? "");
                string? action = msg["action"]?.ToString();

                if (action == "run_mod")
                {
                    RunMod();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bridge error: {ex.Message}");
            }
        }

        private void UpdateFileList()
        {
            string[] files = { "main.lua", "assets/images/bg.png" };
            string json = JArray.FromObject(files).ToString();
            // WebView.Avalonia specific way to send data if available
            MainWebView.ExecuteScriptAsync($"if(window.ide) window.ide.setProjectFiles({json})");
        }

        private void RunMod()
        {
            MainWebView.ExecuteScriptAsync("window.ide.appendLog('Launching Psych Engine...', 'success')");
        }

        private void OnUpdateAvailable(string newVersion)
        {
            Console.WriteLine($"Update available: {newVersion}");
            MainWebView.ExecuteScriptAsync($"window.ide.appendLog('New Update Available: {newVersion}', 'success')");
        }
    }
}
