using Box2D.NET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Game;
using model.Models.Enums;

namespace model.Models.Entities
{
    public class Item : Entity
    {
        private readonly Texture2D _sprite;
        private readonly Vector2 _elementSize = Vector2.Zero;
        private readonly B2ShapeId _shapeId;
        private readonly Rectangle? _rectangle;

        public Item(string iid, Vector2 coordinates, float rotation, Texture2D sprite, Vector2 elementSize, Rectangle? rectangle, State state)
        : base(iid, coordinates, rotation, state, BodyTypeEnum.DynamicBody)
        {
            _sprite = sprite;
            _elementSize = elementSize;
            _rectangle = rectangle;
            
            B2Polygon dynamicBox = B2Geometries.b2MakeBox(_elementSize.X / 2f, _elementSize.Y / 2f);
            B2ShapeDef shapeDef = B2Types.b2DefaultShapeDef();
            shapeDef.density = 0.1f;
            shapeDef.material.friction = 0.1f;
            _shapeId = B2Shapes.b2CreatePolygonShape(PhysixBodyId, ref shapeDef, ref dynamicBox);
        }

        public override Texture2D GetTexture()
        {
            return _sprite;
        }

        public override Vector2 GetTextureOrigin()
        {
            return new Vector2(_elementSize.X, _elementSize.Y) / 2.0f;
        }

        public override Vector2 GetTexturePosition()
        {
            return Position;
        }

        public override Rectangle? GetTextureRectangle()
        {
            return _rectangle;
        }

        public override float GetTextureRotation()
        {
            return Rotation;
        }

        public override Vector2 GetTextureScale()
        {
            if (_rectangle == null) { return Vector2.One; }
            
            var rectangle = _rectangle.Value;
            return new Vector2(_elementSize.X / rectangle.Width, _elementSize.Y / rectangle.Height);
        }

        public override void Update(GameTime gameTime)
        {
            var impulse = new B2Vec2(0,0);
            B2Bodies.b2Body_ApplyForceToCenter(PhysixBodyId, impulse, wake: true);
        }
    }
}