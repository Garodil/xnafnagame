using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Models.Primitives;

namespace logic.Render
{
    public static class RenderHelper
    {
        public static void DrawFilledTriangle(SpriteBatch batch, Texture2D texture, Triangle tr, Microsoft.Xna.Framework.Color cl)
        {
            // var indices = 
            // batch.GraphicsDevice.Indices =
            // batch.GraphicsDevice.SetVertexBuffer();
            // batch.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList);
            // batch.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>
            // (
            //     PrimitiveType.TriangleStrip,
            //     //"array of 3 VertexPositionColor?",
            //     0,
            //     (3? 9?),
            //     //"I have no clue what to put here and why I should put it here?",
            //     //"What do I put here?",
            //     //"If 1 triangle, value should be = 1? 1 Triangle = 1 Primitive"
            // );
        }

        public static void DrawScuffedQuad(SpriteBatch spriteBatch, Texture2D texture, Quad quad, Microsoft.Xna.Framework.Color cl)
        {
            Vector3[] vectors = 
            [
                new Vector3(quad.X1, quad.Y1, 0f),
                new Vector3(quad.X2, quad.Y2, 0f),
                new Vector3(quad.X3, quad.Y3, 0f),
                new Vector3(quad.X4, quad.Y4, 0f),
            ];
            VertexPositionColorTexture[] vertices = 
            [
                new VertexPositionColorTexture() 
                {
                    TextureCoordinate = new Vector2(0, 0),
                    Position = vectors[0],
                    Color = cl
                },
                new VertexPositionColorTexture() 
                {
                    TextureCoordinate = new Vector2(0, 0),
                    Position = vectors[1],
                    Color = cl
                },
                new VertexPositionColorTexture() 
                {
                    TextureCoordinate = new Vector2(0, 0),
                    Position = vectors[2],
                    Color = cl
                },
                new VertexPositionColorTexture() 
                {
                    TextureCoordinate = new Vector2(0, 0),
                    Position = vectors[3],
                    Color = cl
                }
            ];
            short[] vertexIndices = [ 0, 1, 2, 0, 2, 3 ];
            spriteBatch.GraphicsDevice.Textures[0] = texture;

            spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                vertices,
                0,
                4,
                vertexIndices,
                0,
                2
            );
        }

        public static void DrawTriangle(SpriteBatch batch, Texture2D texture, Triangle tr, Microsoft.Xna.Framework.Color cl)
        {
            DrawLine(batch, texture, new Vector2(tr.X1, tr.Y1), new Vector2(tr.X2, tr.Y2), cl);
            DrawLine(batch, texture, new Vector2(tr.X2, tr.Y2), new Vector2(tr.X3, tr.Y3), cl);
            DrawLine(batch, texture, new Vector2(tr.X3, tr.Y3), new Vector2(tr.X1, tr.Y1), cl);
        }

        public static void DrawPoint(SpriteBatch batch, Texture2D texture, Vector2 point, int size, Microsoft.Xna.Framework.Color cl)
        {
            DrawRectangle(batch, texture, point, size, size, cl, true);
        }

        public static void DrawLine(SpriteBatch batch, Texture2D texture, Vector2 from, Vector2 to, Microsoft.Xna.Framework.Color cl, int thiccness = 1)
        {
            var vec1 = new Vector2(to.X - from.X, to.Y - from.Y);
            var rotation1 = Math.Atan2(vec1.Y, vec1.X);
            batch.Draw(
                texture, 
                new Microsoft.Xna.Framework.Rectangle((int)from.X, (int)from.Y, (int)vec1.Length(), thiccness),
                null,
                cl, 
                (float)rotation1, 
                new Vector2 (0f, 0f), 
                SpriteEffects.None, 1f
            );
        }

        public static void DrawRectangle(SpriteBatch batch, Texture2D texture, Vector2 position, float width, float height, Microsoft.Xna.Framework.Color cl, bool centered)
        {
            batch.Draw(
                    texture,
                    new Microsoft.Xna.Framework.Rectangle(
                        centered ? (int)(position.X - width / 2f) : (int)(position.X), 
                        centered ? (int)(position.Y - height / 2f) : (int)(position.Y), 
                        (int)width, 
                        (int)height
                    ),
                    cl
                );
        }

        public static void DrawHollowRectangle(SpriteBatch batch, Texture2D texture, Vector2 position, float width, float height, Microsoft.Xna.Framework.Color cl, int thiccness, bool centered)
        {
            Vector2 fullsize = new Vector2(width, height);
            Vector2 halfSize = fullsize / 2; 
            Vector2 point1 = centered ? position -= halfSize : position, 
                    point2 = point1 + fullsize;

            if(centered == true)
            {

                position.X -= width / 2f;
                position.Y = position.Y - height / 2f;
            }
            DrawLine(batch, texture, point1, new Vector2(point2.X, point1.Y), cl, thiccness);
            DrawLine(batch, texture, new Vector2(point2.X, point1.Y), point2, cl, thiccness);
            DrawLine(batch, texture, point2, new Vector2(point1.X, point2.Y), cl, thiccness);
            DrawLine(batch, texture, new Vector2(point1.X, point2.Y), point1, cl, thiccness);
        }
    }
}