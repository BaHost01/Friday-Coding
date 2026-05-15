using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FridayCodingIDE.UI.Framework
{
    public class DraggableElement : UIElement
    {
        public bool IsDragging { get; private set; }
        private Vector2 _dragOffset;
        private Vector2 _targetPosition;
        private bool _isReturning;

        public event Action OnDrop;

        public override void Update(GameTime gameTime, MouseState mouseState)
        {
            if (!IsEnabled || !IsVisible) return;

            if (IsMouseOver && mouseState.LeftButton == ButtonState.Pressed && !IsDragging && !_isReturning)
            {
                IsDragging = true;
                _dragOffset = mouseState.Position.ToVector2() - Position;
            }

            if (IsDragging)
            {
                Position = mouseState.Position.ToVector2() - _dragOffset;

                if (mouseState.LeftButton == ButtonState.Released)
                {
                    IsDragging = false;
                    _isReturning = true;
                    _targetPosition = Position; // In a real system, this would be the drop target or original pos
                    OnDrop?.Invoke();
                }
            }

            if (_isReturning)
            {
                // Simple lerp animation for dropping/returning
                // For now, let's just stay where dropped, but we can add return animation if needed
                _isReturning = false; 
            }

            base.Update(gameTime, mouseState);
        }
    }
}
