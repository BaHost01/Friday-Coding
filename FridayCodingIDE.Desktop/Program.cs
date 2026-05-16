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
        VelopackApp.Build().Run();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
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
            var dlls = Directory.GetFiles(baseDir, "*.dll");

            foreach (var dll in dlls)
            {
                var attributes = File.GetAttributes(dll);
                if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    File.SetAttributes(dll, attributes | FileAttributes.Hidden);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to occult DLLs: {ex.Message}");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseDesktopWebView()
            .WithInterFont()
            .LogToTrace();
}