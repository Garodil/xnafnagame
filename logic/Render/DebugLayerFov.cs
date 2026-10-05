using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Game;
using model.Interfaces.Service;
using model.Models.Game.Tilemap;
using model.Models.Primitives;
using model.Models.Render.Debug;
using Newtonsoft.Json;
using scenes.Scenes;

namespace logic.Render
{
    public class DebugLayerFov : DebugLayer
    {
        private readonly IFovService _fovService;
        private float _maxDistance = 1000;

        private int _maxTileAmount = 1000;

        private readonly Color
            _visibleTileColor,
            _tileVectorColor,
            _visibleRibColor,
            _extendedRibColor,
            _segmentColor,
            _collisionPointColor;


        private Rectangle[] _visibleTiles = [];
        private Rib[] _visibleRibs = [];
        private Rib[] _extendedRibs = [];
        private Rib[] _raysToQuads = [];
        private Quad[] _fovQuads = [];
        private Vector2[] _fovCollisionPoints = [];

        public DebugLayerFov(
            string version,
            SpriteBatch worldBatch,
            SpriteBatch overlayBatch,
            SpriteFont debugFont,
            Texture2D debugTex,
            IFovService fovService,
            Color visibleTileColor,
            Color tileVectorColor,
            Color visibleRibColor,
            Color fovSegmentColor,
            Color fovCollisionPointColor,
            Color fovExtendedRibColor
        ) 
            : base(version, worldBatch, overlayBatch, debugFont, debugTex)
        {
            _fovService = fovService;
            _tileVectorColor = tileVectorColor;
            _visibleRibColor = visibleRibColor;
            _visibleTileColor = visibleTileColor;
            _segmentColor = fovSegmentColor;
            _collisionPointColor = fovCollisionPointColor;
            _extendedRibColor = fovExtendedRibColor;
        }

        public override void CalculateDataLayer(GameTime gameTime, State currentState)
        {
            var tilemap = currentState.DefaultTilemapLayer;
            var player = currentState.Player;
            if(tilemap == null || player == null)
            {
                return;
            }
            CalculateVisibleTiles(tilemap.GetTiles(), player.Position);
            CalculateRaysToVisibleQuads(tilemap.GetTiles(), player.Position);
            CalculateVisibleSegments(tilemap.GetTiles(), player.Position);
        }

        public override void CalculateRenderDataLayer(GameTime gameTime, State currentState)
        {
            
        }

        public override void DrawInWorldLayer()
        {
            DrawVisibleTiles();
            DrawFovQuads();
            DrawRaysToVisibleQuads();
            DrawExtendedRibs();
            DrawVisibleRibs();
            DrawCollisionPoints();
        }

        public override void DrawOverlayLayer()
        {
            string[] lines =
            [
                $"Build: {_version}",
                $"Quads: { _fovQuads.Length }",
                $"Quad points: { _fovCollisionPoints.Length }"
            ];
            var displayCoordinates = Vector2.Zero;
            foreach (var line in lines)
            {
                DrawOverlayText(displayCoordinates, line);
                displayCoordinates = new Vector2(displayCoordinates.X, displayCoordinates.Y + _debugFont.LineSpacing);
            }
        }

        private void DrawOverlayText(Vector2 place, string text)
        {
            _overlayBatch.DrawString(_debugFont, text, place, Color.White);
        }

        private void DrawVisibleTiles()
        {
            foreach(var tile in _visibleTiles)
            {
                RenderHelper.DrawRectangle(_worldBatch, _debugTex, new Vector2(tile.X, tile.Y), tile.Width, tile.Height, _visibleTileColor, false);
            }
        }

        private void CalculateVisibleTiles(IEnumerable<Tile> tiles, Vector2 playerPosition)
        {
            var fovTiles = _fovService.FindFovEffectiveTiles(
                tiles,
                playerPosition,
                _maxDistance
            );
            _visibleTiles = fovTiles.Select(x => new Rectangle()
            {
                X = (int)x.Position.X, //todo: maybe float?
                Y = (int)x.Position.Y,
                Height = x.Size.Height,
                Width = x.Size.Width
            }).ToArray();
        }

        private void DrawRaysToVisibleQuads()
        {
            foreach(var rib in _raysToQuads)
            {
                RenderHelper.DrawLine(_worldBatch, _debugTex, new Vector2(rib.X1, rib.Y1), new Vector2(rib.X2, rib.Y2), _tileVectorColor, 1);
            }
        }

        private void CalculateRaysToVisibleQuads(IEnumerable<Tile> tiles, Vector2 playerPosition)
        {
            var tilesOrdered = _fovService.OrderTilesByDistanceToPlayer(tiles, playerPosition).ToArray();
            List<Rib> ribs = [];
            foreach(var orderedTile in tilesOrdered.Take(_maxTileAmount))
            {
                var nearestTile = orderedTile.Tile;
                var vectors = _fovService.GetTileBoundingVectors(nearestTile, playerPosition);
                foreach(var boundingVector in vectors)
                {
                    var resultingPoint = boundingVector + playerPosition;
                    var rib = new Rib()
                    {
                        X1 = playerPosition.X,
                        Y1 = playerPosition.Y,
                        X2 = resultingPoint.X,
                        Y2 = resultingPoint.Y
                    };
                    ribs.Add(rib);
                }
            }
            _raysToQuads = ribs.ToArray();
        }

        private void DrawCollisionPoints()
        {
            foreach(var collsionPoint in _fovCollisionPoints)                                  
            {
                RenderHelper.DrawPoint(_worldBatch, _debugTex, collsionPoint, 2, _collisionPointColor);
            } 
        }

        private void DrawFovQuads()
        {
            foreach(var quad in _fovQuads)
            {
                RenderHelper.DrawScuffedQuad(_worldBatch, _debugTex, quad, _segmentColor);
            }
        }

        private void DrawVisibleRibs()
        {
            foreach(var rib in _visibleRibs)
            {
                RenderHelper.DrawLine(_worldBatch, _debugTex, new Vector2(rib.X1, rib.Y1), new Vector2(rib.X2, rib.Y2), _visibleRibColor, 1);
            }
        }

        private void DrawExtendedRibs()
        {
            foreach(var rib in _extendedRibs)
            {
                RenderHelper.DrawLine(_worldBatch, _debugTex, new Vector2(rib.X1, rib.Y1), new Vector2(rib.X2, rib.Y2), _extendedRibColor, 1);
            }
        }


        private void CalculateVisibleSegments(IEnumerable<Tile> tiles, Vector2 playerPosition)
        {
            List<Quad> quads = new List<Quad>();
            List<Rib> visibleRibsCollection = new List<Rib>();

            var orderedTiles = _fovService.OrderTilesByDistanceToPlayer(tiles, playerPosition).ToArray();
            var visibleRibs = _fovService.GetVisibleRibs(ref orderedTiles, playerPosition, _maxDistance);
            var extendedRibs = _fovService.ExtendRibs(ref orderedTiles, visibleRibs.Item1, playerPosition, _maxDistance);
            _fovService.ExtendRibsForTile(ref orderedTiles, playerPosition, _maxDistance);
            foreach (var tile in orderedTiles.Where(x => x.IsVisible == true))
            {
                tile.VisibleRibs.Sort(new Rib.AngleComparer());
                for (int i = 0; i < tile.VisibleRibs.Count - 1; i += 1)
                {
                    Rib firstRib = tile.VisibleRibs[i];
                    Rib lastRib = tile.VisibleRibs[i + 1];
                    var quad = new Quad()
                    {
                        X1 = firstRib.X1,
                        Y1 = firstRib.Y1,
                        X2 = firstRib.X2,
                        Y2 = firstRib.Y2,
                        X3 = lastRib.X2,
                        Y3 = lastRib.Y2,
                        X4 = lastRib.X1,
                        Y4 = lastRib.Y1
                    };
                    quads.Add(quad);

                }
                visibleRibsCollection.AddRange(tile.VisibleRibs);
            }
            _visibleRibs = visibleRibsCollection.ToArray();
            _fovQuads = quads.ToArray();
            _fovCollisionPoints = extendedRibs.Item2.ToArray();
            _extendedRibs = extendedRibs.Item1.ToArray();
        }
    }
}