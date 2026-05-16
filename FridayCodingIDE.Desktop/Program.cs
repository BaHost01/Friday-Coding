using Avalonia;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.WebView.Desktop;
using Velopack;
using Spectre.Console;

namespace FridayCodingIDE.Desktop;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AllocConsole();

    [STAThread]
    public static void Main(string[] args)
    {
        AnsiConsole.MarkupLine("[bold blue][[SYSTEM]][/] Starting Friday-Coding IDE...");

        // Velopack.Must run as early as possible.
        try 
        {
            VelopackApp.Build().Run();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Velopack initialization failed.");
            AnsiConsole.WriteException(ex);
        }

        AnsiConsole.MarkupLine($"[bold blue][[SYSTEM]][/] OS detected: [yellow]{RuntimeInformation.OSDescription}[/]");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AnsiConsole.MarkupLine("[bold blue][[SYSTEM]][/] Allocating Console for Windows...");
            AllocConsole();
            OccultDlls();
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void OccultDlls()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            AnsiConsole.MarkupLine($"[bold blue][[FS]][/] Checking for visible DLLs in [grey]{baseDir.EscapeMarkup()}[/]");
            var dlls = Directory.GetFiles(baseDir, "*.dll");

            int hiddenCount = 0;
            foreach (var dll in dlls)
            {
                var attributes = File.GetAttributes(dll);
                if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    File.SetAttributes(dll, attributes | FileAttributes.Hidden);
                    AnsiConsole.MarkupLine($"[bold cyan][[FS]][/] Occulted: [grey]{Path.GetFileName(dll).EscapeMarkup()}[/]");
                    hiddenCount++;
                }
            }
            
            if (hiddenCount > 0)
                AnsiConsole.MarkupLine($"[bold green][[SUCCESS]][/] Occulted [yellow]{hiddenCount}[/] DLL files.");
            else
                AnsiConsole.MarkupLine("[bold blue][[FS]][/] No visible DLLs found to occult.");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Failed to occult DLLs.");
            AnsiConsole.WriteException(ex);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseDesktopWebView()
            .WithInterFont()
            .LogToTrace();
}