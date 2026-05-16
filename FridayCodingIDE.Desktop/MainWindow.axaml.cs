using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using FridayCodingIDE.Desktop.Services;
using FridayCodingIDE.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AvaloniaWebView;
using Spectre.Console;

namespace FridayCodingIDE.Desktop
{
    public partial class MainWindow : Window
    {
        private ProjectManager _projectManager = new ProjectManager();
        private LuaService _luaService = new LuaService();
        private Process? _serverProcess;

        public MainWindow()
        {
            InitializeComponent();
            
            AnsiConsole.MarkupLine("[bold blue][[INFO]][/] Initializing Friday-Coding IDE...");

            // Initialize/Load Default Project
            string defaultProjectPath = Path.Combine(AppContext.BaseDirectory, "Projects", "DefaultMod");
            _projectManager.LoadProject(defaultProjectPath);

            // Start Backend Server
            StartBackendServer();

            // WebView initialization
            MainWebView.NavigationCompleted += (s, e) => {
                AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] WebView Navigation Completed.");
                UpdateFileList();
            };
            MainWebView.WebMessageReceived += OnMessageReceived;

            // Load UI
            string uiPath = Path.Combine(AppContext.BaseDirectory, "Assets", "UI", "index.html");
            if (File.Exists(uiPath))
            {
                AnsiConsole.MarkupLine($"[bold blue][[INFO]][/] Loading UI from: [yellow]{uiPath.EscapeMarkup()}[/]");
                MainWebView.Url = new Uri("file://" + uiPath);
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] UI file not found!");
            }

            // Auto Update Check
            Task.Run(async () => await AutoUpdater.CheckForUpdatesAsync(OnUpdateAvailable));
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

                AnsiConsole.MarkupLine($"[bold cyan][[BRIDGE]][/] Received: [grey]{message.EscapeMarkup()}[/]");
                
                var msg = JObject.Parse(message);
                string? action = msg["action"]?.ToString();

                if (action == "run_mod")
                {
                    string? code = msg["code"]?.ToString();
                    RunMod(code ?? "");
                }
                else if (action == "install_psych")
                {
                    string? url = msg["url"]?.ToString();
                    InstallPsych(url ?? "");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Bridge error occurred.");
                AnsiConsole.WriteException(ex);
            }
        }

        private void UpdateFileList()
        {
            string[] files = { "main.lua", "assets/images/bg.png" };
            string json = JArray.FromObject(files).ToString();
            SafeExecuteScriptAsync($"if(window.ide) window.ide.setProjectFiles({json})");
        }

        private void RunMod(string code)
        {
            AnsiConsole.MarkupLine("[bold yellow][[ACTION]][/] Executing Lua Script...");
            _luaService.ExecuteScript(code, (output) => {
                AnsiConsole.MarkupLine($"[grey][[LUA]][/] {output.EscapeMarkup()}");
                SafeExecuteScriptAsync($"window.ide.appendLog({JsonConvert.SerializeObject(output)}, 'success')");
            });
        }

        private void OnUpdateAvailable(string newVersion)
        {
            AnsiConsole.MarkupLine($"[bold green][[UPDATE]][/] New Version Available: [yellow]{newVersion}[/]");
            SafeExecuteScriptAsync($"window.ide.appendLog('New Update Available: {newVersion}', 'success')");
        }

        private void InstallPsych(string url)
        {
            Task.Run(async () => {
                try
                {
                    await AnsiConsole.Status()
                        .Spinner(Spectre.Console.Spinner.Known.Dots)
                        .StartAsync("[bold yellow]Installing Psych Engine...[/]", async ctx => {
                            AnsiConsole.MarkupLine($"[bold blue][[INFO]][/] Downloading from: [grey]{url.EscapeMarkup()}[/]");
                            await Task.Delay(3000); // Simulate download
                            
                            ctx.Status("[bold yellow]Extracting Assets...[/]");
                            SafeExecuteScriptAsync("window.ide.appendLog('Extracting Assets...', 'progress', 'install-step')");
                            await Task.Delay(2000); // Simulate extraction
                            
                            ctx.Status("[bold yellow]Finalizing Installation...[/]");
                            SafeExecuteScriptAsync("window.ide.appendLog('Finalizing...', 'progress', 'install-step')");
                            await Task.Delay(1500);
                        });

                    AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Psych Engine Installed successfully.");
                    SafeExecuteScriptAsync("window.ide.appendLog('Psych Engine Installed!', 'success', 'install-step')");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Installation failed.");
                    AnsiConsole.WriteException(ex);
                    SafeExecuteScriptAsync("window.ide.appendLog('Installation Failed!', 'error', 'install-step')");
                }
            });
        }

        private async void SafeExecuteScriptAsync(string script)
        {
            try
            {
                await MainWebView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Failed to execute script in WebView.");
                AnsiConsole.MarkupLine($"[grey]Script: {script.EscapeMarkup()}[/]");
                AnsiConsole.WriteException(ex);
            }
        }

        private void StartBackendServer()
        {
            try
            {
                int port = GetFreeTcpPort();
                AnsiConsole.MarkupLine($"[bold blue][[INFO]][/] Allocated Port: [yellow]{port}[/]");

                string serverPath = Path.Combine(AppContext.BaseDirectory, "Server.exe");
                if (File.Exists(serverPath))
                {
                    AnsiConsole.MarkupLine("[bold blue][[INFO]][/] Launching Server.exe...");
                    
                    _serverProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = serverPath,
                            Arguments = $"--port {port}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    _serverProcess.OutputDataReceived += (s, e) => {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            AnsiConsole.MarkupLine($"[grey][[SERVER]][/] {e.Data.EscapeMarkup()}");
                            SafeExecuteScriptAsync($"window.ide.appendLog({JsonConvert.SerializeObject("[SERVER] " + e.Data)}, 'progress')");
                        }
                    };

                    _serverProcess.ErrorDataReceived += (s, e) => {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            AnsiConsole.MarkupLine($"[bold red][[SERVER ERROR]][/] {e.Data.EscapeMarkup()}");
                            SafeExecuteScriptAsync($"window.ide.appendLog({JsonConvert.SerializeObject("[SERVER ERROR] " + e.Data)}, 'error')");
                        }
                    };

                    _serverProcess.Start();
                    _serverProcess.BeginOutputReadLine();
                    _serverProcess.BeginErrorReadLine();
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold yellow][[WARN]][/] Server.exe not found. Running in standalone mode.");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Failed to start Backend Server.");
                AnsiConsole.WriteException(ex);
            }
        }

        private int GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                _serverProcess.Kill();
            }
            base.OnClosed(e);
        }
    }
}
