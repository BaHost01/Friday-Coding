using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FridayCodingIDE.UI
{
    public class IntroLoadingScreen : BaseScreen
    {
        private float _timer;
        private float _rotation;
        private Texture2D _luaLogo;
        private Texture2D _gradient;
        private const float Duration = 2.0f;

        public IntroLoadingScreen(Game1 game, ScreenManager screenManager) : base(game, screenManager) { }

        public override void LoadContent()
        {
            // Create a 1x1 white texture for the background/gradient if assets aren't loaded via Content pipeline yet
            _luaLogo = new Texture2D(Game.GraphicsDevice, 100, 100);
            Color[] data = new Color[100 * 100];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Blue; // Placeholder blue for Lua
            _luaLogo.SetData(data);

            _gradient = new Texture2D(Game.GraphicsDevice, 1, 256);
            Color[] gradientData = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                float amount = i / 255f;
                gradientData[i] = Color.Lerp(Color.Black, Color.White, amount * 0.2f); // Subtle gradient
            }
            _gradient.SetData(gradientData);
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += delta;
            _rotation += delta * 5.0f; // Spin speed

            if (_timer >= Duration)
            {
                ScreenManager.ChangeScreen(new SetupLoadingScreen(Game, ScreenManager));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            // Draw Gradient Background
            spriteBatch.Draw(_gradient, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

            // Draw Spinning Logo in Middle
            Vector2 screenCenter = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            Vector2 origin = new Vector2(_luaLogo.Width / 2, _luaLogo.Height / 2);
            
            spriteBatch.Draw(_luaLogo, screenCenter, null, Color.White, _rotation, origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
