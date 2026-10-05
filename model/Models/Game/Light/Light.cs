using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace model.Models.Game.Light
{
    public abstract class Light
    {
        protected Light(Vector2 position)
        {
            Position = position;
            Origin = Vector2.Zero;
        }

        public Vector2 Position { private set; get; }
        public abstract Texture2D GetLightTexture();
        public Vector2 Origin { protected set; get; }
        public Color ModulateColor { set; get; } = Color.White;
    }
}