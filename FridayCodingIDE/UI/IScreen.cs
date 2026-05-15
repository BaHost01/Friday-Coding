using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FridayCodingIDE.UI
{
    public interface IScreen
    {
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
