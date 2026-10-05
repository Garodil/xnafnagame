using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Interfaces.Factory;
using model.Models.Entities;
using model.Models.Sound;
using model.Game;

namespace scenes.Factory
{
    public class PlayerFactory : IFactory<Entity>
    {
        string _iid;
        private Texture2D _texture;
        private Vector2 _size;
        private Vector2 _coords;
        private Rectangle? _rectangle;
        private Sound _sound;

        public PlayerFactory(string iid, Texture2D texture, Vector2 size, Vector2 coords, Sound sound, Rectangle? rectangle = null)
        {
            _iid = iid;
            _texture = texture;
            _size = size;
            _coords = coords;
            _rectangle = rectangle;
            _sound = sound;
        }

        public Entity Get(State state)
        {
            return new Player(_iid, _coords, 0.0f, _texture, _sound, _rectangle, state);
        }
    }
}