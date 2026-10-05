using model.Game;
using model.Interfaces.Factory;
using model.Models.Entities;

namespace model.Interfaces.Service
{
    public interface IGameSpawnService
    {
        public void AddFactory<Type>(IFactory<Type> factory);
        public void AddFactory(IFactory<Entity>[] factories);

        public void SpawnEntity<Type>(State gameState, bool isVisible = true, bool isEnabled = true);

        public void DespawnEntity(State gameState, Entity entity);
        public void DespawnEntity(State gameState, Entity[] entities);
        public void ClearFactories();
    }
}