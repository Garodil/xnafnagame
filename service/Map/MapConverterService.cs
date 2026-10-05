using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using ldtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using model.Exceptions;
using model.Interfaces.Factory;
using model.Interfaces.Repository;
using model.Interfaces.Service;
using model.Models.Entities;
using model.Models.Game;
using model.Models.Game.Light;
using model.Models.Game.Tilemap;
using model.Models.Game.View;
using model.Models.Map;
using scenes.Factory;

namespace service.Map
{
    public class MapConverterService : IMapConverterService
    {
        private readonly IMapImporter _mapImporter;
        private readonly IMapGroupingService _mapGrouper;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _mapBasePath;

        public MapConverterService(string mapBasePath, IServiceProvider serviceProvider)
        {
            if(string.IsNullOrWhiteSpace(mapBasePath))
            {
                throw new ArgumentException("Maps base path cannot be null");
            }
            _mapBasePath = mapBasePath;
            _serviceProvider = serviceProvider;
            _mapImporter = serviceProvider.GetService(typeof(IMapImporter)) as IMapImporter ?? throw new Exception("No map importer found");
            _mapGrouper = serviceProvider.GetService(typeof(IMapGroupingService)) as IMapGroupingService ?? throw new Exception("No map grouper found");
        }

        public int GetLevels(string worldRelPath)
        {
            var ldtkWorld = _mapImporter.LoadWorld(GetLevelPath(worldRelPath));
            return ldtkWorld.Levels.Length;
        }

        public LevelScheme GetLevelScheme(string worldRelPath, int levelIndex, ContentManager contentManager)
        {
            var ldtkWorld = _mapImporter.LoadWorld(GetLevelPath(worldRelPath));
            var selectedLevel = ldtkWorld.Levels[levelIndex];
            var textureAtlusses = ExtractWorldTilesetAtlussesWithLayersIds(ldtkWorld, worldRelPath, contentManager);
            var requiredTags = ExtractWorldRequiredTagsWithLayersIds(ldtkWorld);
            var layers = ConvertLevel(selectedLevel, textureAtlusses, requiredTags, ldtkWorld.Defs);
            var entitiesLayers = layers.OfType<EntitiesLayer>().ToArray();
            int lastCameraIndex = 0;

            return new LevelScheme
            (
                selectedLevel,
                levelIndex,
                layers.OfType<TilemapLayer>().ToArray(),
                ConvertEntities(entitiesLayers, textureAtlusses, contentManager),
                ConvertCameras(entitiesLayers, ref lastCameraIndex),
                ConvertLights(entitiesLayers),
                GetActiveCameraIid(entitiesLayers),
                GetLevelBackground(selectedLevel, contentManager),
                Path.GetFullPath(Path.Combine(_mapBasePath, worldRelPath))
            );
        }

        private LevelBackground? GetLevelBackground(Level selectedLevel, ContentManager contentManager)
        {
            var bgPos = selectedLevel.BgPos;
            if(bgPos == null) { return null; }

            var size = new Size((int)bgPos.CropRect[2], (int)bgPos.CropRect[3]);
            var background = contentManager.Load<Texture2D>(selectedLevel.BgRelPath.TrimEnd(['p', 'n', 'g']).Trim(['.', '/']));
            var scale = new Vector2((float)bgPos.Scale[0], (float)bgPos.Scale[1]);
            var position = new Vector2(bgPos.TopLeftPx[0], bgPos.TopLeftPx[1]);
            return new LevelBackground(size, background, scale, position);
        }

        private string GetLevelPath(string worldRelPath)
        {
            return Path.GetFullPath(Path.Combine(_mapBasePath, worldRelPath));
        }

        private (long, Texture2D)[] ExtractWorldTilesetAtlussesWithLayersIds(LdtkJson world, string worldRelPath, ContentManager contentManager)
        {
            return world.Defs.Tilesets.Select(tileset => (tileset.Uid, LoadTilesetImage(worldRelPath, tileset.RelPath, contentManager))).ToArray();
        }

        private (long, string[])[] ExtractWorldRequiredTagsWithLayersIds(LdtkJson world)
        {
            return world.Defs.Layers.Select(layer => (layer.Uid, layer.RequiredTags)).ToArray();
        }

        private Layer[] ConvertLevel(Level lvl, (long, Texture2D)[] textureAtlusses, (long, string[])[] requiredTags, Definitions defs)
        {
            Layer[] ourLayers = new Layer[lvl.LayerInstances.Length];
            int ourLayerIndex = 0;
            foreach (var layer in lvl.LayerInstances)
            {
                var textureAtlas = textureAtlusses.Where(x => x.Item1 == layer.TilesetDefUid).Select(x => x.Item2).FirstOrDefault();
                var layerTags = requiredTags.Where(x => x.Item1 == layer.LayerDefUid).Select(x => x.Item2).First();
                var layerDefinition = defs.Layers.Where(x => x.Uid == layer.LayerDefUid).First();
                var tilesetDefinition = defs.Tilesets.Where(x => x.Uid == layer.TilesetDefUid).FirstOrDefault();

                var order = ourLayers.Length - ourLayerIndex;
                ourLayers[ourLayerIndex++] = layer.Type switch
                {
                    "Tiles" => ConvertTilesLayer(layer, textureAtlas, layerDefinition, tilesetDefinition, defs.Enums, order, false),
                    "IntGrid" => ConvertIntGridLayer(layer, textureAtlas, layerDefinition, tilesetDefinition, defs.Enums, order),
                    "AutoLayer" => ConvertAutoTileLayer(layer, textureAtlas, layerDefinition, tilesetDefinition, defs.Enums, order),
                    "Entities" => ConvertEntitiesLayer(layer, layerTags, order),
                    _ => throw new MapException($"Tile layer type {layer.Type} is not supported"),
                };
            }

            return ourLayers;
        }

        private IFactory<Entity>[] ConvertEntities(EntitiesLayer[] entitiesLayers, (long, Texture2D)[] textureAtlusses, ContentManager contentManager)
        {
            IFactory<Entity>[] allConvertedEntities = [];
            int index = 0;
            var layers = entitiesLayers.Where(layer => layer.RequiredTags.Contains("entity"));
            foreach (var layer in layers)
            {
                IFactory<Entity>[] convertedEntities = new IFactory<Entity>[layer.Entities.Length];
                var entities = layer.Entities.Where(x => x.Tags.Contains("entity"));
                foreach (var entity in entities)
                {
                    Texture2D textureAtlas = textureAtlusses.Where(x => x.Item1 == entity.Tile.TilesetUid).Select(x => x.Item2).First();

                    var iid = entity.Iid;
                    var position = new Vector2(entity.Px[0], entity.Px[1]);
                    var size = new Vector2(entity.Width, entity.Height);
                    var rectangle = new Microsoft.Xna.Framework.Rectangle((int)entity.Tile.X, (int)entity.Tile.Y, (int)entity.Tile.W, (int)entity.Tile.H);

                    convertedEntities[index++] = entity.Tags.Contains("_player") switch
                    {
                        true => new PlayerFactory(
                            iid,
                            textureAtlas,
                            size,
                            position,
                            new model.Models.Sound.Sound(
                                contentManager.Load<SoundEffect>(
                                    entity.FieldInstances.Single(field => field.Identifier == "Sound").Value.TrimEnd('w','a','v').Trim('.', '/')
                                ).CreateInstance()),
                            rectangle
                        ),

                        _ => new ItemFactory(iid, textureAtlas, size, position, rectangle)
                    };
                }
                allConvertedEntities = [.. allConvertedEntities, .. convertedEntities];
            }
            return allConvertedEntities;
        }

        private string? GetActiveCameraIid(EntitiesLayer[] entitiesLayers)
        {
            return entitiesLayers
                .Single(layer => layer.RequiredTags.Contains("_cameraManager"))
                .Entities.Single(entity => entity.Tags.Contains("_cameraManager"))
                .FieldInstances.Single(field => field.Identifier == "ActiveCamera")
                .Value["entityIid"];
        }

        private Camera[] ConvertCameras(EntitiesLayer[] entitiesLayers, ref int lastCameraId)
        {
            Camera[] allCameras = [];
            var cameraLayers = entitiesLayers.Where(layer => layer.RequiredTags.Contains("cam"));
            foreach (var layer in cameraLayers)
            {
                var ldtkCameras = layer.Entities.Where(x => x.Tags.Contains("cam"));
                Camera[] cameras = new Camera[ldtkCameras.Count()];
                int _lastCameraIndex = 0;
                foreach (var camera in ldtkCameras)
                {
                    string name = camera.FieldInstances.Single(x => x.Identifier == "Name").Value;
                    string iid = camera.Iid;
                    bool enabled = camera.FieldInstances.Single(x => x.Identifier == "Enabled").Value;
                    cameras[_lastCameraIndex++] = camera.Identifier switch
                    {
                        "FollowCamera" => new FollowCamera(
                            lastCameraId++,
                            iid,
                            name,
                            (string)camera.FieldInstances.Single(x => x.Identifier == "FollowEntity").Value["entityIid"],
                            enabled
                        ),

                        "StaticCamera" => new StaticCamera(
                            lastCameraId++,
                            iid,
                            name,
                            new Vector2(camera.Px[0], camera.Px[1]),
                            enabled
                        ),

                        _ => throw new MapException($"bad camera: {camera.Iid}")
                    };
                }
                allCameras = [.. allCameras, .. cameras];
            }
            return allCameras;
        }

        private Light[] ConvertLights(EntitiesLayer[] lightsLayers)
        {
            var gds = _serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService ?? throw new Exception("No graphics device manager");
            return lightsLayers
                .Where(x => x.RequiredTags.Contains("light"))
                .SelectMany(x => x.Entities)
                .Select(x => ConvertLight(gds.GraphicsDevice, x))
                .ToArray();
        }

        private Light ConvertLight(GraphicsDevice gd, EntityInstance light)
        {
            var color = light.FieldInstances.First(x => x.Identifier == "Base_color").Value;
            string type = light.FieldInstances.First(x => x.Identifier == "Light_types").Value;
            var size = light.Width;
            var position = new Vector2(light.Px[0], light.Px[1]);

            Microsoft.Xna.Framework.Color lightBaseColor = HexStringToColor(color);
            return type switch
            {
                "Point" => new PointLight(gd, position, (int)size, lightBaseColor),
                _ => throw new Exception("Light type not recognized")        
            };
        }

        private Microsoft.Xna.Framework.Color HexStringToColor(string hex)
        {
            var color = ColorTranslator.FromHtml(hex);
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
        }

        private EntitiesLayer ConvertEntitiesLayer(
            ldtk.LayerInstance layer,
            string[] tags,
            int order
        )
        {
            return new EntitiesLayer(layer.EntityInstances, tags, order);
        }
        
        private TilemapLayer ConvertTilesLayer(
            ldtk.LayerInstance layer, 
            Texture2D textureAtlas, 
            LayerDefinition layerDefinition, 
            TilesetDefinition tilesetDefinition, 
            EnumDefinition[] enums, 
            int order,
            bool? isCollidable = null
        )
        {
            return ConvertLayer(layer, textureAtlas, layerDefinition, tilesetDefinition, [], order, isCollidable ?? true);
        }

        private TilemapLayer ConvertIntGridLayer(
            ldtk.LayerInstance layer, 
            Texture2D textureAtlas, 
            LayerDefinition layerDefinition,
            TilesetDefinition tilesetDefinition, 
            EnumDefinition[] enums, 
            int order,
            bool? isCollidable = null
        )
        {
            //Only int grids dictate collision
            return ConvertLayer(layer, textureAtlas, layerDefinition, tilesetDefinition, enums, order, isCollidable ?? true);
        }

        private TilemapLayer ConvertAutoTileLayer(
            ldtk.LayerInstance layer, 
            Texture2D textureAtlas, 
            LayerDefinition layerDefinition, 
            TilesetDefinition tilesetDefinition, 
            EnumDefinition[] enums, 
            int order,
            bool? isCollidable = null
        )
        {
            return ConvertLayer(layer, textureAtlas, layerDefinition, tilesetDefinition, [], order, isCollidable ?? false);
        }

        private TilemapLayer ConvertLayer(
            ldtk.LayerInstance layer, 
            Texture2D textureAtlas, 
            LayerDefinition layerDefinition, 
            TilesetDefinition tilesetDefinition, 
            EnumDefinition[] enums, 
            int order,
            bool isCollidable
        )
        {
            var tilemapSize = new Size((int)layer.CWid, (int)layer.CHei);
            var tileSize = new Size((int)layer.GridSize, (int)layer.GridSize);
            var collisionEnum = enums.Where(x => x.Tags.Contains("collision")).FirstOrDefault();

            var tileArray = layer.AutoLayerTiles.Select(tile => 
            {
                var atlasPosition = new Microsoft.Xna.Framework.Rectangle((int)tile.Src[0], (int)tile.Src[1], tileSize.Width, tileSize.Height);
                var collision = GetTileCollision(tile, tilesetDefinition, collisionEnum);
                return new Tile(tileSize, textureAtlas, new Vector2(tile.Px[0], tile.Px[1]), collision, atlasPosition);
            }).ToArray();
            return new TilemapLayer(tilemapSize, tileSize, tileArray, _mapGrouper, true, layer.Visible, isCollidable)
            {
                ParallaxFactor = new Vector2((float)layerDefinition.ParallaxFactorX, (float)layerDefinition.ParallaxFactorY),
                IsParallaxScaled = layerDefinition.ParallaxScaling,
                Order = order
            };
        }

        private static TileCollisionEnum GetTileCollision(TileInstance tile, TilesetDefinition tilesetDefinition, EnumDefinition? collisionEnum)
        {
            var enumTag = tilesetDefinition.EnumTags.FirstOrDefault(x => x.TileIds.Contains(tile.T));
            if(collisionEnum == null || enumTag == null)
            {
                return TileCollisionEnum.NoCollision; //no collision enum - no collisions
            }
            
            return Enum.TryParse<TileCollisionEnum>(enumTag.EnumValueId, out var parseResult) ? parseResult : TileCollisionEnum.NoCollision;
        }

        private Texture2D LoadTilesetImage(string worldRelPath, string tilesetRelPath, ContentManager contentManager)
        {
            var worldDirectory = Path.GetDirectoryName(GetLevelPath(worldRelPath)) ?? throw new MapException("No worldRelPath detected");
            var resultPath = Path.GetFullPath(Path.Combine(worldDirectory, tilesetRelPath));
            return contentManager.Load<Texture2D>(resultPath);
        }

        // |0|1|2|3| INDEX = y * width + x
        // |4|5|6|7| y = INDEX div width
        // |8|9|0|1| x = INDEX mod width
        // |2|3|4|5|

        private Microsoft.Xna.Framework.Rectangle CalculateAtlasPosition(long tileIndex, int atlasSizeWidthPix, Size tileSizePix)
        {
            int tilemapSizeWidth = (int)Math.Floor((double)(atlasSizeWidthPix / tileSizePix.Width));

            int indexX = (int)(tileIndex % tilemapSizeWidth), 
                indexY = (int)(tileIndex / tilemapSizeWidth);

            return new Microsoft.Xna.Framework.Rectangle(indexX * tileSizePix.Width, indexY * tileSizePix.Height, tileSizePix.Width, tileSizePix.Height);
        }
    }
}