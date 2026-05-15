using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FridayCodingIDE.UI.Framework
{
    public class TemplateItem : DraggableElement
    {
        public string Name { get; set; }
        public Color IconColor { get; set; }

        public TemplateItem(string name, Color color)
        {
            Name = name;
            IconColor = color;
            BackgroundColor = new Color(50, 50, 50);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ResourceManager.WhiteTexture, GetBounds(), BackgroundColor);
            
            // Draw a small colored square representing the type
            Rectangle iconBounds = new Rectangle((int)GetAbsolutePosition().X + 5, (int)GetAbsolutePosition().Y + 5, 30, 30);
            spriteBatch.Draw(ResourceManager.WhiteTexture, iconBounds, IconColor);

            // Draw text using FontStashSharp
            var font = ResourceManager.FontSystem.GetFont(20);
            font.DrawText(spriteBatch, Name, GetAbsolutePosition() + new Vector2(45, 10), Color.White);

            base.Draw(spriteBatch);
        }
    }
}
