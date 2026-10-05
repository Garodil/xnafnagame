using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using model.Interfaces.Service;
using model.Interfaces.Sound;
using model.Models.Configs.Enum;

namespace logic.Component
{
    public class SoundPlayerComponent : GameComponent, IDisposable
    {
        private ISceneManager _sceneManager;
        private IMusicQueueService _musicQueueService;

        private WeakReference<Dictionary<SoundChannelsEnum, float>> _channelsVolume;

        private bool _isMuted = false;
        private bool _isTrackLooping = false;

        private static Stopwatch _currentSongTimer = new Stopwatch();

        public SoundPlayerComponent(Game game, Dictionary<SoundChannelsEnum, float> ch) : base(game)
        {
            _channelsVolume = new WeakReference<Dictionary<SoundChannelsEnum, float>>(ch);
        }
        public void SetVolumeSettings(Dictionary<SoundChannelsEnum, float> sc)
        {
            _channelsVolume = new WeakReference<Dictionary<SoundChannelsEnum, float>>(sc);
        }

        public override void Initialize()
        {
            _sceneManager = Game.Services.GetService(typeof(ISceneManager)) as ISceneManager ?? throw new Exception();
            _musicQueueService = Game.Services.GetService(typeof(IMusicQueueService)) as IMusicQueueService ?? throw new Exception(); 
            _musicQueueService.SetWrap(true);
        }

        public override void Update(GameTime gameTime)
        {
            //var scene = _sceneManager.GetScene();
            PlaySounds(gameTime);
            PlayMusic(gameTime);
        }

        private void PlaySounds(GameTime gameTime)
        {
            float sound_val = 0f;
            if(_channelsVolume.TryGetTarget(out var dict))
            {
                sound_val = dict[SoundChannelsEnum.Main] * dict[SoundChannelsEnum.Sound];
            }
            else
            {
                Console.WriteLine("channelsVolume not found");
            }
            var currentScene = _sceneManager.GetScene();
            var state = currentScene.GetState();
            var objectSounds = state.SoundEmitters.SelectMany(x => x.GetSounds());
            var ambientSounds = state.Ambient.Where(x => x.IsEnabled);
            var allSounds = objectSounds.Concat(ambientSounds);
            foreach(var sound in allSounds.OrderBy(x => x.Order))
            {
                if(sound.IsEnabled == false && sound.IsPlaying)
                {
                    sound.Stop();
                }
                if(sound.IsEnabled && sound.IsPlaying == false)
                {
                    //sound.Play(_globalVolumeSetting);
                    sound.Play(sound_val);
                }
            }
        }

        private void PlaySong()
        {
            var currentSong = _musicQueueService.GetSelectedSong();
            if(currentSong == null)
            {
                return; //nothing to play
            }
            MediaPlayer.IsRepeating = _isTrackLooping;
            MediaPlayer.Play(currentSong);
        }

        private void NextSong()
        {
            var currentIndex = _musicQueueService.GetSelectedSongIndex();
            _musicQueueService.SetSelectedSong(currentIndex + 1);
        }

        private void PlayMusic(GameTime gameTime)
        {
            float music = 0f;
            if(_channelsVolume.TryGetTarget(out var dict))
            {
                music = dict[SoundChannelsEnum.Main] * dict[SoundChannelsEnum.Music];
            }
            else
            {
                Console.WriteLine("channel volume not found");
            }
            if(MediaPlayer.IsMuted != _isMuted)
            {
                MediaPlayer.IsMuted = _isMuted;
            }
            if(Math.Abs(MediaPlayer.Volume - music) > 1e-5) 
            {
                MediaPlayer.Volume = music;
            }
            if(MediaPlayer.State == MediaState.Stopped || MediaPlayer.State == MediaState.Paused)
            {
                NextSong();
                _currentSongTimer.Stop();
                PlaySong();
                _currentSongTimer.Start();
            }
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {

        }

        protected override void OnUpdateOrderChanged(object sender, EventArgs args)
        {
        }
    }
}