using System.Collections.Generic;
using ldtk;
using model.Interfaces.Factory;
using model.Models.Entities;
using model.Models.Game.Tilemap;
using model.Models.Game.View;

namespace model.Models.Game
{
    public class LevelScheme
    {
        private readonly Level _loadedLevel;
        private readonly int _levelIndex;
        private IReadOnlyCollection<TilemapLayer> _tilemapLayers;
        private IReadOnlyCollection<IFactory<Entity>> _factories;
        private IReadOnlyCollection<Camera> _cameras;
        private IReadOnlyCollection<Light.Light> _lights;
        private readonly string? _activeCameraIid;
        private readonly LevelBackground? _levelBackground;
        private readonly string _levelPath;
        
        public LevelScheme
        (
            Level level,
            int levelIndex,
            TilemapLayer[] tilemapLayers,
            IFactory<Entity>[] factories,
            Camera[] cameras,
            Light.Light[] lights,
            string? activeCameraIid,
            LevelBackground? levelBackround,
            string levelPath
        )
        {
            _loadedLevel = level;
            _levelIndex = levelIndex;
            _tilemapLayers = tilemapLayers;
            _factories = factories;
            _cameras = cameras;
            _lights = lights;
            _activeCameraIid = activeCameraIid;
            _levelBackground = levelBackround;
            _levelPath = levelPath;
        }

        public Level LevelJson { get => _loadedLevel; }
        public int Index { get => _levelIndex; }
        public IReadOnlyCollection<TilemapLayer> TilemapLayers { get => _tilemapLayers; }
        public IReadOnlyCollection<IFactory<Entity>> Factories { get => _factories; }
        public IReadOnlyCollection<Camera> Cameras { get => _cameras; }
        public string? ActiveCameraIid { get => _activeCameraIid; }
        public LevelBackground? LevelBackground { get => _levelBackground; }
        public string FullLevelPath { get => _levelPath; }
        public IReadOnlyCollection<Light.Light> Lights { get => _lights; }
    }
}
