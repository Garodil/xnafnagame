using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Game.Tilemap;

namespace model.Models.Game.Fov
{
    public class TilemapGroup
    {
        public TilemapGroup(uint index, Tile[] tiles)
        {
            GroupIndex = index;
            Tiles = tiles;
        }

        public uint GroupIndex { private set; get; }
        public Tile[] Tiles { private set; get; }
    }
}