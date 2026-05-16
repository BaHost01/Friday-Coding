using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using FridayCodingIDE.Desktop.Services;
using FridayCodingIDE.Services;
using Newtonsoft.Json.Linq;
using AvaloniaWebView;
using Spectre.Console;

namespace FridayCodingIDE.Desktop
{
    public partial class MainWindow : Window
    {
        private ProjectManager _projectManager = new ProjectManager();

        public MainWindow()
        {
            InitializeComponent();
            
            AnsiConsole.MarkupLine("[bold blue][INFO][/] Initializing Friday-Coding IDE...");

            // WebView initialization
            MainWebView.NavigationCompleted += (s, e) => {
                AnsiConsole.MarkupLine("[bold green][SUCCESS][/] WebView Navigation Completed.");
                UpdateFileList();
            };
            MainWebView.WebMessageReceived += OnMessageReceived;

            // Load UI
            string uiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "UI", "index.html");
            if (File.Exists(uiPath))
            {
                AnsiConsole.MarkupLine($"[bold blue][INFO][/] Loading UI from: [yellow]{uiPath}[/]");
                MainWebView.Url = new Uri("file://" + uiPath);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red][ERROR][/] UI file not found!");
            }

            // Auto Update Check
            Task.Run(async () => {
                try 
                {
                    await AutoUpdater.CheckForUpdatesAsync(OnUpdateAvailable);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine("[bold red][ERROR][/] AutoUpdater failed.");
                    AnsiConsole.WriteException(ex);
                }
            });
        }

        private void OnMessageReceived(object? sender, object e)
        {
            try
            {
                string? message = e.ToString(); 
                // Using reflection to get Message property if it exists
                var prop = e.GetType().GetProperty("Message");
                if (prop != null) message = prop.GetValue(e)?.ToString();
                
                if (string.IsNullOrEmpty(message)) return;

                AnsiConsole.MarkupLine($"[bold cyan][BRIDGE][/] Received: [grey]{message.EscapeMarkup()}[/]");
                
                var msg = JObject.Parse(message);
                string? action = msg["action"]?.ToString();

                if (action == "run_mod")
                {
                    RunMod();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][ERROR][/] Bridge error occurred.");
                AnsiConsole.WriteException(ex);
            }
        }

        private void UpdateFileList()
        {
            string[] files = { "main.lua", "assets/images/bg.png" };
            string json = JArray.FromObject(files).ToString();
            SafeExecuteScriptAsync($"if(window.ide) window.ide.setProjectFiles({json})");
        }

        private void RunMod()
        {
            AnsiConsole.MarkupLine("[bold yellow][ACTION][/] Running Mod...");
            SafeExecuteScriptAsync("window.ide.appendLog('Launching Psych Engine...', 'success')");
        }

        private void OnUpdateAvailable(string newVersion)
        {
            AnsiConsole.MarkupLine($"[bold green][UPDATE][/] New Version Available: [yellow]{newVersion}[/]");
            SafeExecuteScriptAsync($"window.ide.appendLog('New Update Available: {newVersion}', 'success')");
        }

        /// <summary>
        /// Executes a script in the WebView safely by catching and logging any exceptions.
        /// </summary>
        private async void SafeExecuteScriptAsync(string script)
        {
            try
            {
                await MainWebView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][ERROR][/] Failed to execute script in WebView.");
                AnsiConsole.MarkupLine($"[grey]Script: {script.EscapeMarkup()}[/]");
                AnsiConsole.WriteException(ex);
            }
        }
    }
}
