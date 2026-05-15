using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace FridayCodingIDE.UI
{
    public class SetupLoadingScreen : BaseScreen
    {
        private Texture2D _luaLogo;
        private Texture2D _psychLogo;
        private string _status = "Initializing Setup...";
        private bool _isComplete = false;
        private float _completionTimer = 0f;

        public SetupLoadingScreen(Game1 game, ScreenManager screenManager) : base(game, screenManager) { }

        public override void LoadContent()
        {
            // Placeholder logos
            _luaLogo = new Texture2D(Game.GraphicsDevice, 100, 100);
            Color[] luaData = new Color[100 * 100];
            for (int i = 0; i < luaData.Length; i++) luaData[i] = Color.Blue;
            _luaLogo.SetData(luaData);

            _psychLogo = new Texture2D(Game.GraphicsDevice, 100, 100);
            Color[] psychData = new Color[100 * 100];
            for (int i = 0; i < psychData.Length; i++) psychData[i] = Color.DarkRed;
            _psychLogo.SetData(psychData);

            // Start the async setup task
            Task.Run(SetupEnvironmentAsync);
        }

        private async Task SetupEnvironmentAsync()
        {
            string testingDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "TestingEnvironment", "PsychEngine"));
            string zipPath = Path.Combine(Path.GetTempPath(), "PsychEngine.zip");
            string downloadUrl = "https://gamebanana.com/dl/1406924";

            try
            {
                if (Directory.Exists(testingDir) && Directory.GetFileSystemEntries(testingDir).Length > 0)
                {
                    _status = "Testing environment already exists. Skipping download.";
                    await Task.Delay(1000);
                }
                else
                {
                    _status = "Downloading Psych Engine...";
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(downloadUrl);
                        using (var fs = new FileStream(zipPath, FileMode.Create))
                        {
                            await response.Content.CopyToAsync(fs);
                        }
                    }

                    _status = "Extracting Psych Engine...";
                    if (!Directory.Exists(testingDir)) Directory.CreateDirectory(testingDir);
                    ZipFile.ExtractToDirectory(zipPath, testingDir, true);
                    
                    File.Delete(zipPath);
                    _status = "Setup Complete!";
                }
                
                _isComplete = true;
            }
            catch (Exception ex)
            {
                _status = $"Error: {ex.Message}";
                Console.WriteLine(_status);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_isComplete)
            {
                _completionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_completionTimer >= 1.5f)
                {
                    ScreenManager.ChangeScreen(new EditorScreen(Game, ScreenManager));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            Vector2 screenCenter = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            
            // Draw Psych Logo (Left)
            spriteBatch.Draw(_psychLogo, new Vector2(screenCenter.X - 200, screenCenter.Y - 50), Color.White);

            // Draw Lua Logo (Right)
            spriteBatch.Draw(_luaLogo, new Vector2(screenCenter.X + 100, screenCenter.Y - 50), Color.White);

            // TODO: Draw status text (needs SpriteFont)
            // For now, we just log status to console and visualize with completion
        }
    }
}
