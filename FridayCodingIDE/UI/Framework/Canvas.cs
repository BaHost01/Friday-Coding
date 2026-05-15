using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FridayCodingIDE.UI.Framework
{
    public class Canvas
    {
        private List<UIElement> _rootElements = new List<UIElement>();
        private readonly GraphicsDevice _graphicsDevice;

        public Canvas(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void AddElement(UIElement element)
        {
            _rootElements.Add(element);
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            foreach (var element in _rootElements)
            {
                element.Update(gameTime, mouseState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in _rootElements)
            {
                element.Draw(spriteBatch);
            }
        }
    }
}
