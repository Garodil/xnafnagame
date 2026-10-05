using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Models.Map;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace model.Models.Game.Tilemap
{
    public class Tile
    {
        private readonly Size _size;
        private readonly Texture2D _texture;
        private readonly Rectangle? _atlasPosition;
        private readonly Vector2 _position;
        private readonly Microsoft.Xna.Framework.Point _gridPosition;
        private readonly TileCollisionEnum _tileCollision;

        public Tile(Size size, Texture2D texture, Vector2 position, TileCollisionEnum tileCollision, Rectangle? atlasPosition = null)
        {
            _size = size;
            _texture = texture;
            _atlasPosition = atlasPosition;
            _position = position;
            _gridPosition = new Microsoft.Xna.Framework.Point((int)position.X / size.Width, (int)position.Y / size.Height);
            _tileCollision = tileCollision;
        }

        public Size Size { get => _size; }
        public Texture2D Texture { get => _texture; }
        public Rectangle? AtlasPosition { get => _atlasPosition; }
        public Vector2 Position { get => _position; }
        public Microsoft.Xna.Framework.Point GridPosition { get => _gridPosition; }

        public TileCollisionEnum TileCollision { get => _tileCollision; }

        public Vector2 CenterPosition{ get => new Vector2(_position.X + _size.Width / 2f, _position.Y + _size.Height / 2f); }
    }
}