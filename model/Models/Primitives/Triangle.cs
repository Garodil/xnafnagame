using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Models.Primitives
{
    public struct Triangle
    {
        public float X1, Y1;
        public float X2, Y2;
        public float X3, Y3;

        public double Area()
        {
            return Math.Abs((X1 * (Y2 - Y3) +
                         X2 * (Y3 - Y1) + 
                         X3 * (Y1 - Y2)) / 2.0);
        }
    }
}