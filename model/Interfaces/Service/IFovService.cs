using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using model.Models.Game.Fov;
using model.Models.Game.Tilemap;
using model.Models.Primitives;
using Myra.MML;
using Newtonsoft.Json.Linq;

namespace model.Interfaces.Service
{
    public interface IFovService
    {
        IEnumerable<Tile> FindFovEffectiveTiles(IEnumerable<Tile> orderedTiles, Vector2 playerPosition, float maxDistance);

        Vector2[] GetTileBoundingVectors(Tile tile, Vector2 playerPosition);

        void CaclucateShadowGeometry(IEnumerable<Tile> tiles, Vector2 playerPosition);

        IOrderedEnumerable<FovTile> OrderTilesByDistanceToPlayer(IEnumerable<Tile> tiles, Vector2 playerPosition);

        bool PointInTriangle(Triangle tr, Vector2 point);

        bool LinesIntersect(Rib line1, Rib line2);

        (List<Rib>, List<Vector2>) GetVisibleRibs(ref FovTile[] tilesOrdered, Vector2 playerPosition, float maxDistance);

        (List<Rib>, List<Vector2>) ExtendRibs(ref FovTile[] tilesOrdered, IEnumerable<Rib> ribs, Vector2 playerPosition, float maxDistance);

        (List<Rib>, List<Vector2>) ExtendRibsForTile(ref FovTile[] tilesOrdered, Vector2 playerPosition, float maxDistance);
    }
}