using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Models.Sound
{
    public abstract class Audible
    {
        public bool IsEnabled { set; get; }
        public virtual bool IsPlaying { get; }
        private float _internalVolume = 1.0f;
        public int Order { set; get; }

        public float InternalVolume 
        { 
            set 
            { 
                _internalVolume = Math.Clamp(value, 0.0f, 1.0f);
            }
            get 
            {
                return _internalVolume;
            }
        }

        public void Play(float volume = 1.0f)
        {
            if(IsEnabled == false)
            {
                return;
            }
            volume = Math.Clamp(volume, 0.0f, 1.0f);
            PlayInternal(volume * InternalVolume);
        }

        public void Stop()
        {
            if(IsPlaying == true && IsEnabled == false)
            {
                StopInternal();
            }
        }

        public void Pause()
        {
            if(IsPlaying == true && IsEnabled == false)
            {
                PauseInternal();
            }
        }

        protected abstract void PlayInternal(float volume);
        protected abstract void StopInternal();
        protected abstract void PauseInternal();
    } 
}