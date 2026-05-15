using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FridayCodingIDE.UI.Framework;
using FridayCodingIDE.Models;
using System.Collections.Generic;

namespace FridayCodingIDE.UI
{
    public class PropertyGrid : Panel
    {
        private FNFObjectData _target;
        private List<TextInputElement> _inputs = new List<TextInputElement>();

        public void SetTarget(FNFObjectData target)
        {
            _target = target;
            Refresh();
        }

        private void Refresh()
        {
            Children.Clear();
            if (_target == null) return;

            var font = ResourceManager.FontSystem.GetFont(18);
            
            // Tag
            AddPropertyField("Tag:", _target.Tag, 50, (val) => _target.Tag = val);
            // X
            AddPropertyField("X:", _target.Position.X.ToString(), 100, (val) => {
                if (float.TryParse(val, out float res)) _target.Position = new Vector2(res, _target.Position.Y);
            });
            // Y
            AddPropertyField("Y:", _target.Position.Y.ToString(), 150, (val) => {
                if (float.TryParse(val, out float res)) _target.Position = new Vector2(_target.Position.X, res);
            });
        }

        private void AddPropertyField(string label, string value, int y, System.Action<string> onUpdate)
        {
            var labelElem = new Label { Text = label, Position = new Vector2(10, y) };
            var input = new TextInputElement
            {
                Text = value,
                Position = new Vector2(80, y),
                Size = new Vector2(150, 30),
                BackgroundColor = new Color(50, 50, 50)
            };
            // In a real system, we'd trigger onUpdate when the text changes or on Enter
            AddChild(labelElem);
            AddChild(input);
        }
    }

    public class Label : UIElement
    {
        public string Text { get; set; }
        public override void Draw(SpriteBatch spriteBatch)
        {
            var font = ResourceManager.FontSystem.GetFont(18);
            font.DrawText(spriteBatch, Text, GetAbsolutePosition(), Color.White);
        }
    }
}
