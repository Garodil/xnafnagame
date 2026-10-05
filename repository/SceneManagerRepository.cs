using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using model.Interfaces;
using model.Interfaces.Repository;

namespace repository
{
    public class SceneManagerRepository (IServiceProvider services, ContentManager content) : ISceneManagerRepository
    {
        private readonly IServiceProvider _services = services;
        private readonly ContentManager _content = content;
        private readonly IDictionary<string, IScene> _scenes = new Dictionary<string, IScene>();
        public void AddScene(IScene scene)
        {
            scene.Load(_content, _services);
            _scenes.Add(scene.Name, scene);
        }

        public IScene? GetScene(string name)
        {
            IScene? scene = _scenes[name];
            return scene;
        }

        public void RemoveScene(string name)
        {
            IScene? scene = _scenes[name];
            scene?.Unload(_services);
            _scenes.Remove(name);
        }

        public void RemoveScene(IScene scene)
        {
            scene.Unload(_services);
            _scenes.Remove(scene.Name);
        }
    }
}