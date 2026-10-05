using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using model.Interfaces.Service;
using model.Interfaces.Sound;
using model.Models.Entities;
using model.Models.Input;
using model.Models.Ui.Commands;
using scenes.Manager;

namespace scenes.Scenes
{
    public class VoidScene(string name) : Scene(name)
    {
        WeakReference<IMenuManager> _menuManager;

        protected override void LoadScene(ContentManager content, IServiceProvider services)
        {
            var musicTrack = content.Load<Song>("Music/s2-2");
            var state = GetState();

            var mapLoader = services.GetService(typeof(IMapConverterService)) as IMapConverterService ?? throw new Exception();

            var levelScheme = mapLoader.GetLevelScheme("test-tileset.ldtk", 0, content);
            state.AddTilemapLayer(levelScheme.TilemapLayers.ToArray());
            state.CameraManager.AddCameras(levelScheme.Cameras);
            state.CameraManager.TryChangeActiveCameraByIid(levelScheme.ActiveCameraIid);
            state.LevelBackground = levelScheme.LevelBackground;
            state.AddLights(levelScheme.Lights);

            var musicQueueService = services.GetService(typeof(IMusicQueueService)) as IMusicQueueService ?? throw new Exception();
            musicQueueService.AddMusic(musicTrack);

            var spawner = services.GetService(typeof(IGameSpawnService)) as IGameSpawnService ?? throw new Exception();
            spawner.AddFactory(levelScheme.Factories.ToArray());
            _menuManager = new WeakReference<IMenuManager>(services.GetService(typeof(IMenuManager)) as IMenuManager ?? throw new Exception());
        }

        public override void OnEnter(IServiceProvider services)
        {
            var spawner = services.GetService(typeof(IGameSpawnService)) as IGameSpawnService ?? throw new Exception();
            var state = GetState();
            spawner.SpawnEntity<Player>(state);
            spawner.SpawnEntity<Item>(state);

            state.CameraManager.StartFollowCameras(state);
            if (_menuManager.TryGetTarget(out var menuManager))
            {
                menuManager.PushCommand(new HideMenuCommand());
                menuManager.PushCommand(new ShowMenuVariantCommand(MenuTypeEnum.GameMenu));
            }
        }

        public override void OnExit(IServiceProvider services)
        {
            var spawner = services.GetService(typeof(IGameSpawnService)) as IGameSpawnService ?? throw new Exception();
            var state = GetState();
            spawner.DespawnEntity(state, state.Entities);
        }

        protected override void UnloadScene(ContentManager content, IServiceProvider services)
        {
            var state = GetState();
            state.CameraManager.ClearCameras();
            var spawner = services.GetService(typeof(IGameSpawnService)) as IGameSpawnService ?? throw new Exception();
            spawner.ClearFactories();
        }

        public override void ProcessInputs(GameTime gt, InputDataModel action)
        {
            var previousState = GetState();
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Up, out var upBtn);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Down, out var downBtn);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Left, out var left);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Right, out var right);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.Escape, out var escape);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.NumPad0, out var numZero);
            action.KeyboardState.TryGetValue(Microsoft.Xna.Framework.Input.Keys.NumPad1, out var numOne);

            if (escape == InputStateEnum.Pressed)
            {
                if (_menuManager.TryGetTarget(out var menuManager))
                {
                    if(menuManager.IsMenuEnabled() == false)
                    {
                        menuManager.PushCommand(new ShowMenuCommand());   
                    }
                }
            }

            if (upBtn == InputStateEnum.Down || upBtn == InputStateEnum.Pressed)
            {
                previousState.Player?.ApplyJump(1);
            }
            else if(downBtn == InputStateEnum.Down || downBtn == InputStateEnum.Pressed)
            {
                previousState.Player?.ApplyJump(-1);
            }
            else
            {
                previousState.Player?.ApplyJump(0);
            }

            if (left == InputStateEnum.Down || left == InputStateEnum.Pressed)
            {
                previousState.Player?.ApplyWalk(-1);
            }
            else if (right == InputStateEnum.Down || right == InputStateEnum.Pressed)
            {
                previousState.Player?.ApplyWalk(1);
            }
            else
            {
                previousState.Player?.ApplyWalk(0);
            }

            foreach (var updatable in previousState.Updatebles.OrderBy(x => x.UpdateOrder))
            {
                updatable.Update(gt);
            }
        }

        public override bool IsPaused()
        {
            return _menuManager.TryGetTarget(out var menuManager) && menuManager.IsMenuEnabled();
        }
    }
}