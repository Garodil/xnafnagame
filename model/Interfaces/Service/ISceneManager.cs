namespace model.Interfaces.Service
{
    public interface ISceneManager
    {
        public IScene GetScene();
        public void ChangeScene(IScene neWscene);
        public void ReloadCurrentScene();
    }
}