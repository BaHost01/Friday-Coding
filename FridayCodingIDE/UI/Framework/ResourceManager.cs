using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using System.IO;
using System;

namespace FridayCodingIDE.UI.Framework
{
    public static class ResourceManager
    {
        public static Texture2D WhiteTexture { get; private set; }
        public static FontSystem FontSystem { get; private set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            WhiteTexture = new Texture2D(graphicsDevice, 1, 1);
            WhiteTexture.SetData(new[] { Color.White });

            FontSystem = new FontSystem();
            
            // Try to load a font from Psych Engine assets if available
            string fontPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "TestingEnvironment", "PsychEngine", "PsychEngine", "assets", "fonts", "vcr.ttf"));
            if (File.Exists(fontPath))
            {
                FontSystem.AddFont(File.ReadAllBytes(fontPath));
            }
        }
    }
}
