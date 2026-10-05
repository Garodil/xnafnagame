using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using model.Interfaces.Objects;
using model.Models.Entities;
using model.Models.Game;
using model.Models.Game.Tilemap;
using model.Models.Sound;
using static Box2D.NET.B2Worlds;
using global::Box2D.NET;
using static Box2D.NET.B2Types;
using model.Models.Game.View;
using model.Interfaces.Manager;
using model.Models.Game.Light;
namespace model.Game
{
    public class State
    {
        private TilemapLayer[] _tilemapLayers = Array.Empty<TilemapLayer>();
        public IEnumerable<Interfaces.Objects.IDrawable> Drawables => Entities.Where(x => x is Interfaces.Objects.IDrawable).Where(x => x.Visible == true);
        public IEnumerable<IUpdateable> Updatebles => EnabledEntities.Where(x => x is IUpdateable);
        public IEnumerable<IAudibleEmitter> SoundEmitters => EnabledEntities.Where(x => x is IAudibleEmitter).Select(x => (IAudibleEmitter)x); //Changed

        private LevelBackground? _levelBackground;

        public Light[] Lights { get; private set; } = Array.Empty<Light>();
        public Entity[] Entities { get; private set; } = Array.Empty<Entity>();
        public IEnumerable<Entity> EnabledEntities => Entities.Where(x => x.Enabled == true);
        public IEnumerable<Entity> DisabledEntities => Entities.Where(x => x.Enabled == false);

        public Audible[] Ambient { get; private set; } = Array.Empty<Audible>();
        public TilemapLayer[] TilemapLayers { get => _tilemapLayers; }
        public TilemapLayer? DefaultTilemapLayer { get => _tilemapLayers.Where(x => x.IsVisible == true).OrderBy(x => x.Order).FirstOrDefault(); }
        public Player? Player => EnabledEntities.Where(x => x is Player).Select(x => x as Player).FirstOrDefault(); //Changed
        public LevelBackground? LevelBackground { get => _levelBackground; set => _levelBackground = value; }

        public ICameraManager CameraManager = new CameraManager();

        private readonly B2WorldDef _worldDef;
        private readonly B2WorldId _worldId;


        public State()
        {
            _worldDef = b2DefaultWorldDef();
            _worldDef.gravity = new B2Vec2(0.0f, 10.0f);
            _worldId = b2CreateWorld(ref _worldDef);
        }

        ~State()
        {
            foreach(var tilemapLayer in TilemapLayers)
            {
                tilemapLayer.Dispose();
            }
        }

        public B2WorldId GetWorldId()
        {
            return _worldId;
        }

        public void AddTilemapLayer(TilemapLayer layer)
        {
            _tilemapLayers = [.. _tilemapLayers, layer];
            AddTilemapCollisionsIntoTheWorld(layer);
        }
        public void AddTilemapLayer(IEnumerable<TilemapLayer> layers)
        {
            _tilemapLayers = [.. _tilemapLayers, .. layers];
            foreach(var layer in layers)
            {
                AddTilemapCollisionsIntoTheWorld(layer);
                break;
            }
        }

        private void AddTilemapCollisionsIntoTheWorld(TilemapLayer layer)
        {
            layer.InitCollisions(_worldId);
        }

        public B2BodyId AddEntity(Entity ent)
        {

            Entities = [.. Entities, ent];
            return ent.PhysixBodyId;
        }

        public B2BodyId[] AddEntity(IEnumerable<Entity> ent) //creating Entity returning BodyID
        {

            Entities = [.. Entities, .. ent];
            B2BodyId[] BodyIds = new B2BodyId[ent.Count()];
            short i = 0; //index for bodyIds array
            foreach (var body in ent)
            {
                BodyIds[i] = body.PhysixBodyId;
                ++i;
            }
            
            return BodyIds;
        }

        public void RemoveEntity(Entity ent)
        {
            Entities = Entities.Where(x => x != ent).ToArray();
        }

        public void RemoveEntity(IEnumerable<Entity> ent)
        {
            Entities = Entities.Where(x => ent.Contains(x) == false).ToArray();
        }

        public void AddLight(Light light)
        {
            Lights = [.. Lights, light];
        }

        public void AddLights(IEnumerable<Light> lights)
        {
            Lights = [.. Lights, .. lights];
        }

        public void RemoveLight(Light light)
        {
            Lights = Lights.Where(x => x != light).ToArray();
        }

        public void AddAmbient(Audible aud)
        {
            Ambient = [.. Ambient, aud];
        }

        public void AddAmbient(IEnumerable<Audible> aud)
        {
            Ambient = [.. Ambient, .. aud];
        }

        public void RemoveAmbient(Audible aud)
        {
            Ambient = Ambient.Where(x => x != aud).ToArray();
        }

        public void RemoveAmbient(IEnumerable<Audible> aud)
        {
            Ambient = Ambient.Where(x => aud.Any(y => y == x) == false).ToArray();
        }

        public void MakePhysicsStep(GameTime gt)
        {
            //var timeStep = 1 / 60f; // The amount of time to simulate, this should be a fixed number. Usually 1/60. 
            var subStepCount = 4;	//The number of sub-steps, increasing the sub-step count can increase accuracy. Usually 4. 
            B2Worlds.b2World_Step(_worldId, (float)gt.ElapsedGameTime.TotalSeconds , subStepCount);
        }
    }
}