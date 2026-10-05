using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using model.Interfaces;
using model.Interfaces.Service;
using model.Models.Input;
using Menu = scenes.Menus.Menu;

namespace logic.Component
{
    public class InputLogicComponent: IUpdateable, IGameComponent
    {
        private IServiceProvider _serviceProvider;
        private IInputManagerService _inputManagerService;
        public InputLogicComponent(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            _inputManagerService = _serviceProvider.GetService(typeof(IInputManagerService)) as IInputManagerService ?? throw new Exception();
        }

        private bool _enabled = true;
        public bool Enabled { 
            set {
                _enabled = value;
                EnabledChanged?.Invoke(this, new EventArgs() );
            }
            get {
                return _enabled;
            }
        }

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

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public void Update(GameTime gameTime)
        {
            _inputManagerService.ProcessActions();
        }
    }
}