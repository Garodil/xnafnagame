using Microsoft.Xna.Framework.Content;
using Myra.Graphics2D.UI;
using model.Interfaces.Service;
using scenes.Menus.Commands;
using model.Models.Ui.Commands;
using model.Models.Ui.Elements;
using System.Runtime.Serialization;

namespace scenes.Menus {
    public class MainMenu : Menu
    {   
        public MainMenu(IMenuManager menuManager) 
            : base(null, menuManager)
        {
        }

        private MenuButton _resumeButton, _newGameButton, _quitToMenuButton, _quitButton;

        protected override Container InitContainer()
        {
            _resumeButton = new MenuButton("Resume", this, new UpMenuCommand(), false);
            _newGameButton = new MenuButton("New game", this, new SwitchSceneCommand("void"), true);
            _quitToMenuButton = new MenuButton("Quit to menu", this, new SwitchSceneCommand("main"), false);
            _quitButton = new MenuButton("Quit", this, new ExitCommand(), true);

            var addButtonCommands = new MenuButton[] {
                _resumeButton,
                _newGameButton,
                new MenuButton("Load", this, new SwitchMenuCommand("MenuLoad"), true),
                new MenuButton("Options", this, new SwitchMenuCommand("MenuOptions"), true),
                _quitToMenuButton,
                _quitButton
            };

            var widget = new VerticalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10,
            };

            _menuElements.AddRange(addButtonCommands);
            foreach (var elem in addButtonCommands)
            {
                elem.Click += (s, a) => OnBtnClick(elem, a);
                widget.Widgets.Add(elem.Widget);
            }

            SwitchToGameMenuVariant();

            return widget;
        }

        private void OnBtnClick(object? sender, EventArgs ea)
        {
            if (sender is not MenuButton btn)
            {
                return;
            }
            var command = btn.GetCommand();
            if(command == null)
            {
                return;
            }
            EnqueueCommand(command);
        }

        protected override void Init(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        protected override void OnEnter(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        protected override void OnExit(ContentManager contentManager, IServiceProvider serviceProvider)
        {
        }

        public void SwitchToMainMenuVariant()
        {
            _resumeButton.Widget.Visible = false;
            _resumeButton.Widget.Enabled = false;
            _newGameButton.Widget.Visible = true;
            _newGameButton.Widget.Enabled = true;
            _quitToMenuButton.Widget.Visible = false;
            _quitToMenuButton.Widget.Enabled = false;
            _quitButton.Widget.Visible = true;
            _quitButton.Widget.Enabled = true;
        }

        public void SwitchToGameMenuVariant()
        {
            _resumeButton.Widget.Visible = true;
            _resumeButton.Widget.Enabled = true;
            _newGameButton.Widget.Visible = false;
            _newGameButton.Widget.Enabled = false;
            _quitToMenuButton.Widget.Visible = true;
            _quitToMenuButton.Widget.Enabled = true;
            _quitButton.Widget.Visible = false;
            _quitButton.Widget.Enabled = false;
        }
    }
}