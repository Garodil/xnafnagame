namespace model.Models.Ui.Commands
{
    public class SwitchSceneCommand : MenuCommand
    {
        private readonly string _sceneName;

        public SwitchSceneCommand(string sceneName) : base($"Switch to scene {sceneName}")
        {
            _sceneName = sceneName;
        }

        public string SceneName => _sceneName;
    }
}