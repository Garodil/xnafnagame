using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using model.Interfaces.Service;
using model.Models.Input;
using model.Models.Ui.Commands;
using scenes.Manager;

namespace scenes.Scenes
{

    public class MainMenuScene(string name) : Scene(name)
    {
        WeakReference<IMenuManager> _menuManager;
        protected override void LoadScene(ContentManager content, IServiceProvider services)
        {  
            _menuManager = new WeakReference<IMenuManager>(services.GetService(typeof(IMenuManager)) as IMenuManager ?? throw new Exception());
        }

        public override void OnEnter(IServiceProvider services)
        {
            _menuManager = new WeakReference<IMenuManager>(services.GetService(typeof(IMenuManager)) as IMenuManager ?? throw new Exception());
            if (_menuManager.TryGetTarget(out var menuManager))
            {
                menuManager.PushCommand(new ShowMenuCommand());
                menuManager.PushCommand(new ShowMenuVariantCommand(MenuTypeEnum.MainMenu));
            }
        }

        public override void OnExit(IServiceProvider services)
        {
        }

        protected override void UnloadScene(ContentManager content, IServiceProvider services)
        {
        }

        public override void ProcessInputs(GameTime gt, InputDataModel actions)
        {
        }

        public override bool IsPaused()
        {
            return false;
        }
    }
}