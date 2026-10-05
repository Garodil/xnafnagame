using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using model.Game;
using model.Interfaces;
using model.Models.Input;

namespace scenes.Manager
{
    public abstract class Scene(string name) : IScene
    {
        protected readonly string _name = name;
        public string Name => _name;

        protected State _sceneState = new State();

        private bool _isLoaded = false;

        private ContentManager _sceneContentManager;

        public State GetState() 
        {
            return _sceneState;
        }

        public bool IsLoaded() => _isLoaded;

        public void Load(ContentManager content, IServiceProvider services)
        {
            _sceneContentManager = new ContentManager(services, content.RootDirectory);
            LoadScene(_sceneContentManager, services);
            _isLoaded = true;
            Debug.WriteLine($"Scene loaded: {this.GetType().FullName}");
        }

        public void Unload(IServiceProvider services)
        {
            UnloadScene(_sceneContentManager, services);
            _sceneContentManager.Unload();
            _sceneContentManager.Dispose();
            _sceneState = null;
            _isLoaded = false;
        }

        protected abstract void LoadScene(ContentManager content, IServiceProvider services);

        protected abstract void UnloadScene(ContentManager content, IServiceProvider services);

        public abstract void OnEnter(IServiceProvider services);

        public abstract void OnExit(IServiceProvider services);

        public abstract void ProcessInputs(GameTime gt, InputDataModel actions);
        public abstract bool IsPaused();
    }
}
