using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace model.Models.Primitives
{
    public struct Rib
    {
        public float X1;
        public float X2;
        public float Y1;
        public float Y2; 

        public Rib()
        {

        }

        public Rib(Vector2 from, Vector2 to)
        {
            X1 = from.X; Y1 = from.Y;
            X2 = to.X; Y2 = to.Y;
        }

        public float LengthSquared
        {
            get => (X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2);
        }

        public string ToString()
        {
            return $"[({X1} {Y1}),({X2} {Y2})]";
        }

        public class AngleComparer : IComparer<Rib>
        {
            public int Compare(Rib rib1, Rib rib2)
            {
                var rib1angle = (Math.Atan2(rib1.Y2 - rib1.Y1, rib1.X2 - rib1.X1) + MathHelper.TwoPi) % MathHelper.TwoPi;
                var rib2angle = (Math.Atan2(rib2.Y2 - rib2.Y1, rib2.X2 - rib2.X1) + MathHelper.TwoPi) % MathHelper.TwoPi;
                if(rib1angle > MathHelper.PiOver4 * 3 && rib2angle < MathHelper.PiOver4)
                {
                    rib2angle += MathHelper.TwoPi;
                }
                if(rib2angle > MathHelper.PiOver4 * 3 && rib1angle < MathHelper.PiOver4)
                {
                    rib1angle += MathHelper.TwoPi;
                }
                if(Math.Abs(rib1angle - rib2angle) < 1e-4)
                {
                    return 0;
                } 
                else if(rib1angle < rib2angle)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }

    
}