using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using logic.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using model.Game;
using model.Interfaces.Service;
using model.Models.Game;
using model.Models.Game.Tilemap;
using model.Models.Primitives;
using model.Models.Render;
using Newtonsoft.Json;
using service.Logic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace logic.Component
{
    public class WorldRenderingComponent : IGameComponent, IDrawable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ContentManager _contentManager;
        private SpriteBatch _spriteBatch, _fovBatch, _lightBatch;
        private IGraphicsDeviceService _graphicsDeviceService;
        private IRenderTargetService _renderTargetService;
        private ISceneManager _sceneManager;
        private IFovService _fovService;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        private int _pixelize = 2;

        private int _drawOrder;
        public int DrawOrder { 
            set {
                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, new EventArgs());
            } 
            get {
                return _drawOrder;
            } 
        }
        private bool _visible = false;
        public bool Visible { 
            set {
                _visible = value;
                VisibleChanged?.Invoke(this, new EventArgs() );
            }
            get {
                return _visible;
            }
        }

        Texture2D _debugTex;

        Color _maskColor = Color.Black;
        Color _unMaskColor = Color.White;
        Color _debugColorRed = new Color((byte)220, (byte)0.7, (byte)0.7, (byte)120);
        Color _debugColorGreen = new Color((byte)0.7, (byte)220, (byte)0.7, (byte)120);
        Color _debugColorBlue = new Color((byte)0.7, (byte)0.7, (byte)220, (byte)120);
        Color _debugColorPurple = new Color((byte)128, (byte)0.7, (byte)128, (byte)120);
        Color _debugColorBlack = new Color((byte)0, (byte)0, (byte)0, (byte)120);



        public WorldRenderingComponent(IServiceProvider serviceProvider, ContentManager content)
        {
            _serviceProvider = serviceProvider;
            _contentManager = content;
        }

        public void Initialize()
        {
            var gds = _serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            if(gds == null)
            {
                throw new ArgumentNullException("Graphics device service not known");
            }
            _debugTex = new Texture2D(gds.GraphicsDevice, 1, 1);
            _debugTex.SetData(new Microsoft.Xna.Framework.Color[1] { Microsoft.Xna.Framework.Color.White });
            _graphicsDeviceService = gds;
            if(gds.GraphicsDevice != null)
            {
                SetupGraphicsDeviceDependentElements(gds.GraphicsDevice);
            }
            _graphicsDeviceService.DeviceCreated += OnDeviceCreated;
            _sceneManager = _serviceProvider.GetService(typeof(ISceneManager)) as ISceneManager ?? throw new Exception();
            _fovService = _serviceProvider.GetService(typeof(IFovService)) as IFovService ?? throw new Exception();
            _renderTargetService = _serviceProvider.GetService(typeof(IRenderTargetService)) as IRenderTargetService ?? throw new Exception();
        }

        private void OnDeviceCreated (object? sender, EventArgs args)
        {
            if(sender == null)
            {
                throw new ArgumentNullException(nameof(sender), "sender must not be null");
            }
            SetupGraphicsDeviceDependentElements((sender as IGraphicsDeviceService).GraphicsDevice);
        }

        private void SetupGraphicsDeviceDependentElements(GraphicsDevice gd)
        {
            _spriteBatch = new SpriteBatch(gd);
            _lightBatch = new SpriteBatch(gd);
            _fovBatch = new SpriteBatch(gd);
            Visible = true;
        }

        public void Draw(GameTime gameTime)
        {
            if(_graphicsDeviceService.GraphicsDevice == null)
            {
                return;
            }
            var state = GetState();
            
            state.CameraManager.SetViewportSize(
                _graphicsDeviceService.GraphicsDevice.Viewport.Width,  
                _graphicsDeviceService.GraphicsDevice.Viewport.Height
            );

            var fovRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.Fov);
            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(fovRenderTarget);
            _graphicsDeviceService.GraphicsDevice.Clear(Color.White); 
            
            float maxDistance = Math.Max(_graphicsDeviceService.GraphicsDevice.Viewport.Width, _graphicsDeviceService.GraphicsDevice.Viewport.Height);
            DrawFov(gameTime, state, maxDistance);

            var textureRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.World);
            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(textureRenderTarget);
            _graphicsDeviceService.GraphicsDevice.Clear(Color.Black);
            DrawScene(gameTime, state);

            var lightRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.Light);
            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(lightRenderTarget);
            _graphicsDeviceService.GraphicsDevice.Clear(Color.Black);
            DrawLight(gameTime, state);
        }

        private void DrawLight(GameTime gameTime, State state)
        {
            _spriteBatch.Begin(
                SpriteSortMode.Immediate, 
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                null,
                state.CameraManager.GetActiveCameraMatrix()
            );
            
            foreach (var light in state.Lights)
            {
                _spriteBatch.Draw(light.GetLightTexture(), light.Position, null, light.ModulateColor, 0f, light.Origin, 1, SpriteEffects.None, 0);
            }
            _spriteBatch.End();
        }

        private void DrawFov(GameTime gameTime, State state, float maxDistance)
        {

            var cameraPosition = state.CameraManager.ActiveCamera?.Coordinates;
            var fovLayer = state.DefaultTilemapLayer;
            if(fovLayer == null || cameraPosition == null)
            {
                return;
            }

            var tiles = fovLayer.GetTiles();
            var cameraPositionConvered = ConvertVector2(cameraPosition.Value);

            _fovBatch.Begin(
                SpriteSortMode.Immediate, 
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                null,
                state.CameraManager.GetActiveCameraMatrix()
            );
            
            var orderedTiles = _fovService.OrderTilesByDistanceToPlayer(tiles, cameraPositionConvered).ToArray();
            var visibleRibs = _fovService.GetVisibleRibs(ref orderedTiles, cameraPositionConvered, maxDistance);
            _fovService.ExtendRibsForTile(ref orderedTiles, cameraPositionConvered, maxDistance);
            foreach (var tile in orderedTiles.Where(x => x.IsVisible == true))
            {
                tile.VisibleRibs.Sort(new Rib.AngleComparer());
                for (int i = 0; i < tile.VisibleRibs.Count - 1; i += 1)
                {
                    Rib firstRib = tile.VisibleRibs[i];
                    Rib lastRib = tile.VisibleRibs[i + 1];
                    RenderHelper.DrawScuffedQuad(
                        _fovBatch,
                        _debugTex,
                        new Quad()
                        {
                            X1 = firstRib.X1,
                            Y1 = firstRib.Y1,
                            X2 = firstRib.X2,
                            Y2 = firstRib.Y2,
                            X3 = lastRib.X2,
                            Y3 = lastRib.Y2,
                            X4 = lastRib.X1,
                            Y4 = lastRib.Y1
                        },
                        _maskColor);
                }
                RenderHelper.DrawScuffedQuad(
                    _fovBatch,
                    _debugTex,
                    MyMathHelper.GetTileQuad(tile.Tile),
                    _unMaskColor
                );
            }

            _fovBatch.End();
        }

        private void DrawScene(GameTime gameTime, State state)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, default, state.CameraManager.GetActiveCameraMatrix());
            var cameraPosition = state.CameraManager.ActiveCamera?.Coordinates;
            if (state.LevelBackground != null) { DrawBackground(state.LevelBackground); }
            DrawTilemapLayers(
                gameTime, 
                state.TilemapLayers.Where(x => x.IsVisible == true).OrderBy(x => x.Order), 
                ConvertVector2(cameraPosition.GetValueOrDefault())
            );
            DrawEntities(state.Drawables.OrderBy(x => x.DrawOrder));
            _spriteBatch.End();
        }

        
        private Vector2 ConvertVector2(Vector2 vec2)
        {
            return vec2;
        }
        

        private void DrawBackground(LevelBackground levelBackground)
        {
            _spriteBatch.Draw(
                levelBackground.Texture,
                levelBackground.Position,
                null,
                Color.White,
                0,
                Vector2.Zero,
                levelBackground.Scale,
                SpriteEffects.None,
                0
            );
        }

        private void DrawTilemapLayers(GameTime time, IEnumerable<TilemapLayer> layers, Vector2 parallaxFocalPoint)
        {
            foreach (var tileLayer in layers)
            {
                var parallaxScale = tileLayer.IsParallaxScaled ? Vector2.One - tileLayer.ParallaxFactor : Vector2.One;
                var parallaxShift = Vector2.Multiply(tileLayer.ParallaxFactor, parallaxFocalPoint);
                var maxShift = Vector2.Multiply(tileLayer.ParallaxFactor, new Vector2(tileLayer.TilemapSizePixels.Width, tileLayer.TilemapSizePixels.Height));
                parallaxShift = Vector2.Clamp(parallaxShift, Vector2.Zero, maxShift);
                foreach (var tile in tileLayer.GetTiles())
                {
                    _spriteBatch.Draw(
                        tile.Texture,
                        Vector2.Multiply(tile.Position, parallaxScale) + parallaxShift,
                        tile.AtlasPosition,
                        Microsoft.Xna.Framework.Color.White,
                        0,
                        Vector2.Zero,
                        parallaxScale,
                        SpriteEffects.None,
                        0
                    );
                }
            }
        }

        private void DrawEntities(IEnumerable<model.Interfaces.Objects.IDrawable> drawables)
        {
            foreach(var drawable in drawables)
            {
                var texture = drawable.GetTexture();
                _spriteBatch.Draw(
                    texture, 
                    ConvertVector2(drawable.GetTexturePosition()), 
                    drawable.GetTextureRectangle(), 
                    Microsoft.Xna.Framework.Color.White, 
                    drawable.GetTextureRotation(), 
                    ConvertVector2(drawable.GetTextureOrigin()), 
                    ConvertVector2(drawable.GetTextureScale()), 
                    SpriteEffects.None,
                    0
                );
            }
        }

        private State GetState()
        {
            return _sceneManager.GetScene().GetState();
        }
    }
}