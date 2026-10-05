using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using model.Interfaces.Repository;
using model.Interfaces.Service;
using model.Interfaces.Ui;
using model.Models.Input;
using model.Models.Ui;
using model.Models.Ui.Commands;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using scenes.Menus;
using scenes.Menus.Commands;
using scenes.Menus.MenuInstances;
using model.Models.Configs;

namespace scenes.Manager 
{
    public class MenuManager : IMenuManager 
    {
        private GameOptions _config;
        private IConfigManager _configManager;
        private readonly IServiceProvider _services;
        private readonly ContentManager _content;
        private readonly ISceneManagerRepository _sceneRepository;
        private readonly ISceneManager _sceneManager;
        private readonly IMenu _menuTreeRoot;
        private readonly WeakReference<Game> _game;
        private IMenu _currentMenu;
        private readonly Desktop _desktop;

        private readonly Queue<MenuCommand> _commandQueue = new Queue<MenuCommand>();
        private readonly MainMenu _mainMenu;

        public MenuManager(IServiceProvider services, ContentManager content, Desktop desktop, Game game, GameOptions gameConfig, IConfigManager manager)
        {
            _config = gameConfig;
            _configManager = manager;
            _services = services;
            _content = content;
            _sceneRepository = services.GetService(typeof(ISceneManagerRepository)) as ISceneManagerRepository ?? throw new Exception();
            _sceneManager = services.GetService(typeof(ISceneManager)) as ISceneManager ?? throw new Exception();
            _desktop = desktop;
            _game = new WeakReference<Game>(game);

            _mainMenu = new MainMenu(this);
            _currentMenu = _menuTreeRoot = GenerateMenuStructure(_mainMenu);
            InitBaseMenuLayout();
        }

        private void InitBaseMenuLayout()
        {
            
            if(!Stylesheet.Current.ButtonStyles.ContainsKey("mainMenuButton")) 
            {
                var mainMenuButtonStyle = new ButtonStyle
                {
                    BorderThickness = new Thickness(2),
                    Border = null,
                    Background = new SolidBrush(new Color(50, 50, 50)),
                    OverBackground = new SolidBrush(new Color(65, 65, 65)),
                    PressedBackground = new SolidBrush(new Color(80, 80, 80)),
                    Width = 100,
                    Height = 30,
                };
                Stylesheet.Current.ButtonStyles.Add("mainMenuButton", mainMenuButtonStyle);
            }
            var rootPanel = new Panel() {
                Background = new SolidBrush(Color.Black)
            };
            _desktop.Root = rootPanel;
        }


        private IMenu GenerateMenuStructure(MainMenu mainMenu)
        {
            var loadMenu = new MenuLoad(mainMenu, this);
            var optionsMenu = new MenuOptions(mainMenu, this, _config, _configManager);
            mainMenu.AddChild(loadMenu);
            mainMenu.AddChild(optionsMenu);
            return mainMenu;
        }


        private IMenu GetMenu() 
        {
            return _currentMenu ?? _menuTreeRoot;
        }

        /// <summary>
        /// exit current menu and enter the input menu if it is not null, else does nothing
        /// </summary>
        public void SwapMenu(IMenu? menu)
        {
            if(_desktop != null && menu != null)
            {
                _currentMenu?.Exit(_content, _services);
                var rootPanel = _desktop.Root as Container;
                rootPanel?.Widgets.Clear();
                rootPanel?.Widgets.Add(menu.GetWidget());
                _currentMenu = menu;
                _currentMenu?.Enter(_content, _services);
            }
        }
        
        public void Update(GameTime gt, InputDataModel inputs)
        {
            if(IsMenuEnabled())
            {
                ProcessInputs(gt, inputs);
            }
        
            while(_commandQueue.TryDequeue(out var command))
            {
                ExecuteCommand(command);
            }
        }

        private void ProcessInputs(GameTime gt, InputDataModel inputs)
        {
            // building actions
            inputs.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Up, out var up);
            inputs.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Down, out var down);
            inputs.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Left, out var left);
            inputs.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Escape, out var escape);
            inputs.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Enter, out var enter);
            inputs.MouseState.TryGetValue(MouseButtonEnum.LeftButton, out var lmb);

            // bool esc = false;
            // bool interact = false;
            int move = 0;
            if(escape == InputStateEnum.Pressed)
            {
                PushCommand(new UpMenuCommand());
            }
            if(enter == InputStateEnum.Pressed || lmb == InputStateEnum.Pressed)
            {
            }

            if(up == InputStateEnum.Pressed)
            {
                move -= 1;
            }
            if(down == InputStateEnum.Pressed)
            {
                move += 1;
            }

            var currentMenu = GetMenu();
            if (move > 0)
            {
                _desktop.FocusedKeyboardWidget = currentMenu.GetNextWidget();
            }
            if (move < 0)
            {
                _desktop.FocusedKeyboardWidget = currentMenu.GetPreviousWidget();
            }
        }

        private void ExecuteCommand(MenuCommand command)
        {
            if (command == null)
            {
                return;
            }

            //init command here
            switch(command)
            {
                case ShowMenuCommand showMenuCommand:
                    EnableMenu();
                    break;
                case HideMenuCommand hideMenuCommand:
                    DisableMenu();
                    break;
                case ShowMenuVariantCommand showMenuVariantCommand:
                    switch(showMenuVariantCommand.Type)
                    {
                        case MenuTypeEnum.MainMenu:
                            SwitchToMainMenu();
                            break;
                        case MenuTypeEnum.GameMenu:
                            SwitchToGameMenu();
                            break;
                    }
                    break;
                case SwitchMenuCommand switchMenu:
                    SwapMenu(_currentMenu?.GetChild(switchMenu.MenuName));
                    break;
                case SwitchSceneCommand switchScene:
                    _sceneManager.ChangeScene(_sceneRepository.GetScene(switchScene.SceneName) ?? throw new Exception());
                    break;
                case UpMenuCommand _:
                    var parent = _currentMenu?.GetParent();
                    if(parent == null)
                    {
                        DisableMenu();
                    }
                    else
                    {
                        SwapMenu(parent);
                    }
                    break;
                case ExitCommand _:
                    if(_game.TryGetTarget(out var game))
                    {
                        game.Exit();
                    }
                    break;
            }
        }

        private void EnableMenu()
        {
            SwapMenu(_menuTreeRoot);
            _desktop.Root.Visible = true;
            _desktop.Root.Enabled = true;
        }

        private void DisableMenu()
        {
            SwapMenu(_menuTreeRoot);
            _menuTreeRoot.Exit(_content, _services);
            _desktop.Root.Visible = false;
            _desktop.Root.Enabled = false;
        }

        public void Render(GameTime gt)
        {
            _desktop.Render();
        }

        public void PushCommand(MenuCommand command)
        {
            _commandQueue.Enqueue(command);
        }

        public bool IsMenuEnabled()
        {
            return _desktop.Root.Visible && _desktop.Root.Enabled;
        }

        private void SwitchToMainMenu()
        {
            _mainMenu.SwitchToMainMenuVariant();
        }

        private void SwitchToGameMenu()
        {
            _mainMenu.SwitchToGameMenuVariant();
        }
    }
}