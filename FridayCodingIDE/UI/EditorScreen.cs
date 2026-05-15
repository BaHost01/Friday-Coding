using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FridayCodingIDE.UI.Framework;
using FridayCodingIDE.Services;
using FridayCodingIDE.Models;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;

namespace FridayCodingIDE.UI
{
    public class EditorScreen : BaseScreen
    {
        private Canvas _canvas;
        private Panel _leftPanel;
        private Panel _centerPanel;
        private Panel _rightPanel;
        private Panel _bottomPanel;

        private VisualDesignerCanvas _visualDesigner;
        private CodeEditor _codeEditor;
        private PropertyGrid _propertyGrid;
        private List<string> _consoleLogs = new List<string>();

        public EditorScreen(Game1 game, ScreenManager screenManager) : base(game, screenManager) { }

        public override void LoadContent()
        {
            ResourceManager.Initialize(Game.GraphicsDevice);
            _canvas = new Canvas(Game.GraphicsDevice);

            int width = Game.GraphicsDevice.Viewport.Width;
            int height = Game.GraphicsDevice.Viewport.Height;
            int sideWidth = 250;
            int bottomHeight = 150;

            // Layout setup
            _leftPanel = new Panel { Position = Vector2.Zero, Size = new Vector2(sideWidth, height - bottomHeight), BackgroundColor = new Color(30, 30, 30) };
            _centerPanel = new Panel { Position = new Vector2(sideWidth, 0), Size = new Vector2(width - (sideWidth * 2), height - bottomHeight), BackgroundColor = new Color(20, 20, 20) };
            _rightPanel = new Panel { Position = new Vector2(width - sideWidth, 0), Size = new Vector2(sideWidth, height - bottomHeight), BackgroundColor = new Color(30, 30, 30) };
            _bottomPanel = new Panel { Position = new Vector2(0, height - bottomHeight), Size = new Vector2(width, bottomHeight), BackgroundColor = new Color(25, 25, 25) };

            _canvas.AddElement(_leftPanel);
            _canvas.AddElement(_centerPanel);
            _canvas.AddElement(_rightPanel);
            _canvas.AddElement(_bottomPanel);

            _visualDesigner = new VisualDesignerCanvas { Size = _centerPanel.Size, BackgroundColor = Color.Transparent };
            _centerPanel.AddChild(_visualDesigner);

            _codeEditor = new CodeEditor { Size = _centerPanel.Size, BackgroundColor = Color.Transparent, IsVisible = false };
            _centerPanel.AddChild(_codeEditor);

            _propertyGrid = new PropertyGrid { Size = _rightPanel.Size, BackgroundColor = Color.Transparent };
            _rightPanel.AddChild(_propertyGrid);

            // Toolbar/Run Button
            var runButton = new ButtonElement { Text = "RUN MOD", Position = new Vector2(width - 130, 10), Size = new Vector2(120, 40), BackgroundColor = new Color(0, 120, 215) };
            runButton.OnClick += RunMod;
            _bottomPanel.AddChild(runButton);

            // Templates
            _leftPanel.AddChild(new Label { Text = "TEMPLATES", Position = new Vector2(10, 10) });
            _leftPanel.AddChild(new TemplateItem("Sprite", Color.Cyan) { Position = new Vector2(10, 50), Size = new Vector2(230, 40) });
            _leftPanel.AddChild(new TemplateItem("Note", Color.Lime) { Position = new Vector2(10, 100), Size = new Vector2(230, 40) });
            
            Log("IDE Ready.");
        }

        private void RunMod()
        {
            Log("Generating Lua and Exporting...");
            
            string workspacePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            string psychModsPath = Path.Combine(workspacePath, "TestingEnvironment", "PsychEngine", "PsychEngine", "mods", "CurrentProject");
            
            try 
            {
                if (!Directory.Exists(psychModsPath)) Directory.CreateDirectory(psychModsPath);
                
                ExportManager.ExportMod(workspacePath, psychModsPath);
                
                string exePath = Path.Combine(workspacePath, "TestingEnvironment", "PsychEngine", "PsychEngine", "PsychEngine.exe");
                if (File.Exists(exePath))
                {
                    Log("Launching Psych Engine...");
                    Process.Start(new ProcessStartInfo(exePath) { WorkingDirectory = Path.GetDirectoryName(exePath) });
                }
                else
                {
                    Log("Error: PsychEngine.exe not found.");
                }
            }
            catch (Exception ex)
            {
                Log($"Run Failed: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            _consoleLogs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            if (_consoleLogs.Count > 5) _consoleLogs.RemoveAt(0);
        }

        public override void Update(GameTime gameTime)
        {
            _canvas.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            _canvas.Draw(spriteBatch);

            // Draw console logs in bottom panel
            var font = ResourceManager.FontSystem.GetFont(14);
            for (int i = 0; i < _consoleLogs.Count; i++)
            {
                font.DrawText(spriteBatch, _consoleLogs[i], _bottomPanel.GetAbsolutePosition() + new Vector2(10, 60 + (i * 18)), Color.Gray);
            }
        }
    }
}
