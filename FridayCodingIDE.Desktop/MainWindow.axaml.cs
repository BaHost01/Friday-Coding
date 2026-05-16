using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
            try
            {
                AnsiConsole.MarkupLine("[bold blue][[INFO]][/] Refreshing Project Explorer...");
                string[] files = { "main.lua", "assets/images/bg.png" };
                string json = JArray.FromObject(files).ToString();
                SafeExecuteScriptAsync($"if(window.ide) window.ide.setProjectFiles({json})");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Failed to update file list.");
                AnsiConsole.WriteException(ex);
            }
        }

        private void RunMod(string code)
        {
            AnsiConsole.MarkupLine("[bold yellow][[ACTION]][/] Executing Lua Script (Internal Engine)...");
            AnsiConsole.MarkupLine($"[grey][[LUA]][/] Script length: [yellow]{code.Length}[/] characters.");
            
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
                    AnsiConsole.MarkupLine("[bold yellow][[ACTION]][/] Initiating Psych Engine installation...");
                    string installDir = Path.Combine(AppContext.BaseDirectory, "Engine", "PsychEngine");
                    
                    if (!Directory.Exists(installDir)) 
                    {
                        Directory.CreateDirectory(installDir);
                        AnsiConsole.MarkupLine($"[bold blue][[FS]][/] Created directory: [grey]{installDir.EscapeMarkup()}[/]");
                    }

                    string zipPath = Path.Combine(AppContext.BaseDirectory, "psych_engine.zip");

                    await AnsiConsole.Status()
                        .Spinner(Spectre.Console.Spinner.Known.Dots)
                        .StartAsync("[bold yellow]Downloading Psych Engine...[/]", async ctx => {
                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Add("User-Agent", "Friday-Coding-IDE");
                                AnsiConsole.MarkupLine($"[bold blue][[NET]][/] GET: [grey]{url.EscapeMarkup()}[/]");
                                
                                var response = await client.GetAsync(url);
                                response.EnsureSuccessStatusCode();

                                using (var fs = new FileStream(zipPath, FileMode.Create))
                                {
                                    await response.Content.CopyToAsync(fs);
                                }
                            }
                            
                            AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Download complete.");
                            
                            ctx.Status("[bold yellow]Extracting Psych Engine...[/]");
                            SafeExecuteScriptAsync("window.ide.appendLog('Extracting Psych Engine...', 'progress', 'install-step')");
                            
                            if (Directory.Exists(installDir)) 
                            {
                                AnsiConsole.MarkupLine("[bold blue][[FS]][/] Cleaning up existing engine files...");
                                Directory.Delete(installDir, true);
                                Directory.CreateDirectory(installDir);
                            }
                            
                            AnsiConsole.MarkupLine($"[bold blue][[FS]][/] Extracting to [grey]{installDir.EscapeMarkup()}[/]");
                            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, installDir);
                            
                            File.Delete(zipPath);
                            AnsiConsole.MarkupLine("[bold blue][[FS]][/] Cleanup: Deleted temp zip file.");
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
                // Detailed debug logging for scripts (truncated for console clarity)
                string brief = script.Length > 50 ? script.Substring(0, 47) + "..." : script;
                AnsiConsole.MarkupLine($"[grey][[WEB]][/] Executing JS: [grey]{brief.EscapeMarkup()}[/]");
                
                await MainWebView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                // CRITICAL: Use AnsiConsole.WriteLine to avoid markup parsing on script content which might contain [ or ]
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Failed to execute script in WebView.");
                AnsiConsole.WriteLine($"Script Content: {script}");
                AnsiConsole.WriteException(ex);
            }
        }

        private void StartBackendServer()
        {
            try
            {
                AnsiConsole.MarkupLine("[bold blue][[SYSTEM]][/] Searching for free TCP Port...");
                int port = GetFreeTcpPort();
                AnsiConsole.MarkupLine($"[bold blue][[NET]][/] Allocated Local Port: [yellow]{port}[/]");

                string serverName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Server.exe" : "Server";
                string serverPath = Path.Combine(AppContext.BaseDirectory, serverName);
                
                if (File.Exists(serverPath))
                {
                    AnsiConsole.MarkupLine($"[bold blue][[SYSTEM]][/] Launching Backend: [grey]{serverPath.EscapeMarkup()}[/]");
                    
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
                    AnsiConsole.MarkupLine($"[bold green][[SUCCESS]][/] Server.exe started (PID: [yellow]{_serverProcess.Id}[/])");
                    _serverProcess.BeginOutputReadLine();
                    _serverProcess.BeginErrorReadLine();
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold yellow][[WARN]][/] Server.exe not found at path. Functional bridge unavailable.");
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
            AnsiConsole.MarkupLine("[bold blue][[SYSTEM]][/] Shutting down Friday-Coding IDE...");
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                AnsiConsole.MarkupLine("[bold yellow][[ACTION]][/] Terminating backend server process...");
                _serverProcess.Kill();
                AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Backend server terminated.");
            }
            AnsiConsole.MarkupLine("[bold blue][[SYSTEM]][/] Goodbye!");
            base.OnClosed(e);
        }
    }
}
