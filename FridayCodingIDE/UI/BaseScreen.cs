using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FridayCodingIDE.UI
{
    public abstract class BaseScreen : IScreen
    {
        protected readonly Game1 Game;
        protected readonly ScreenManager ScreenManager;

        protected BaseScreen(Game1 game, ScreenManager screenManager)
        {
            Game = game;
            ScreenManager = screenManager;
        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
