using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Models.Structs
{
    public struct Size
    {
        public int Height{get;set;}
        public int Width{get;set;}
        public Size(int h, int w)
        {
            Height = h;
            Width = w;
        }
        public Size(){}
        public bool Equals(Size other)
        {
            return this.Width == other.Width && this.Height == other.Height;
        }

    }
}