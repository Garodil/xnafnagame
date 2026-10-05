using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace model.Models.Game
{
    public class LevelBackground
    {
        private readonly Size _size;
        private readonly Texture2D _texture;
        private readonly Vector2 _scale;
        private readonly Vector2 _position;

        public LevelBackground(Size size, Texture2D texture, Vector2 scale, Vector2 position)
        {
            _size = size;
            _texture = texture;
            _scale = scale;
            _position = position;
        }

        public Size Size { get => _size; }
        public Texture2D Texture { get => _texture; }
        public Vector2 Scale { get => _scale; }
        public Vector2 Position { get => _position; }
    }
}