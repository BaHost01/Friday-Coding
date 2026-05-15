using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

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
            "UserInterface"
        };

        private const string ConfigFileName = "project.json";

        public ProjectConfig CurrentProject { get; private set; } = new ProjectConfig();
        public string CurrentProjectPath { get; private set; } = string.Empty;

        public bool InitializeNewProject(string path, string name)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                ValidateDirectories(path);

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
                Console.WriteLine($"Error initializing project: {ex.Message}");
                return false;
            }
        }

        public bool LoadProject(string path)
        {
            try
            {
                string configPath = Path.Combine(path, ConfigFileName);
                if (!File.Exists(configPath))
                {
                    return false;
                }

                string json = File.ReadAllText(configPath);
                CurrentProject = JsonSerializer.Deserialize<ProjectConfig>(json) ?? new ProjectConfig();
                CurrentProjectPath = path;

                ValidateDirectories(path);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading project: {ex.Message}");
                return false;
            }
        }

        public void SaveProject(string path, ProjectConfig config)
        {
            string configPath = Path.Combine(path, ConfigFileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configPath, json);
        }

        public void ValidateDirectories(string basePath)
        {
            foreach (var dir in RequiredDirectories)
            {
                string dirPath = Path.Combine(basePath, dir);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
        }
    }
}
