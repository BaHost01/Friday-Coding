using FridayCodingIDE.Services;
using System.IO;
using System;

var projectManager = new ProjectManager();
string workspacePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));

try 
{
    if (!projectManager.LoadProject(workspacePath))
    {
        projectManager.InitializeNewProject(workspacePath, "Default FNF Mod Project");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Project management initialization failed: {ex.Message}");
}

try 
{
    using var game = new FridayCodingIDE.Game1();
    game.Run();
}
catch (Exception ex)
{
    // Expected to fail in CLI environments without a display
    Console.WriteLine($"IDE Graphical Interface could not start: {ex.Message}");
}
