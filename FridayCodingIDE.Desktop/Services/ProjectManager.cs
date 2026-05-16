using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Spectre.Console;

namespace FridayCodingIDE.Services
{
    public class ProjectManager
    {
        private static readonly string[] RequiredDirectories = new[]
        {
            "Lua",
            "CustomLua",
            "CustomBytecode",
            "Exporting",
            "Shared",
            "SyntaxChecking",
            "Templates",
            "TestingEnvironment",
            "UserInterface",
            "assets/images",
            "assets/sounds",
            "assets/music",
            "assets/data",
            "assets/songs"
        };

        private const string ConfigFileName = "project.json";

        public ProjectConfig CurrentProject { get; private set; } = new ProjectConfig();
        public string CurrentProjectPath { get; private set; } = string.Empty;

        public bool InitializeNewProject(string path, string name)
        {
            try
            {
                AnsiConsole.MarkupLine($"[bold blue][[INFO]][/] Initializing new project: [yellow]{name.EscapeMarkup()}[/] at [grey]{path.EscapeMarkup()}[/]");
                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Created project root directory.");
                }

                ValidateDirectories(path);
                EnsureDefaultFiles(path);

                var config = new ProjectConfig
                {
                    ProjectName = name,
                    LastModified = DateTime.Now
                };

                SaveProject(path, config);
                CurrentProject = config;
                CurrentProjectPath = path;

                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Error initializing project.");
                AnsiConsole.WriteException(ex);
                return false;
            }
        }

        public bool LoadProject(string path)
        {
            try
            {
                AnsiConsole.MarkupLine($"[bold blue][[INFO]][/] Loading project from: [grey]{path.EscapeMarkup()}[/]");
                
                string configPath = Path.Combine(path, ConfigFileName);
                if (!File.Exists(configPath))
                {
                    AnsiConsole.MarkupLine("[bold yellow][[WARN]][/] project.json not found. Creating default config.");
                    InitializeNewProject(path, "Recovered Project");
                    return true;
                }

                string json = File.ReadAllText(configPath);
                CurrentProject = JsonSerializer.Deserialize<ProjectConfig>(json) ?? new ProjectConfig();
                CurrentProjectPath = path;

                ValidateDirectories(path);
                EnsureDefaultFiles(path);

                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Error loading project.");
                AnsiConsole.WriteException(ex);
                return false;
            }
        }

        public void SaveProject(string path, ProjectConfig config)
        {
            try
            {
                string configPath = Path.Combine(path, ConfigFileName);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(configPath, json);
                AnsiConsole.MarkupLine("[bold green][[SUCCESS]][/] Project configuration saved.");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[bold red][[ERROR]][/] Failed to save project configuration.");
                AnsiConsole.WriteException(ex);
            }
        }

        public void ValidateDirectories(string basePath)
        {
            foreach (var dir in RequiredDirectories)
            {
                string dirPath = Path.Combine(basePath, dir);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                    AnsiConsole.MarkupLine($"[bold cyan][[FS]][/] Created missing directory: [grey]{dir.EscapeMarkup()}[/]");
                }
            }
        }

        public void EnsureDefaultFiles(string basePath)
        {
            // Ensure main.lua exists
            string mainLuaPath = Path.Combine(basePath, "Lua", "main.lua");
            if (!File.Exists(mainLuaPath))
            {
                string defaultLua = "-- Friday-Coding IDE Default Script\n\nfunction onCreate()\n    -- Called when the script starts\n    debugPrint('Hello from Friday-Coding!')\nend\n";
                File.WriteAllText(mainLuaPath, defaultLua);
                AnsiConsole.MarkupLine("[bold cyan][[FS]][/] Created default [grey]main.lua[/]");
            }

            // Ensure a basic .gitkeep in empty directories if needed, or just specific files
            // For example, a placeholder background if it's missing (as referenced in MainWindow)
            string placeholderBg = Path.Combine(basePath, "assets", "images", "bg.png");
            if (!File.Exists(placeholderBg))
            {
                // We can't easily create a PNG here without a library, but we can touch it or skip
                // For now, let's just log that it's missing or create a dummy file if appropriate
                // File.WriteAllBytes(placeholderBg, new byte[0]); // Dummy empty file
            }
        }
    }
}
