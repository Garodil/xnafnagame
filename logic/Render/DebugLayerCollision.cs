using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box2D.NET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Game;
using model.Interfaces;
using model.Interfaces.Service;
using scenes.Manager;

namespace model.Models.Render.Debug
{
    public class DebugLayerCollision : DebugLayer
    {
        private readonly Color 
            _entitiesColor, 
            _contactPointColor, 
            _blocksColor;

        private B2BodyId[] _collisionEntitiesIds = Array.Empty<B2BodyId>();
        private B2BodyId[] _collisionTilesIds = Array.Empty<B2BodyId>();

        public DebugLayerCollision(
            string version,
            Color entitiesColor,
            Color contactPointColor,
            Color blocksColor,
            SpriteBatch worldBatch, 
            SpriteBatch overlayBatch, 
            SpriteFont debugFont,
            Texture2D debugTexture
            ) 
            : base(version, worldBatch, overlayBatch, debugFont, debugTexture)
        {
            _entitiesColor = entitiesColor;
            _contactPointColor = contactPointColor;
            _blocksColor = blocksColor;
        }

        public override void CalculateDataLayer(GameTime gameTime, State currentState)
        {
            _collisionEntitiesIds = currentState.EnabledEntities.Select(x => x.PhysixBodyId).ToArray();
            _collisionTilesIds = currentState.TilemapLayers.Where(x => x.CollisionIds.Length > 0).SelectMany(x => x.CollisionIds).ToArray();
        }

        public override void DrawInWorldLayer()
        {
            DrawCollisionDebug();
        }

        public override void DrawOverlayLayer()
        {
            string[] lines =
            [
                $"Build: {_version}"
            ];
            var displayCoordinates = Vector2.Zero;
            foreach(var line in lines)
            {
                DrawOverlayText(displayCoordinates, line);
                displayCoordinates = new Vector2(displayCoordinates.X, displayCoordinates.Y + _debugFont.LineSpacing);
            }
        }

        private void DrawOverlayText(Vector2 place, string text)
        {
            _overlayBatch.DrawString(_debugFont, text, place, Color.White);
        }

        private void DrawCollisionDebug()
        {
            Span<B2ShapeId> shapeArray = new Span<B2ShapeId>(new B2ShapeId[10]);
            Span<B2ContactData> contactArray = new Span<B2ContactData>(new B2ContactData[50]);
            if (_collisionEntitiesIds != null && _collisionEntitiesIds.Length != 0)
            {
                foreach (var bodyId in _collisionEntitiesIds)
                {
                    var shapesReturnedAmount = B2Bodies.b2Body_GetShapes(bodyId, shapeArray, shapeArray.Length);
                    DrawAABBs(shapeArray, shapesReturnedAmount, _entitiesColor);
                    var returnedContactDataAmount = B2Bodies.b2Body_GetContactData(bodyId, contactArray, contactArray.Length);
                    DrawContactPointss(contactArray, returnedContactDataAmount, _contactPointColor);
                }
            }

            if (_collisionTilesIds != null && _collisionTilesIds.Length != 0)
            {
                foreach (var collisionId in _collisionTilesIds)
                {
                    var shapesReturnedAmount = B2Bodies.b2Body_GetShapes(collisionId, shapeArray, shapeArray.Length);
                    DrawAABBs(shapeArray, shapesReturnedAmount, _blocksColor);
                }
            }
        }

        private void DrawAABBs(Span<B2ShapeId> shapeArray, int totalShapeAmount, Color debugColor)
        {
            for (int i = 0; i < totalShapeAmount; ++i)
            {
                var shape = shapeArray[i];
                B2AABB aabb = B2Shapes.b2Shape_GetAABB(shape);
                _worldBatch.Draw(
                    _debugTex,
                    new Rectangle(
                        (int)aabb.lowerBound.X,
                        (int)aabb.lowerBound.Y,
                        (int)(aabb.upperBound.X - aabb.lowerBound.X),
                        (int)(aabb.upperBound.Y - aabb.lowerBound.Y)
                    ),
                    debugColor
                );
            }
        }

        private void DrawContactPointss(Span<B2ContactData> contactArray, int totalContactAmount, Color debugColor)
        {
            for (int i = 0; i < totalContactAmount; ++i)
            {
                var contact = contactArray[i];
                for (int j = 0; j < contact.manifold.pointCount; ++j)
                {
                    var point = contact.manifold.points[j];
                    
                    _worldBatch.Draw(
                        _debugTex,
                        new Rectangle(
                            (int)point.point.X - 1,
                            (int)point.point.Y - 1,
                            2,
                            2
                        ),
                        debugColor
                    );
                }
            }
        }

        public override void CalculateRenderDataLayer(GameTime gameTime, State currentState)
        {
        }
    }
}