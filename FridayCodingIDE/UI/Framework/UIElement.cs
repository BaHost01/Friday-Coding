using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FridayCodingIDE.UI.Framework
{
    public abstract class UIElement
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
        public Color BackgroundColor { get; set; } = Color.Transparent;

        protected List<UIElement> Children { get; } = new List<UIElement>();
        public UIElement Parent { get; set; }

        public event Action OnClick;
        public event Action OnMouseEnter;
        public event Action OnMouseLeave;

        protected bool IsMouseOver { get; private set; }
        private bool _wasMouseOver;

        public virtual void AddChild(UIElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public virtual void Update(GameTime gameTime, MouseState mouseState)
        {
            if (!IsEnabled || !IsVisible) return;

            Rectangle bounds = new Rectangle((int)GetAbsolutePosition().X, (int)GetAbsolutePosition().Y, (int)Size.X, (int)Size.Y);
            IsMouseOver = bounds.Contains(mouseState.Position);

            if (IsMouseOver && !_wasMouseOver) OnMouseEnter?.Invoke();
            if (!IsMouseOver && _wasMouseOver) OnMouseLeave?.Invoke();
            
            if (IsMouseOver && mouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                // This is a simplified click detection
            }

            _wasMouseOver = IsMouseOver;

            foreach (var child in Children)
            {
                child.Update(gameTime, mouseState);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            if (BackgroundColor != Color.Transparent)
            {
                // Draw background using a 1x1 white texture (should be provided by a global resource)
                // spriteBatch.Draw(whiteTexture, GetBounds(), BackgroundColor);
            }

            foreach (var child in Children)
            {
                child.Draw(spriteBatch);
            }
        }

        public Vector2 GetAbsolutePosition()
        {
            if (Parent == null) return Position;
            return Parent.GetAbsolutePosition() + Position;
        }

        public Rectangle GetBounds()
        {
            Vector2 absPos = GetAbsolutePosition();
            return new Rectangle((int)absPos.X, (int)absPos.Y, (int)Size.X, (int)Size.Y);
        }
    }
}
