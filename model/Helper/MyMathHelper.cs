using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using model.Models.Game.Fov;
using model.Models.Game.Tilemap;
using model.Models.Primitives;

namespace service.Logic
{
    public static class MyMathHelper
    {
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X*b.Y - a.Y*b.X; //ross
        }

        public static float Sign (Vector2 pt, Rib rb)
        {
            return (pt.X - rb.X2) * (rb.Y1 - rb.Y2) - (rb.X1 - rb.X2) * (pt.Y - rb.Y2); // cross
        }

        //WORKS
        public static bool CheckPointInTriangle(Vector2 point, Triangle triangle)
        {
            Rib[] triangleRibs = [
                new Rib() { X1 = triangle.X1, Y1 = triangle.Y1, X2 = triangle.X2, Y2 = triangle.Y2 },
                new Rib() { X1 = triangle.X2, Y1 = triangle.Y2, X2 = triangle.X3, Y2 = triangle.Y3 },
                new Rib() { X1 = triangle.X3, Y1 = triangle.Y3, X2 = triangle.X1, Y2 = triangle.Y1 }
            ];
            bool? isLastDotPositive = null;

            foreach(var rib in triangleRibs)
            {
                var sign = Sign(point, rib);                
                var isNewDotSignPositive = sign > 0;
                if(isLastDotPositive != null && isNewDotSignPositive != isLastDotPositive)
                {
                    return false;
                }
                
                isLastDotPositive = isNewDotSignPositive;
            }

            return true;
        }

        //WORKS
        public static bool CheckLinesIntersect(Rib line1, Rib line2, out Vector2 collisionPoint)
        {

            var cross1 = Cross(
                new Vector2(line2.X2 - line2.X1, line2.Y2 - line2.Y1), 
                new Vector2(line1.X1 - line2.X1, line1.Y1 - line2.Y1)
            );
            var cross2 = Cross(
                new Vector2(line1.X2 - line1.X1, line1.Y2 - line1.Y1), 
                new Vector2(line2.X2 - line2.X1, line2.Y2 - line2.Y1)
            );

            collisionPoint = Vector2.Zero;

            if (Math.Abs(cross2) < 1e-4) 
            {
                return false;
            }

            float r = cross1 / cross2;
            var cross3 = Cross(
                new Vector2(line1.X2 - line1.X1, line1.Y2 - line1.Y1), 
                new Vector2(line1.X1 - line2.X1, line1.Y1 - line2.Y1)
            );

            float s = cross3 / cross2;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            //collision point isn't accurate !!
            collisionPoint = new Vector2(
                line1.X1 + (int)(0.5f + r * (line1.X2 - line1.X1)),
                line1.Y1 + (int)(0.5f + r * (line1.Y2 - line1.Y1))
            );

            return true;  
        }

        public static bool IsRibEndsAtPoint(float pointX, float pointY, float ribX1, float ribY1, float ribX2, float ribY2)
        {
            return (FloatsEqual(pointX, ribX1, 1) && FloatsEqual(pointY, ribY1, 1)) //is point 1 same
                || (FloatsEqual(pointX, ribX2, 1) && FloatsEqual(pointY, ribY2, 1)); //is point 2 same
        }

        public static Quad GetTileQuad(Tile tile)
        {
            return new Quad {
                X1 = tile.Position.X,                   Y1 = tile.Position.Y,
                X2 = tile.Position.X + tile.Size.Width, Y2 = tile.Position.Y,
                X3 = tile.Position.X + tile.Size.Width, Y3 = tile.Position.Y + tile.Size.Height,
                X4 = tile.Position.X,                   Y4 = tile.Position.Y + tile.Size.Height,
            };
        }

        public static Vector2[] GetTilePoints(Tile tile)
        {
            return [ 
                tile.Position, 
                new Vector2(tile.Position.X + tile.Size.Width, tile.Position.Y), 
                new Vector2(tile.Position.X + tile.Size.Width, tile.Position.Y + tile.Size.Height), 
                new Vector2(tile.Position.X, tile.Position.Y + tile.Size.Height),
            ];
        }

        public static Rib[] GetTileRibs(Tile tile)
        {
            Vector2[] points = [
                tile.Position,
                new Vector2(tile.Position.X + tile.Size.Width, tile.Position.Y),
                new Vector2(tile.Position.X + tile.Size.Width, tile.Position.Y + tile.Size.Height),
                new Vector2(tile.Position.X, tile.Position.Y + tile.Size.Height),
            ];
            return [
                new Rib(points[0], points[1]),
                new Rib(points[1], points[2]),
                new Rib(points[2], points[3]),
                new Rib(points[3], points[0])
            ];
        }

        public static bool RibsEqual(Rib rib1, Rib rib2)
        {
            return FloatsEqual(rib1.X1, rib2.X1) && FloatsEqual(rib1.Y1, rib2.Y1)
                    && FloatsEqual(rib1.X2, rib2.X2) && FloatsEqual(rib1.Y2, rib2.Y2);
        }

        public static bool FloatsEqual(float a, float b)
        {
            float epsilon = 1e-3f;
            return Math.Abs(a - b) < epsilon;
        }

        public static bool FloatsEqual(float a, float b, float epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool VectorsEqual(Vector2 a, Vector2 b)
        {
            return FloatsEqual(a.X, b.X) && FloatsEqual(b.Y, b.Y);
        }

        public static bool RibsHaveCommonEnds(Rib a, Rib b)
        {
            return FloatsEqual(a.X1, b.X1) && FloatsEqual(a.Y1, b.X1) //a1 == b1
                    || FloatsEqual(a.X2, b.X2) && FloatsEqual(a.Y2, b.Y2) //a2 == b2
                    || FloatsEqual(a.X1, b.X2) && FloatsEqual(a.Y1, b.Y2) //a1 == b2
                    || FloatsEqual(a.X2, b.X1) && FloatsEqual(a.Y2, b.Y1); //a2 ==b1
        }

        public static double GetVectorAngle(Vector2 vector)
        {
            return Math.Atan2(vector.Y, vector.X);
        }
    }
}