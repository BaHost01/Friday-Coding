using System.IO;
using System.Collections.Generic;

namespace FridayCodingIDE.Services
{
    public static class ExportManager
    {
        public static void ExportMod(string projectPath, string exportPath)
        {
            if (!Directory.Exists(exportPath)) Directory.CreateDirectory(exportPath);

            // Export logic: only scripts, events, and relevant assets
            string[] directoriesToExport = { "Lua", "CustomLua", "UserInterface" };
            
            foreach (var dir in directoriesToExport)
            {
                string sourceDir = Path.Combine(projectPath, dir);
                string targetDir = Path.Combine(exportPath, dir);

                if (Directory.Exists(sourceDir))
                {
                    CopyDirectory(sourceDir, targetDir);
                }
            }
            
            // Specifically export main.lua if it's in the root
            string mainLua = Path.Combine(projectPath, "main.lua");
            if (File.Exists(mainLua))
            {
                File.Copy(mainLua, Path.Combine(exportPath, "main.lua"), true);
            }
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string dest = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            foreach (string folder in Directory.GetDirectories(sourceDir))
            {
                string dest = Path.Combine(targetDir, Path.GetFileName(folder));
                CopyDirectory(folder, dest);
            }
        }
    }
}
