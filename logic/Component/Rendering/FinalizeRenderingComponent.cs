using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using model.Interfaces.Service;
using model.Models.Render;

namespace logic.Component
{
    public class FinalizeRenderingComponent : IGameComponent, IDrawable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ContentManager _contentManager;
        private SpriteBatch _scalingBatch;
        private IGraphicsDeviceService _graphicsDeviceService;
        private IRenderTargetService _renderTargetService;
        private Effect _fovEffect;
        private int _drawOrder = 0;
        private bool _isVisible = false;
        private int _pixelization = 2;

        public FinalizeRenderingComponent(IServiceProvider serviceProvider, ContentManager content)
        {
            _serviceProvider = serviceProvider;
            _contentManager = content;
        }

        public int DrawOrder { 
            set {
                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, new EventArgs());
            }
            get {
                return _drawOrder;
            }
        }

        public bool Visible { 
            private set {
                _isVisible = value;
                VisibleChanged?.Invoke(this, new EventArgs());
            }
            get {
                return _isVisible;
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public void Draw(GameTime gameTime)
        {
            var fovRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.Fov);
            var uiRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.Ui);
            var worldRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.World);
            var debugUiRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.DebugUi);
            var debugInWorldRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.DebugInWorld);
            var lightRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.Light);
            var mixRenderTarget = _renderTargetService.GetRenderTarget(RenderTargetEnum.Mix);
            _fovEffect.CurrentTechnique = _fovEffect.Techniques[0];
            _fovEffect.Parameters["Mask"].SetValue(fovRenderTarget);
            _fovEffect.Parameters["Light"].SetValue(lightRenderTarget);

            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(mixRenderTarget);
            _scalingBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.NonPremultiplied, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                _fovEffect,
                Matrix.Identity
            );
            _scalingBatch.Draw(
                worldRenderTarget, 
                new Rectangle(
                    0, 0, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Width, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Height
                ),
                Color.White
            );     
            _scalingBatch.End();

            _scalingBatch.Begin(
                SpriteSortMode.Immediate, 
                BlendState.Additive, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                null,
                Matrix.Identity
            );
            _scalingBatch.Draw(
                uiRenderTarget, 
                new Rectangle(
                    0, 0, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Width, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Height
                ),
                Color.White
            );       
            _scalingBatch.Draw(
                debugInWorldRenderTarget, 
                new Rectangle(
                    0, 0, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Width, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Height
                ),
                Color.White
            );
            _scalingBatch.Draw(
                debugUiRenderTarget, 
                new Rectangle(
                    0, 0, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Width, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Height
                ),
                Color.White
            );
            _scalingBatch.End();

            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(null);
            _scalingBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.NonPremultiplied, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone,
                null,
                Matrix.Identity
            );
            _scalingBatch.Draw(
                mixRenderTarget, 
                new Rectangle(
                    0, 0, 
                    _graphicsDeviceService.GraphicsDevice.Viewport.Width, _graphicsDeviceService.GraphicsDevice.Viewport.Height
                ),
                Color.White
            );
            _scalingBatch.End();
        }

        public void Initialize()
        {
            var gds = _serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            if(gds == null)
            {
                throw new ArgumentNullException("Graphics device service not known");
            }
            _renderTargetService = _serviceProvider.GetService(typeof(IRenderTargetService)) as IRenderTargetService ?? throw new Exception();
            _graphicsDeviceService = gds;
            if(gds.GraphicsDevice != null)
            {
                SetupGraphicsDeviceDependentElements(gds.GraphicsDevice);
            }
            _graphicsDeviceService.DeviceCreated += OnDeviceCreated;
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
            _fovEffect = _contentManager.Load<Effect>("Resource/Effects/VisibilityShader");
            _scalingBatch = new SpriteBatch(gd);
            Visible = true;
            foreach(var targerName in Enum.GetValues(typeof(RenderTargetEnum)).Cast<RenderTargetEnum>())
            {
                _renderTargetService.InitRenderTarget(targerName, _pixelization, gd);
            }
        }
        
    }
}