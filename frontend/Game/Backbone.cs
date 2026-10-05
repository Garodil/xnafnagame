using logic.Component;
using Microsoft.Xna.Framework;
using model.Interfaces.Service;
using repository;
using repository.Resource.Graphics;
using Myra.Graphics2D.UI;
using scenes.Manager;
using scenes.Scenes;
using service;
using service.Map;
using model.Interfaces.Sound;
using service.Sound;
using service.Logic;
using model.Interfaces.Repository;
using Microsoft.Xna.Framework.Input;
//-----------------TEST-------------------------------------
using model.Models.Configs;
using service.Config;
using model.Models.Input;
using model.Models.Structs;
using model.Models.Configs.Enum;
//-------------------------------------------------------



namespace frontend.Game
{
    class Backbone: Microsoft.Xna.Framework.Game
    {
        private int _pixelization = 2;
        //private MouseManager _mouseManager;
        private GameOptions _options;
        private ConfigManagerService _configManagerService;

        public Backbone(GameOptions options, ConfigManagerService configManagerService)
        {
            if (options == null)
            {
                Console.WriteLine("WARNING! OPTIONS NOT FOUND");
                return; //TODO сделать обработчик критических ситуаций, можно через стандартное значение
            }

            _configManagerService = configManagerService;
            _options = options;
            var gdm = new GraphicsDeviceManager(this);
            _configManagerService.ConnectGDM(gdm);
            // Typically you would load a config here...
            gdm.PreferredBackBufferWidth = options.VideoOptions.ResolutionDictionary[options.VideoOptions.CurrentResolutionIndex].Width;
            gdm.PreferredBackBufferHeight = options.VideoOptions.ResolutionDictionary[options.VideoOptions.CurrentResolutionIndex].Height;
            gdm.IsFullScreen = false;
            gdm.SynchronizeWithVerticalRetrace = true;
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = "Resource";
            
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        
        protected override bool BeginDraw()
        {
            return base.BeginDraw();
        }

        protected override void EndDraw()
        {
            base.EndDraw();
        }

        protected override void BeginRun()
        {
            base.BeginRun();
        }

        protected override void EndRun()
        {
            base.EndRun();
        }

        public MouseInfo CustomMouseInfoGetter()
        {
            var state = Mouse.GetState();

            return new MouseInfo
            {
                Position = new Point(state.X / _pixelization, state.Y / _pixelization),
                IsLeftButtonDown = IsActive && state.LeftButton == ButtonState.Pressed,
                IsMiddleButtonDown = IsActive && state.MiddleButton == ButtonState.Pressed,
                IsRightButtonDown = IsActive && state.RightButton == ButtonState.Pressed,
                Wheel = state.ScrollWheelValue
            };
        }

        protected override void Initialize()
        {
            var inputManagerRepository = new InputManagerRepository();
            Services.AddService(typeof(IInputManagerService), new InputManagerService(inputManagerRepository));

            Services.AddService(typeof(IMapImporter), new MapImporter());
            Services.AddService(typeof(IMapGroupingService), new MapGroupingService());
            Services.AddService(typeof(IMapConverterService), new MapConverterService("Resource/Tiles", Services));
            Services.AddService(typeof(IFovService), new FovService());
            Services.AddService(typeof(IGameSpawnService), new GameSpawnService(Services));
            
            var sceneManager = new SceneManager(Services);
            var sceneRepository = new SceneManagerRepository(Services, Content);
            var sceneChanger = new SceneChanger(sceneManager, sceneRepository);
            Services.AddService(typeof(ISceneManager), sceneManager);
            Services.AddService(typeof(ISceneManagerRepository), sceneRepository);
            Services.AddService(typeof(ISceneChanger), sceneChanger);

            var desktop = Subsystems.InitMyra(this, CustomMouseInfoGetter);
            Services.AddService(typeof(IMenuManager), new MenuManager(Services, Content, desktop, this, _options, _configManagerService));
            Services.AddService(typeof(IMusicQueueService), new MusicQueueService());
            Services.AddService(typeof(IRenderTargetService), new RenderTargetService());
            var renderingComponent = new WorldRenderingComponent(Services, Content)
            {
                DrawOrder = 1,
            };
            var guiComponent = new MainMenuComponent(Services)
            {
                DrawOrder = 5,
                UpdateOrder = 1
            };
            var debugComponent = new DebugRenderingComponent(Services, Content, ThisAssembly.GetVersion())
            {
                DrawOrder = 3,
                UpdateOrder = 3
            };
            var finalizeRenderComponent = new FinalizeRenderingComponent(Services, Content)
            {
                DrawOrder = 4,
            };
            _configManagerService.ConnectProvider(Services);
            var logicComponent = new GameLogicComponent(Services)
            {
                UpdateOrder = 0
            };
            var menuComponent = new InputLogicComponent(Services)
            {
                UpdateOrder = 2
            };
            var soundComponent = new SoundPlayerComponent(this, _options.AudioOptions.ChanelsVolume);
            _configManagerService.ConnectSound(soundComponent);
            Components.Add(finalizeRenderComponent);
            Components.Add(renderingComponent);
            Components.Add(guiComponent);
            Components.Add(logicComponent);
            Components.Add(menuComponent);
            Components.Add(soundComponent);
            Components.Add(debugComponent);

            base.Initialize();

            /// should not be called at initialize phase
            sceneRepository.AddScene(new MainMenuScene("main"));
            sceneRepository.AddScene(new VoidScene("void"));
            sceneChanger.ChangeScene("main");
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
        }

        protected override bool ShowMissingRequirementMessage(Exception exception)
        {
            return base.ShowMissingRequirementMessage(exception);
        }
    }
}