using System;
using System.Collections;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using model.Models.Game.Fov;
using model.Interfaces.Service;
using Box2D.NET;
using Microsoft.VisualBasic;
using ldtk;
using model.Models.Enums;
using model.Models.Map;

namespace model.Models.Game.Tilemap
{
    public class TilemapLayer : Layer
    {
        private readonly Size _tilemapSize;
        private readonly Tile[] _tiles = [];
        private readonly TilemapGroup[] _tileGroups = [];
        private readonly Size _tileSize;
        private Size _tilemapSizePixels;

        public int Order { set; get; }
        public Size TilemapSize { get => _tilemapSize; }
        public Size TileSize { get => _tileSize; }
        public Size TilemapSizePixels { get => _tilemapSizePixels; }

        public Vector2 ParallaxFactor { get; set; }
        public bool IsParallaxScaled { get; set; }
        public bool IsVisible { set; get; }

        public bool IsCollidable { set; get; }
        public B2BodyId[] CollisionIds { private set; get; } = [];
        

        public TilemapLayer (Size tilemapSize, Size tileSize, Tile[] tiles, IMapGroupingService mapGroupingService, bool createFovGrouping, bool isVisible = true, bool isCollidable = false)
        {
            if(tilemapSize.Width <= 0 || tilemapSize.Height <= 0)
            {
                throw new ArgumentException("Tilemap size cannot be empty or negative");
            }
            if(tiles.Length > tilemapSize.Width * tilemapSize.Height)
            {
                throw new ArgumentException("Tilemap size is greater than tilemap size");
            }
            if (createFovGrouping == true)
            {
                var tileGroups = mapGroupingService.GroupLayerTilesIntoConvexShapes(tiles);
                _tileGroups = tileGroups.Select(x => new TilemapGroup(x.Key, x.Select(y => y.Tile).ToArray())).ToArray();
            }

            _tilemapSize = tilemapSize;
            _tiles = tiles;
            _tileSize = tileSize;
            _tilemapSizePixels = new(_tilemapSize.Width * _tileSize.Width, _tilemapSize.Height * _tileSize.Height); 
            IsVisible = isVisible;
            IsCollidable = isCollidable;
            IsParallaxScaled = false;
            ParallaxFactor = Vector2.One;
        }

        public IEnumerable<Tile> GetTiles()
        {
            return _tiles;
        }

        public IEnumerable<TilemapGroup> GetTileGroups()
        {
            return _tileGroups;
        }

        public IEnumerable<Tile> GetTilesByGridPosition(Point pt)
        {
            return _tiles.Where(tl => tl.GridPosition.Equals(pt));
        }

        public void InitCollisions(B2WorldId worldId)
        {
            if (IsCollidable == false)
            {
                return;
            }

            var bodyIds = new List<B2BodyId>();
            foreach (var tile in GetTiles())
            {
                B2BodyDef bodyDef = B2Types.b2DefaultBodyDef();
                bodyDef.type = (B2BodyType)BodyTypeEnum.StaticBody;
                bodyDef.position = new B2Vec2(tile.CenterPosition.X, tile.CenterPosition.Y);
                var (sin, cos) = Math.SinCos(0);
                bodyDef.rotation = new B2Rot((float)sin, (float)cos);
                var bodyId = B2Bodies.b2CreateBody(worldId, ref bodyDef);

                B2ShapeDef shapeDef = B2Types.b2DefaultShapeDef();
                shapeDef.density = 1.0f;
                shapeDef.material.friction = 0.3f;

                B2Polygon tileBox = B2Geometries.b2MakeBox(tile.Size.Width / 2, tile.Size.Height / 2);
                B2Shapes.b2CreatePolygonShape(bodyId, ref shapeDef, ref tileBox);
                bodyIds.Add(bodyId);
            }
            CollisionIds = [.. bodyIds];
        }
        
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                RemoveAllCollisions();
                _disposedValue = true;
            }
        }

        void RemoveAllCollisions()
        {
            if(CollisionIds == null || CollisionIds.Length == 0)
            {
                return;
            }
            var removed = new List<B2BodyId>();
            foreach(var collisionId in CollisionIds)
            {
                B2Bodies.b2DestroyBody(collisionId);
                removed.Add(collisionId);
            }
            CollisionIds = [.. CollisionIds.Where(x => removed.Contains(x) == false)];
        }
    }
}