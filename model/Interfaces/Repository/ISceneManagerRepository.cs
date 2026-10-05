namespace model.Interfaces.Repository
{
    public interface ISceneManagerRepository
    {
        public IScene? GetScene(string sceneName);
        public void AddScene(IScene scene);

        public void RemoveScene(string sceneName);
        public void RemoveScene(IScene scene);
    }
}