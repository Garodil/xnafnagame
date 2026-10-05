using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace model.Models.Sound
{
    public class Sound : Audible
    {
        private readonly SoundEffectInstance _track;
        public bool IsLooped { set => _track.IsLooped = value; get => _track.IsLooped; }
        public float Pan { set => _track.Pan = value; get => _track.Pan; }
        public float Pitch { set => _track.Pitch = value; get => _track.Pitch; }
        public override bool IsPlaying { get => _track.State == SoundState.Playing; }

        public Sound(SoundEffectInstance track)
        {
            _track = track;
        }

        protected override void PlayInternal(float volume)
        {
            _track.Volume = volume;
            _track.Play();
        }

        protected override void StopInternal()
        {
            _track.Stop();
        }

        protected override void PauseInternal()
        {
            _track.Pause();
        }
    }
}