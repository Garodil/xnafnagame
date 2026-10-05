using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.NET;
using model.Models.Enums;
using model.Game;

namespace model.Models.Entities
{
    public abstract class Entity : Interfaces.Objects.IDrawable, IUpdateable
    {
        public string Iid { get; }
        public Vector2 Position {
            get 
            {
                var position = B2Bodies.b2Body_GetPosition(PhysixBodyId);
                return new Vector2(position.X, position.Y);
            }
        }
        public float Rotation {
            get
            {
                B2Rot rotation = B2Bodies.b2Body_GetRotation(PhysixBodyId);
                return B2MathFunction.b2Rot_GetAngle(rotation);
            }
        }
        private readonly B2BodyId _bodyId;

        protected Entity(string iid, Vector2 coordinates, float rotation, State state, BodyTypeEnum bodyType)
        {
            Iid = iid;

            var worldId = state.GetWorldId();
            var bodyDef = B2Types.b2DefaultBodyDef();
            bodyDef.type = (B2BodyType)bodyType;
            bodyDef.position = new B2Vec2(coordinates.X, coordinates.Y);
            bodyDef.motionLocks = new B2MotionLocks() 
            { 
                angularZ = true
            };
            bodyDef.rotation = B2MathFunction.b2MakeRot(rotation);
        
            _bodyId = B2Bodies.b2CreateBody(worldId, ref bodyDef);
        }

        ~Entity()
        {
            B2Bodies.b2DestroyBody(_bodyId);
        }

        public B2BodyId PhysixBodyId { get => _bodyId; }

        private int _drawOrder = 0;
        public int DrawOrder
        {
            set
            {
                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                return _drawOrder;
            }
        }

        private bool _visible = false;
        public bool Visible
        {
            set
            {
                _visible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                return _visible;
            }
        }
        public bool Enabled
        {
            set
            {
                if (value == true)
                {
                    B2Bodies.b2Body_Enable(_bodyId);
                } else
                {
                    B2Bodies.b2Body_Disable(_bodyId);
                }
                EnabledChanged?.Invoke(this, EventArgs.Empty);
                
            }
            get
            {
                return B2Bodies.b2Body_IsEnabled(_bodyId);
            }
        }

        private int _updateOrder = 0;
        public int UpdateOrder
        {
            set
            {
                _updateOrder = value;
                UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                return _updateOrder;
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public abstract void Update(GameTime gameTime);

        public abstract Texture2D GetTexture();
        public abstract Vector2 GetTexturePosition();
        public abstract float GetTextureRotation();
        public abstract Vector2 GetTextureScale();
        public abstract Vector2 GetTextureOrigin();
        public abstract Rectangle? GetTextureRectangle();
    }
}