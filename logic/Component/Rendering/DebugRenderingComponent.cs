using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using model.Game;
using model.Interfaces.Service;
using model.Models.Input;
using model.Models.Render;
using Box2D.NET;
using System.Linq;
using model.Models.Render.Debug;
using logic.Render;

namespace logic.Component
{
    public class DebugRenderingComponent : IGameComponent, IDrawable, IUpdateable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ContentManager _contentManager;
        private IInputManagerService _inputManagerService;
        private IRenderTargetService _renderTargetService;
        private SpriteBatch _debugBatch;
        private IGraphicsDeviceService _graphicsDeviceService;
        private ISceneManager _sceneManager;
        private Texture2D _debugTex;
        private SpriteFont _debugFont;
        private readonly string _version;

        private int _currentDebugLayer = 0;
        private DebugLayer[] _debugLayers = [];

        private DebugLayer? GetCurrentDebugLayer()
        {
            if(_currentDebugLayer < 0 || _currentDebugLayer >= _debugLayers.Length)
            {
                return null;
            }
            
            return _debugLayers[_currentDebugLayer];
        }

        Color _debugColorRed = new Color((byte)220, (byte)0.7, (byte)0.7, (byte)120);
        Color _debugColorGreen = new Color((byte)0.7, (byte)220, (byte)0.7, (byte)120);
        Color _debugColorBlue = new Color((byte)0.7, (byte)0.7, (byte)220, (byte)120);
        Color _debugColorPurple = new Color((byte)128, (byte)0.7, (byte)128, (byte)120);
        Color _debugColorBlack = new Color((byte)0, (byte)0, (byte)0, (byte)120);

        public DebugRenderingComponent(IServiceProvider serviceProvider, ContentManager content, string version)
        {
            _serviceProvider = serviceProvider;
            _contentManager = content;
            _version = version;
        }


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
        public bool Enabled => true;
        private int _updateOrder;
        public int UpdateOrder { 
            set {
                _updateOrder = value;
                UpdateOrderChanged?.Invoke(this, new EventArgs());
            } 
            get {
                return _updateOrder;
            } 
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public void Draw(GameTime gameTime)
        {
            var state = GetState();
            GetCurrentDebugLayer()?.CalculateRenderDataLayer(gameTime, state);
            var debugInWorld = _renderTargetService.GetRenderTarget(RenderTargetEnum.DebugInWorld);
            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(debugInWorld);
            _graphicsDeviceService.GraphicsDevice.Clear(Color.Transparent); 
            _debugBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                null,
                state.CameraManager.GetActiveCameraMatrix()
            );
            GetCurrentDebugLayer()?.DrawInWorldLayer();
            _debugBatch.End();

            var debugUi = _renderTargetService.GetRenderTarget(RenderTargetEnum.DebugUi);
            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(debugUi);
            _graphicsDeviceService.GraphicsDevice.Clear(Color.Transparent);
            
            _debugBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                null,
                Matrix.Identity
            );
            GetCurrentDebugLayer()?.DrawOverlayLayer();
            _debugBatch.End();
        }

        public void Initialize()
        {
            var gds = _serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            if (gds == null)
            {
                throw new ArgumentNullException("Graphics device service not known");
            }
            
            _sceneManager = _serviceProvider.GetService(typeof(ISceneManager)) as ISceneManager ?? throw new Exception();
            _inputManagerService = _serviceProvider.GetService(typeof(IInputManagerService)) as IInputManagerService ?? throw new Exception();
            _renderTargetService = _serviceProvider.GetService(typeof(IRenderTargetService)) as IRenderTargetService ?? throw new Exception();

            _graphicsDeviceService = gds;
            if (gds.GraphicsDevice != null)
            {
                GraphicsDeviceInit(gds);
            }
            _graphicsDeviceService.DeviceCreated += OnDeviceCreated;
        }

        private void OnDeviceCreated(object? sender, EventArgs e)
        {
            if(sender == null)
            {
                throw new ArgumentNullException(nameof(sender), "sender must not be null");
            }
            GraphicsDeviceInit((IGraphicsDeviceService)sender);
        }

        private void GraphicsDeviceInit(IGraphicsDeviceService graphicsDeviceService)
        {
            _debugBatch = new SpriteBatch(graphicsDeviceService.GraphicsDevice);
            _debugTex = new Texture2D(graphicsDeviceService.GraphicsDevice, 1, 1);
            _debugTex.SetData(new Color[1] { Color.White });
            _debugFont = _contentManager.Load<SpriteFont>("Resource/Fonts/dejavu-sans-mono");

            var fovService = _serviceProvider.GetService(typeof(IFovService)) as IFovService ?? throw new Exception();

            _debugLayers = [
                new DebugLayerBasic(_version, _sceneManager, _debugBatch, _debugBatch, _debugFont, _debugTex),
                new DebugLayerCollision(_version, _debugColorRed, _debugColorGreen, _debugColorBlue, _debugBatch, _debugBatch, _debugFont, _debugTex),
                new DebugLayerFov(
                    _version, 
                    _debugBatch, _debugBatch, 
                    _debugFont, _debugTex, 
                    fovService, 
                    _debugColorBlue, _debugColorPurple, _debugColorRed, 
                    _debugColorPurple, _debugColorGreen, _debugColorGreen
                )
            ];
        }

        private State GetState()
        {
            return _sceneManager.GetScene().GetState();
        }

        public void Update(GameTime gameTime)
        {
            var action = _inputManagerService.Actions;
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.F2, out var F2);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.F3, out var F3);

            if (F2 == InputStateEnum.Pressed)
            {
                Visible = !Visible;
                if(Visible == false)
                {
                    var debugInWorld = _renderTargetService.GetRenderTarget(RenderTargetEnum.DebugInWorld);
                    _graphicsDeviceService.GraphicsDevice.SetRenderTarget(debugInWorld);
                    _graphicsDeviceService.GraphicsDevice.Clear(Color.Transparent); 

                    var debugUi = _renderTargetService.GetRenderTarget(RenderTargetEnum.DebugUi);
                    _graphicsDeviceService.GraphicsDevice.SetRenderTarget(debugUi);
                    _graphicsDeviceService.GraphicsDevice.Clear(Color.Transparent); 
                }
            }
            if(F3 == InputStateEnum.Pressed)
            {
                _currentDebugLayer += 1;
                if(_currentDebugLayer >= _debugLayers.Length)
                {
                    _currentDebugLayer = 0;
                }
            }

            if(Visible == true)
            {
                GetCurrentDebugLayer()?.CalculateDataLayer(gameTime, GetState());
            }
        }
    }
}