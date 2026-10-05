using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using model.Interfaces.Service;
using model.Models.Game.Fov;
using model.Models.Game.Tilemap;
using model.Models.Primitives;
using service.Logic;

namespace service.Map
{
    public class MapGroupingService : IMapGroupingService
    {
        public IReadOnlyCollection<IGrouping<uint, GroupTile>> GroupLayerTilesIntoConvexShapes(IEnumerable<Tile> tiles)
        {
            var tilesToGroup = tiles.Select(x => new GroupTile()
            {
                Tile = x
            }).ToArray();
            uint groupIndex = 0;
            foreach(var tile in tilesToGroup)
            {
                if(tile.GroupIndex != null)
                {
                    continue;
                }
                var tilePoints = MyMathHelper.GetTilePoints(tile.Tile);
                CheckNeighbors(groupIndex, tile, tilesToGroup);
                groupIndex += 1;
            }

            return tilesToGroup.GroupBy(x => x.GroupIndex.GetValueOrDefault()).ToArray().AsReadOnly();
        }

        private bool CheckNeighbors(uint groupIndex, GroupTile tile, GroupTile[] tiles)
        {
            tile.GroupIndex = groupIndex;
            bool hasNeighbors = false;
            for (int i = -1; i < 2; i += 1)
            {
                for (int j = -1; j < 2; j += 1)
                {
                    if(i == 0 && j == 0)
                    {
                        continue;
                    }
                    var position = tile.GridPosition + new Point(i, j);
                    var tileChecked = tiles.Where(x => x.GridPosition == position).FirstOrDefault();
                    if(tileChecked == null || tileChecked.GroupIndex != null)
                    {
                        continue;
                    }
                    var checkedTilePoints = MyMathHelper.GetTilePoints(tileChecked.Tile);
                    List<Rib> checkedRibs = new List<Rib>();
                    Vector2[] initiatingTilePoints = MyMathHelper.GetTilePoints(tile.Tile);
                    Rib[] initiatingTileRibs = MyMathHelper.GetTileRibs(tile.Tile);
                    for (int tileAPointIndex = 0; tileAPointIndex < initiatingTilePoints.Length; tileAPointIndex += 1)
                    {
                        for (int tileBPointIndex = 0; tileBPointIndex < checkedTilePoints.Length; tileBPointIndex += 1)
                        {
                            checkedRibs.Add(new Rib(initiatingTilePoints[tileAPointIndex], checkedTilePoints[tileBPointIndex]));
                        }
                    }
                    
                    if(IsResultingFigureConcave(initiatingTileRibs, checkedRibs.ToArray()) == true)
                    {
                        continue;
                    }
                    CheckNeighbors(groupIndex, tileChecked, tiles);
                    hasNeighbors = true;
                }
            }

            return hasNeighbors;
        }

        private bool IsResultingFigureConcave(Rib[] initiatingTileRibs, Rib[] checkedRibs)
        {
            foreach(var checkedRib in checkedRibs)
            {
                foreach(var initiatingTileRib in initiatingTileRibs)
                {
                    if(MyMathHelper.CheckLinesIntersect(initiatingTileRib, checkedRib, out var collisionPoint) == false
                        && RibPointsSameAsCollision(initiatingTileRib, collisionPoint) == true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool RibPointsSameAsCollision(Rib rib, Vector2 collision)
        {
            return MyMathHelper.IsRibEndsAtPoint(
                collision.X, collision.Y,
                rib.X1, rib.Y1,
                rib.X2, rib.Y2
            );
        }
    }
}