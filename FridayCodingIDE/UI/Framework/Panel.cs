using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FridayCodingIDE.UI.Framework
{
    public class Panel : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            // Draw background
            spriteBatch.Draw(ResourceManager.WhiteTexture, GetBounds(), BackgroundColor);

            // In a more advanced implementation, we would set up a ScissorRectangle here
            // to clip children to the panel's bounds.
            
            base.Draw(spriteBatch);
        }
    }
}
