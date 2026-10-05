using System;
using model.Interfaces.Service;
using Microsoft.Xna.Framework;
using System.Linq;
using model.Models.Input;
using scenes.Scenes;
using model.Interfaces.Repository;

namespace logic.Component
{
    public class GameLogicComponent: IUpdateable, IGameComponent
    {
        private IServiceProvider _serviceProvider;
        private IInputManagerService _inputManagerService;
        private ISceneManager _sceneManager;
        private ISceneManagerRepository _sceneRepository;
        private ISceneChanger _sceneChanger;

        public GameLogicComponent(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            _sceneManager = _serviceProvider.GetService(typeof(ISceneManager)) as ISceneManager ?? throw new Exception();
            _sceneRepository = _serviceProvider.GetService(typeof(ISceneManagerRepository)) as ISceneManagerRepository ?? throw new Exception();
            _sceneChanger = _serviceProvider.GetService(typeof(ISceneChanger)) as ISceneChanger ?? throw new Exception();
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
            var currentScene = _sceneManager.GetScene();
            currentScene.ProcessInputs(gameTime, _inputManagerService.Actions);
            if (currentScene.IsPaused() == false)
            {
                currentScene.GetState().MakePhysicsStep(gameTime);
            }
        }
        
    }
}
    
