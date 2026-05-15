using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;

namespace FridayCodingIDE.UI.Framework
{
    public class TextInputElement : UIElement
    {
        private StringBuilder _text = new StringBuilder();
        public string Text 
        { 
            get => _text.ToString(); 
            set { _text.Clear(); _text.Append(value); } 
        }

        public bool IsFocused { get; set; }
        public Color BorderColor { get; set; } = Color.White;

        private KeyboardState _prevKeyState;

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            base.Update(gameTime, mouseState);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsFocused = IsMouseOver;
            }

            if (IsFocused)
            {
                KeyboardState keyState = Keyboard.GetState();
                
                // Very basic keyboard handling
                foreach (var key in keyState.GetPressedKeys())
                {
                    if (_prevKeyState.IsKeyUp(key))
                    {
                        if (key == Keys.Back && _text.Length > 0)
                        {
                            _text.Remove(_text.Length - 1, 1);
                        }
                        else if (key == Keys.Space)
                        {
                            _text.Append(" ");
                        }
                        else if (key >= Keys.A && key <= Keys.Z)
                        {
                            _text.Append(key.ToString().ToLower());
                        }
                        // Add more keys as needed...
                    }
                }
                _prevKeyState = keyState;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            Rectangle bounds = GetBounds();
            spriteBatch.Draw(ResourceManager.WhiteTexture, bounds, BackgroundColor);
            
            // Draw Border
            if (IsFocused)
            {
                // Simple border drawing
                DrawBorder(spriteBatch, bounds, 2, BorderColor);
            }

            var font = ResourceManager.FontSystem.GetFont(18);
            font.DrawText(spriteBatch, Text, GetAbsolutePosition() + new Vector2(5, 5), Color.White);

            base.Draw(spriteBatch);
        }

        private void DrawBorder(SpriteBatch spriteBatch, Rectangle bounds, int thickness, Color color)
        {
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X, bounds.Y + bounds.Height - thickness, bounds.Width, thickness), color);
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);
            spriteBatch.Draw(ResourceManager.WhiteTexture, new Rectangle(bounds.X + bounds.Width - thickness, bounds.Y, thickness, bounds.Height), color);
        }
    }
}
