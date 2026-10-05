using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using model.Models.Game.Tilemap;

namespace model.Models.Game.Fov
{
    public class GroupTile
    {
        public Tile Tile {  set; get; }
        public uint? GroupIndex { set; get; }
        public Vector2 Position => Tile.Position;
        public Microsoft.Xna.Framework.Point GridPosition => Tile.GridPosition;
    }
}