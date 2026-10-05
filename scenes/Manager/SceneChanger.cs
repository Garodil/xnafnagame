using model.Interfaces.Repository;
using model.Interfaces.Service;

namespace scenes.Manager
{
    public class SceneChanger : ISceneChanger
    {
        private ISceneManager _sceneManager;
        private ISceneManagerRepository _sceneRepository;
        public SceneChanger(ISceneManager sceneManager, ISceneManagerRepository sceneManagerRepository)
        {
            _sceneManager = sceneManager;
            _sceneRepository = sceneManagerRepository;
        }

        public void ChangeScene(string name)
        {
            _sceneManager.ChangeScene(_sceneRepository.GetScene(name) ?? throw new Exception()) ;
        }
    }
}