using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using model.Models.Game.Tilemap;
using model.Models.Primitives;

namespace model.Models.Game.Fov
{
    public class FovTile
    {
        public FovTile(Tile tile, float lengthSquared, Vector2[] boundingVectors)
        {
            Tile = tile;
            LengthSquared = lengthSquared;
            BoundingVectors = boundingVectors;
        }

        public Tile Tile { private set; get; }

        public Vector2[] BoundingVectors { set; get; }

        public List<Rib> VisibleRibs { set; get; } = [];

        public bool? IsVisible { set; get; }

        public float LengthSquared { private set; get; }
    }
}