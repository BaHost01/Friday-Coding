using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FridayCodingIDE.UI.Framework
{
    public class ButtonElement : UIElement
    {
        public string Text { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.Gray;
        private Color _currentColor;

        public new event Action OnClick;

        private bool _isPressed;

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            base.Update(gameTime, mouseState);

            _currentColor = IsMouseOver ? HoverColor : BackgroundColor;

            if (IsMouseOver && mouseState.LeftButton == ButtonState.Pressed)
            {
                _isPressed = true;
            }

            if (_isPressed && mouseState.LeftButton == ButtonState.Released)
            {
                if (IsMouseOver)
                {
                    OnClick?.Invoke();
                }
                _isPressed = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            spriteBatch.Draw(ResourceManager.WhiteTexture, GetBounds(), _currentColor);

            if (!string.IsNullOrEmpty(Text))
            {
                var font = ResourceManager.FontSystem.GetFont(20);
                Vector2 textSize = font.MeasureString(Text);
                Vector2 textPos = GetAbsolutePosition() + (Size / 2) - (textSize / 2);
                font.DrawText(spriteBatch, Text, textPos, TextColor);
            }

            base.Draw(spriteBatch);
        }
    }
}
