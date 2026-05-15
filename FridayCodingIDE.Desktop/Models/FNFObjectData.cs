using System.Numerics;
using System;

namespace FridayCodingIDE.Models
{
    public class FNFObjectData
    {
        public string Tag { get; set; } = "object_" + Guid.NewGuid().ToString().Substring(0, 4);
        public string ImagePath { get; set; } = "unknown";
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Alpha { get; set; } = 1.0f;
        public bool Antialiasing { get; set; } = true;
        public string Camera { get; set; } = "camGame"; // camGame, camHUD, camOther

        public FNFObjectData(string type)
        {
            Tag = type.ToLower() + "_" + Guid.NewGuid().ToString().Substring(0, 4);
        }
    }
}
