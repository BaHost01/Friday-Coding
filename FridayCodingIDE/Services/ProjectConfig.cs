using System;

namespace FridayCodingIDE.Services
{
    public class ProjectConfig
    {
        public string ProjectName { get; set; } = "New FNF Mod";
        public string Version { get; set; } = "1.0.0";
        public string Author { get; set; } = "Unknown";
        public string MainScript { get; set; } = "main.lua";
        public DateTime LastModified { get; set; } = DateTime.Now;
    }
}
