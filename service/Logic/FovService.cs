using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using model.Interfaces.Service;
using model.Models.Game.Fov;
using model.Models.Game.Tilemap;
using model.Models.Primitives;
using Myra.Graphics2D;
using Myra.MML;

namespace service.Logic
{
    public class FovService : IFovService
    {
        public void CaclucateShadowGeometry(IEnumerable<Tile> tiles, Vector2 playerPosition)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tile> FindFovEffectiveTiles(IEnumerable<Tile> tiles, Vector2 playerPosition, float maxDistance)
        {
            var orderedTiles = OrderTilesByDistance(tiles, playerPosition)
                                .Where(x => x.LengthSquared < maxDistance * maxDistance)
                                .ToList();
            foreach(var testedTile in orderedTiles)
            {
                bool testedTileVisible = false;
                var coveringTiles = orderedTiles.Where(x => x.LengthSquared < testedTile.LengthSquared);
                var testedTilePoints = MyMathHelper.GetTilePoints(testedTile.Tile);
                Rib[] ribsToTestedTile = testedTilePoints.Select(x => new Rib(playerPosition, x)).ToArray();
                foreach(var ribToTile in ribsToTestedTile)
                {
                    var isRibVisible = true;
                    foreach(var coveringTile in coveringTiles)
                    {
                        Rib[] coveringTileRibs = MyMathHelper.GetTileRibs(coveringTile.Tile);
                        foreach(var coveringTileRib in coveringTileRibs)
                        {
                            if(
                                MyMathHelper.CheckLinesIntersect(ribToTile, coveringTileRib, out var collisionPoint) == true
                                && MyMathHelper.RibsHaveCommonEnds(ribToTile, coveringTileRib) == false
                            )
                            {
                                isRibVisible = false;
                                break;
                            }
                        }
                    }

                    if(isRibVisible == true)
                    {
                        testedTileVisible = true;
                        break;
                    }
                }
                testedTile.IsVisible = testedTileVisible;
            }

            return orderedTiles.Where(x => x.IsVisible == true).Select(x => x.Tile);
        }

        public (List<Rib>, List<Vector2>) GetVisibleRibs(ref FovTile[] tilesOrdered, Vector2 playerPosition, float maxDistance)
        {
            var orderedTiles = tilesOrdered
                                .Where(x => x.LengthSquared < maxDistance * maxDistance)
                                .ToList();
            List<Rib> visibleRibs = [];
            List<Vector2> collisionPoints = [];
            foreach(var testedTile in orderedTiles)
            {
                bool testedTileVisible = false;
                var coveringTiles = orderedTiles.Where(x => x.LengthSquared <= testedTile.LengthSquared);
                var testedTilePoints = MyMathHelper.GetTilePoints(testedTile.Tile);
                Rib[] ribsOfTestedTile = testedTilePoints.Select(x => new Rib(playerPosition, x)).ToArray();
                List<Rib> testedTileVisibleRibs = [];
                foreach(var ribToTile in ribsOfTestedTile)
                {
                    var isRibVisible = true;
                    foreach(var coveringTile in coveringTiles)
                    {
                        Rib[] coveringTileRibs = MyMathHelper.GetTileRibs(coveringTile.Tile);
                        foreach(var coveringTileRib in coveringTileRibs)
                        {
                            if(
                                MyMathHelper.CheckLinesIntersect(ribToTile, coveringTileRib, out var collisionPoint) == true
                                && MyMathHelper.RibsHaveCommonEnds(ribToTile, coveringTileRib) == false
                            )
                            {
                                isRibVisible = false;
                                break;
                            }
                        }
                    }

                    if(isRibVisible == true)
                    {
                        testedTileVisible = true;
                        visibleRibs.Add(ribToTile);
                        testedTileVisibleRibs.Add(ribToTile);
                        //break;
                    }
                }
                if(testedTileVisible && testedTileVisibleRibs.Count < 2)
                {
                    
                    var closestVectors = GetShadowBoundingVectors(testedTile.Tile, playerPosition);
                    testedTileVisibleRibs = closestVectors.Select(x => new Rib(playerPosition, x + playerPosition)).ToList();
                }
                testedTile.VisibleRibs = testedTileVisibleRibs;
                testedTile.IsVisible = testedTileVisible;
            }

            var ribs = DeduplicateRibs(visibleRibs);
            return (ribs, collisionPoints);
        }

        private static List<Rib> DeduplicateRibs(List<Rib> ribs)
        {
            List<Rib> deduplicated = new();
            foreach(var rib in ribs)
            {
                if(deduplicated.Any(deduplicatedRib => MyMathHelper.RibsEqual(rib, deduplicatedRib) == true))
                {
                    continue;
                }
                deduplicated.Add(rib);
            }
            return deduplicated;
        }


        public (List<Rib>, List<Vector2>) ExtendRibs(ref FovTile[] tilesOrdered, IEnumerable<Rib> ribs, Vector2 playerPosition, float maxDistance)
        {
            var orderedTiles = tilesOrdered
                                .Where(x => x.LengthSquared < maxDistance * maxDistance)
                                .ToList();

            List<Rib> extendedRibs = [];
            List<Vector2> collisionPoints = [];
            var otherTiles = orderedTiles.Where(x => x.IsVisible == true);
            foreach (var ribToTile in ribs)
            {
                var ribExtended = ExtendRib(ribToTile, maxDistance);
                bool firstIteration = true;
                //bool reextended = false;
                foreach (var testedTile in otherTiles)
                {
                    Rib[] testedTileRibs = MyMathHelper.GetTileRibs(testedTile.Tile);
                    
                    foreach (var testedTileRib in testedTileRibs)
                    {
                        if (MyMathHelper.CheckLinesIntersect(ribExtended, testedTileRib, out var collisionPoint) == true)
                        {
                            collisionPoints.Add(collisionPoint);
                            var oldRibLength = ribExtended.LengthSquared;
                            float
                                xCache = ribExtended.X1,
                                yCache = ribExtended.Y1;
                            ribExtended.X1 = collisionPoint.X;
                            ribExtended.Y1 = collisionPoint.Y;
                            var newRibLength = ribExtended.LengthSquared;
                            if (oldRibLength > newRibLength && firstIteration == false)
                            {
                                ribExtended.X1 = xCache;
                                ribExtended.Y1 = yCache;
                            }
                            firstIteration = false;
                            //reextended = true;
                        }
                    }
                }
                // if(reextended == false)
                // {
                //     Debug.WriteLine("Fuck me why?");
                // }
                extendedRibs.Add(ribExtended);
            }

            return (extendedRibs, collisionPoints);
        }

        public (List<Rib>, List<Vector2>) ExtendRibsForTile(ref FovTile[] tilesOrdered, Vector2 playerPosition, float maxDistance)
        {
            var orderedTiles = tilesOrdered
                                .Where(x => x.LengthSquared < maxDistance * maxDistance)
                                .ToList();

            List<Rib> extendedRibs = [];
            List<Vector2> collisionPoints = [];
            var otherTiles = orderedTiles.Where(x => x.IsVisible == true);
            foreach (var tile in otherTiles)
            {
                List<Rib> tileExtendedRibs = [];
                foreach (var ribToTile in tile.VisibleRibs)
                {
                    var ribExtended = ExtendRib(ribToTile, maxDistance);
                    bool firstIteration = true;
                    bool reextended = false;
                    foreach (var testedTile in otherTiles)
                    {
                        Rib[] testedTileRibs = MyMathHelper.GetTileRibs(testedTile.Tile);

                        foreach (var testedTileRib in testedTileRibs)
                        {
                            if (MyMathHelper.CheckLinesIntersect(ribExtended, testedTileRib, out var collisionPoint) == true)
                            {
                                collisionPoints.Add(collisionPoint);
                                var oldRibLength = ribExtended.LengthSquared;
                                float
                                    xCache = ribExtended.X1,
                                    yCache = ribExtended.Y1;
                                ribExtended.X1 = collisionPoint.X;
                                ribExtended.Y1 = collisionPoint.Y;
                                var newRibLength = ribExtended.LengthSquared;
                                if (oldRibLength > newRibLength && firstIteration == false)
                                {
                                    ribExtended.X1 = xCache;
                                    ribExtended.Y1 = yCache;
                                }
                                firstIteration = false;
                                reextended = true;
                            }
                        }
                    }
                    // if (reextended == false)
                    // {
                    //     Debug.WriteLine("Fuck me why?");
                    // }
                    extendedRibs.Add(ribExtended);
                    tileExtendedRibs.Add(ribExtended);
                }
                tile.VisibleRibs = tileExtendedRibs.ToList();
            }

            return (extendedRibs, collisionPoints);
        }

        private static Rib ExtendRib(Rib rib, float maxLength)
        {
            var vec = new Vector2(rib.X2 - rib.X1, rib.Y2 - rib.Y1);
            vec.Normalize();
            vec *= maxLength;
            return new Rib()
            {
                X1 = rib.X2,
                X2 = rib.X2 + vec.X,
                Y1 = rib.Y2,
                Y2 = rib.Y2 + vec.Y
            };
        }

        private static IOrderedEnumerable<FovTile> OrderTilesByDistance(IEnumerable<Tile> tiles, Vector2 playerPosition)
        {
            return tiles.Select(tile => new FovTile(
                tile, 
                (tile.Position - playerPosition).LengthSquared(),
                GetShadowBoundingVectors(tile, playerPosition)
            )).OrderBy(tile => tile.LengthSquared);
        }

        private static Vector2[] GetShadowBoundingVectors(Tile tile, Vector2 playerPosition)
        {
            var fromPlayerToTileVector = tile.CenterPosition - playerPosition;
            Vector2[] tilePoints = MyMathHelper.GetTilePoints(tile);

            Tuple<float, Vector2>[] dotsAndVectors = new Tuple<float, Vector2>[4];
            int i = 0;
            foreach(var point in tilePoints)
            {
                var pointVector = point - playerPosition;
                var cross = MyMathHelper.Cross(fromPlayerToTileVector, pointVector);
                dotsAndVectors[i] = new Tuple<float, Vector2>(Math.Abs(cross), pointVector);
                i += 1;
            }

            var boundingVectors = dotsAndVectors.OrderByDescending(x => x.Item1).Select(x => x.Item2).Take(2);
            return boundingVectors.ToArray();
        }

        private static Vector2[] GetShadowClosestVectors(Tile tile, Vector2 playerPosition)
        {
            var fromPlayerToTileVector = tile.CenterPosition - playerPosition;
            Vector2[] tilePoints = MyMathHelper.GetTilePoints(tile);

            Tuple<float, Vector2>[] dotsAndVectors = new Tuple<float, Vector2>[4];
            int i = 0;
            foreach(var point in tilePoints)
            {
                var pointVector = point - playerPosition;
                var cross = Vector2.Dot(fromPlayerToTileVector, pointVector);
                dotsAndVectors[i] = new Tuple<float, Vector2>(Math.Abs(cross), pointVector);
                i += 1;
            }

            var boundingVectors = dotsAndVectors.OrderBy(x => x.Item1).Select(x => x.Item2).Take(2);
            return boundingVectors.ToArray();
        }

        public bool PointInTriangle(Triangle tr, Vector2 point)
        {
            return MyMathHelper.CheckPointInTriangle(point, tr);
        }

        public bool LinesIntersect(Rib line1, Rib line2)
        {
            return MyMathHelper.CheckLinesIntersect(line1, line2, out var collision);
        }

        public Vector2[] GetTileBoundingVectors(Tile tile, Vector2 playerPosition)
        {
            return GetShadowClosestVectors(tile, playerPosition);
        }

        public IOrderedEnumerable<FovTile> OrderTilesByDistanceToPlayer(IEnumerable<Tile> tiles, Vector2 playerPosition)
        {
            return OrderTilesByDistance(tiles, playerPosition);
        }
    }
}