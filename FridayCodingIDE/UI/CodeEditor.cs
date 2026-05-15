using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FridayCodingIDE.UI.Framework;

namespace FridayCodingIDE.UI
{
    public class CodeEditor : Panel
    {
        private TextInputElement _textInput;

        public string Code 
        { 
            get => _textInput.Text; 
            set => _textInput.Text = value; 
        }

        public CodeEditor()
        {
            _textInput = new TextInputElement
            {
                Position = new Vector2(10, 10),
                BackgroundColor = Color.Transparent, // Panel handles background
                BorderColor = Color.Transparent
            };
            AddChild(_textInput);
        }

        public override void Update(GameTime gameTime, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            // Code editor should fill its panel
            _textInput.Size = Size - new Vector2(20, 20);
            base.Update(gameTime, mouseState);
        }
    }
}
