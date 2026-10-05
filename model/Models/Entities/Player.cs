using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using model.Interfaces.Objects;
using model.Models.Sound;
using Microsoft.Xna.Framework;
using Box2D.NET;
using model.Game;


namespace model.Models.Entities
{
    public class Player : Entity, IControllable, IAudibleEmitter
    {
        private readonly Texture2D _sprite;
        private readonly Sound.Sound _sound;
        private readonly float _walkPower = 10000;
        private readonly float _jumpPower = 10000;
        private int _walkDirection = 0;
        private bool _isJumping = false;
        private readonly B2ShapeId _shapeId;
        private readonly Vector2 _elementSize = new Vector2(32, 32);
        private readonly Rectangle? _rectangle;

        public Player(string iid, Vector2 coordinates, float rotation, Texture2D sprite, Sound.Sound sound, Rectangle? rectangle, State state) 
            : base(iid, coordinates, rotation, state, Enums.BodyTypeEnum.DynamicBody)
        {
            _sprite = sprite;
            _sound = sound;

            B2Polygon dynamicBox = B2Geometries.b2MakeBox(_elementSize.X / 2f, _elementSize.Y / 2f);
            B2ShapeDef shapeDef = B2Types.b2DefaultShapeDef();
            shapeDef.density = 0.1f;
            shapeDef.material.friction = 0.1f;
            _shapeId = B2Shapes.b2CreatePolygonShape(PhysixBodyId, ref shapeDef, ref dynamicBox);
        }

        public void ApplyWalk(int value)
        {
            _walkDirection = value;
        }

        public void ApplyJump(int value)
        {
            _isJumping = value > 0;
        }

        public IEnumerable<Audible> GetSounds()
        {
            return [ _sound ];
        }

        public override Texture2D GetTexture()
        {
            return _sprite;
        }

        public override Vector2 GetTextureOrigin()
        {
            return new Vector2(_sprite.Width, _sprite.Height) / 2.0f;
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
            return new Vector2(_elementSize.X / _sprite.Width, _elementSize.Y / _sprite.Height);
        }

        public override void Update(GameTime gameTime)
        {
            var impulse = new B2Vec2(_walkDirection * _walkPower, _isJumping ? -1 * _jumpPower : 0);
            B2Bodies.b2Body_ApplyForceToCenter(PhysixBodyId, impulse, wake: true);
            //B2Bodies.b2Body_SetLinearVelocity(PhysixBodyId, impulse); //for kinematic
        }
    }
}