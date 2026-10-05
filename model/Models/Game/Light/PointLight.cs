using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace model.Models.Game.Light
{
    public sealed class PointLight : Light
    {
        private readonly Texture2D _tex;

        public PointLight(GraphicsDevice gd, Vector2 position, int size, Color color) : base(position)
        {
            Size = size;
            _tex = new Texture2D(gd, size, size);
            int radius = size / 2;
            int radiusSquared = radius * radius;

            Color[] pixelArray = new Color[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var distanceToPixelSquared = Math.Pow(radius - i, 2) + Math.Pow(radius - j, 2);
                    pixelArray[i + j * size] = distanceToPixelSquared <= radiusSquared
                        ? color
                        : Color.Black;
                }
            }
            _tex.SetData(pixelArray);
            Origin = new Vector2(size / 2, size / 2 );
        }

        public int Size { get; private set; }

        public override Texture2D GetLightTexture()
        {
            return _tex;
        }
    }
}