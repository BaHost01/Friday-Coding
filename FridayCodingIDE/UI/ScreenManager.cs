using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FridayCodingIDE.UI
{
    public class ScreenManager
    {
        private IScreen _currentScreen;
        private readonly GraphicsDevice _graphicsDevice;

        public ScreenManager(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void ChangeScreen(IScreen newScreen)
        {
            _currentScreen = newScreen;
            _currentScreen.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            _currentScreen?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentScreen?.Draw(spriteBatch);
        }
    }
}
