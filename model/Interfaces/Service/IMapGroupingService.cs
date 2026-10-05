using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Game.Fov;
using model.Models.Game.Tilemap;

namespace model.Interfaces.Service
{
    public interface IMapGroupingService
    {
        IReadOnlyCollection<IGrouping<uint, GroupTile>> GroupLayerTilesIntoConvexShapes(IEnumerable<Tile> tilesToGroup);
    }
}