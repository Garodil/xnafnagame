using System;
using System.Linq;
using model.Game;
using model.Interfaces.Factory;
using model.Interfaces.Service;
using model.Models.Entities;
using scenes.Factory;

namespace service.Logic
{
    public class GameSpawnService : IGameSpawnService
    {
        private IFactory<Player> _playerFactory;
        private IFactory<Entity>[] _factories = [];

        public GameSpawnService(IServiceProvider serviceProvider)
        {
            _playerFactory = serviceProvider.GetService(typeof(IFactory<Player>)) as IFactory<Player> ?? new EmptyFactory<Player>();
        }

        public void AddFactory<Type>(IFactory<Type> factory)
        {
            if (typeof(Type) == typeof(Player))
            {
                _playerFactory = (IFactory<Player>)factory;
            }
        }

        public void AddFactory(IFactory<Entity>[] factories)
        {
            _factories = [.. _factories, .. factories];
        }

        public void DespawnEntity(State gameState, Entity entity)
        {
            gameState.RemoveEntity(entity);
        }

        public void DespawnEntity(State gameState, Entity[] entities)
        {
            gameState.RemoveEntity(entities);
        }

        public void SpawnEntity(State gameState, Entity entity)
        {
            gameState.AddEntity(entity);
        }

        public void SpawnEntity(State gameState, Entity[] entities)
        {
            gameState.AddEntity(entities);
        }

        public void SpawnEntity<Type>(State gameState, bool isVisible = true, bool isEnabled = true)
        {
            Entity entity = null;
            if (typeof(Type) == typeof(Player))
            {
                var playerFactory = _factories.OfType<PlayerFactory>().Single();

                entity = playerFactory.Get(gameState);
                entity.Visible = isVisible;
                entity.Enabled = isEnabled;

                SpawnEntity(gameState, entity);
            }
            if (typeof(Type) == typeof(Item))
            {
                var items = _factories.OfType<ItemFactory>().Select(factory => factory.Get(gameState)).ToArray();
                foreach (var item in items)
                {
                    item.Visible = isVisible;
                    item.Enabled = isEnabled;
                }
                SpawnEntity(gameState, items);
            }
        }

        public void ClearFactories()
        {
            _factories = [];
        }
    }
}