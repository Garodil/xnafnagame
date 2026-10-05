using model.Interfaces;
using model.Interfaces.Service;
using model.Interfaces.Sound;

namespace scenes.Manager
{
    public class SceneManager(IServiceProvider services) : ISceneManager
    {
        private readonly IServiceProvider _services = services;

        private IScene _currentScene; 

        public IScene GetScene() 
        {
            return _currentScene;
        }

        public void ChangeScene(IScene newScene)
        {
            _currentScene?.OnExit( _services);
            
            var musicQueueService = _services.GetService(typeof(IMusicQueueService)) as IMusicQueueService;
            musicQueueService?.ClearQueue();
            
            _currentScene = newScene;
           
            _currentScene.OnEnter(_services);
        }

        public void ReloadCurrentScene() {
            _currentScene?.OnExit(_services);
            _currentScene?.OnEnter(_services);
        }
    }
}