using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Configs;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using repository.Resource.Graphics;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra;
using model.Interfaces.Service;
using logic.Component;
using model.Models.Render;
using model.Models.Configs.Enum;
using service;
using Microsoft.Xna.Framework.Graphics;
using logic.Component;


namespace service.Config
{
    public class ConfigManagerService:IConfigManager
    {
        private SoundPlayerComponent _soundPlayerComponent; //TODO убрать к чертям все эти подключения
        private GraphicsDeviceManager _gdm;                   //надо сделать, чтобы рендер и звук ссылались на конфиг
        private readonly int _pixelization = 2;             //а менеджер менял только актуальный конфиг
        private IServiceProvider _serviceProvider;          //сделать через event или типо того, чтобы при изменении конфига
        private IRenderTargetService _renderTargetService;  //в подключенных компонентах менялись соответствующие переменные
        public void ConnectProvider(IServiceProvider serviceProvider) //Это я для себя больше пишу - Morgot
        {
            _serviceProvider = serviceProvider;
            var type = typeof(IRenderTargetService);
            var render =  _serviceProvider.GetService(type) as IRenderTargetService;
            _renderTargetService = render;
        
        }
        public void ConnectSound(SoundPlayerComponent spc)
        {
            _soundPlayerComponent = spc;
        }
          
        public void ConnectGDM(GraphicsDeviceManager GraphicsDeviceManager)
        {
            _gdm = GraphicsDeviceManager;
        }
        public ConfigManagerService(){}
        public void JsonSerialize(GameOptions options, string path = "../frontend//Config/core.json")
        {
            var data = ConvertToTemporalOptions(options);
            string jsonString = JsonSerializer.Serialize(data);
            Console.WriteLine("Json Serializer serialize new config:");
            Console.WriteLine(jsonString);
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(jsonString);
            }
        }
        public GameOptions JsonDeserialize(string path = "../frontend//Config/core.json")
        {
            string json;
            using (StreamReader reader = new StreamReader(path))

                {
                json = reader.ReadToEnd();
                }
                Console.WriteLine(json);
                return ConvertToGameOptions(JsonSerializer.Deserialize<TemporalOptions>(json));
        }
        public void ChangeConfig(GameOptions oldOpt, GameOptions newOpt)
        {
            ChangeVideoOpt(newOpt.VideoOptions);
            ChangeSoundOpt(newOpt.AudioOptions);
            //а комментариях второй вариант через замену объектов
            oldOpt.AudioOptions.ChangeOptions(newOpt.AudioOptions.ChanelsVolume);
            //oldOpt.SetAudioOptions(newOpt.AudioOptions);
            oldOpt.VideoOptions.ChangeOptions(newOpt.VideoOptions.RenderSubdivition, newOpt.VideoOptions.CurrentResolutionIndex);
            //oldOpt.SetVideoOptions(newOpt.VideoOptions);
            oldOpt.KeyboardOptions.ChangeOptions(newOpt.KeyboardOptions.KeyboardConfig) ;
            //oldOpt.SetKeyboardOptions(newOpt.KeyboardOptions);
            JsonSerialize(newOpt);
        }
        private void ChangeVideoOpt(VideoOptions videoptons)
        {
            _gdm.PreferredBackBufferWidth = videoptons.ResolutionDictionary[videoptons.CurrentResolutionIndex].Width;
            _gdm.PreferredBackBufferHeight = videoptons.ResolutionDictionary[videoptons.CurrentResolutionIndex].Height;
            _gdm.IsFullScreen = false;
            _gdm.SynchronizeWithVerticalRetrace = true;
            _gdm.ApplyChanges();
            _renderTargetService.DisposeAllRenderTargets();
            var gds = _serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            if(gds == null)
            {
                throw new ArgumentNullException("Graphics device service not known");
            }
            if(gds.GraphicsDevice != null)
            {
                foreach(var targerName in Enum.GetValues(typeof(RenderTargetEnum)).Cast<RenderTargetEnum>())
                {
                    _renderTargetService.InitRenderTarget(targerName, _pixelization, gds.GraphicsDevice);
                }
            }
            else
            {
                throw new ArgumentNullException("Graphics device in gds not found");
            }
        }
        private void ChangeSoundOpt(AudioOptions audio)
        {
            _soundPlayerComponent.SetVolumeSettings(audio.ChanelsVolume);
        }

        public TemporalOptions ConvertToTemporalOptions(GameOptions options)
        {
            TemporalOptions tmp = new TemporalOptions();
            tmp.Resolution = options.VideoOptions.ResolutionDictionary[options.VideoOptions.CurrentResolutionIndex];
            tmp.RenderSubdivition = options.VideoOptions.RenderSubdivition;
            tmp.KeyboardConfig = options.KeyboardOptions.KeyboardConfig;
            tmp.ChanelsVolume = options.AudioOptions.ChanelsVolume;
            return tmp;
        }
        public GameOptions ConvertToGameOptions(TemporalOptions tmp)
        {
            AudioOptions aOpt = new AudioOptions(tmp.ChanelsVolume);
            KeyboardOptions kOPt = new KeyboardOptions(tmp.KeyboardConfig);
            VideoOptions vOpt = new VideoOptions(tmp.RenderSubdivition, tmp.Resolution);
            var opt = new GameOptions(vOpt,aOpt, kOPt);
            return opt;
        }
    }
}