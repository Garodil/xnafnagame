using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using model.Interfaces.Service;

namespace logic.Component
{
    public class MainMenuComponent : IGameComponent, IDrawable, IUpdateable
    {
        private readonly IServiceProvider _serviceProvider;
        private IInputManagerService _inputManagerService;
        private IMenuManager _menuManager;
        private IRenderTargetService _renderTargetService;
        private IGraphicsDeviceService _graphicsDeviceService;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

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
        public bool Visible => true;
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

        public MainMenuComponent(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize() {
            _menuManager = _serviceProvider.GetService(typeof(IMenuManager)) as IMenuManager ?? throw new Exception("No menu manager");
            _inputManagerService = _serviceProvider.GetService(typeof(IInputManagerService)) as IInputManagerService ?? throw new Exception("No input manager");
            _renderTargetService = _serviceProvider.GetService(typeof(IRenderTargetService)) as IRenderTargetService ?? throw new Exception();
            _graphicsDeviceService = _serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService ?? throw new Exception();
        }

        public void Draw(GameTime gameTime)
        {
            var ui = _renderTargetService.GetRenderTarget(model.Models.Render.RenderTargetEnum.Ui);
            _graphicsDeviceService.GraphicsDevice.SetRenderTarget(ui);
            _graphicsDeviceService.GraphicsDevice.Clear(Color.Transparent);
            _menuManager.Render(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            _menuManager.Update(gameTime, _inputManagerService.Actions);
        }
    }
}