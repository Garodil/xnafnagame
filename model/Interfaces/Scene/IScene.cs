using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using model.Game;
using model.Models.Input;

namespace model.Interfaces
{
    public interface IScene
    {
        public string Name { get; }

        public bool IsLoaded();

        public State GetState();

        public void OnEnter(IServiceProvider serviceProvider);

        public void OnExit(IServiceProvider serviceProvider);

        public void Load(ContentManager contentManager, IServiceProvider serviceProvider);

        public void Unload(IServiceProvider serviceProvider);
        
        public void ProcessInputs(GameTime gt, InputDataModel actions);
        public bool IsPaused();
    }
}