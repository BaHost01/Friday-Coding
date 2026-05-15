using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FridayCodingIDE.UI.Framework;
using FridayCodingIDE.Models;
using System.Collections.Generic;

namespace FridayCodingIDE.UI
{
    public class VisualDesignerCanvas : UIElement
    {
        private List<FNFObjectData> _objects = new List<FNFObjectData>();
        public FNFObjectData SelectedObject { get; private set; }

        public void AddObject(FNFObjectData data)
        {
            _objects.Add(data);
            SelectedObject = data;
        }

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            base.Update(gameTime, mouseState);
            
            // Handle clicking objects to select them
            if (IsMouseOver && mouseState.LeftButton == ButtonState.Pressed)
            {
                // In a full implementation, we'd hit-test each object
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ResourceManager.WhiteTexture, GetBounds(), BackgroundColor);

            foreach (var obj in _objects)
            {
                // Draw a placeholder for the sprite
                Vector2 drawPos = GetAbsolutePosition() + obj.Position;
                Rectangle rect = new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)(100 * obj.Scale.X), (int)(100 * obj.Scale.Y));
                spriteBatch.Draw(ResourceManager.WhiteTexture, rect, Color.White * obj.Alpha);
                
                // If selected, draw a border
                if (obj == SelectedObject)
                {
                    DrawSelectionBorder(spriteBatch, rect);
                }
            }

            base.Draw(spriteBatch);
        }

        private void DrawSelectionBorder(SpriteBatch spriteBatch, Rectangle bounds)
        {
            int t = 2;
            Color c = Color.Yellow;
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X - t, bounds.Y - t, bounds.Width + t * 2, t), c);
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X - t, bounds.Y + bounds.Height, bounds.Width + t * 2, t), c);
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X - t, bounds.Y - t, t, bounds.Height + t * 2), c);
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X + bounds.Width, bounds.Y - t, t, bounds.Height + t * 2), c);
        }
    }
}
