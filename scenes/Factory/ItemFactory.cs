using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Game;
using model.Interfaces.Factory;
using model.Models.Entities;

namespace scenes.Factory
{
    public class ItemFactory : IFactory<Entity>
    {
        private string _iid;
        private Texture2D _texture;
        private Vector2 _size;
        private Vector2 _coords;
        private Rectangle? _rectangle;

        public ItemFactory(string iid, Texture2D texture, Vector2 size, Vector2 coords, Rectangle? rectangle = null)
        {
            _iid = iid;
            _texture = texture;
            _size = size;
            _coords = coords;
            _rectangle = rectangle;
        }
        public Entity Get(State state)
        {
            return new Item(_iid, _coords, 0.0f, _texture, _size, _rectangle, state);
        }
    }
}