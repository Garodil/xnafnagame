using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace model.Models.Configs
{
    public class GameOptions
    {
        public VideoOptions VideoOptions{get; set;}
        public AudioOptions AudioOptions{get; set;}
        public KeyboardOptions KeyboardOptions{get; set;}
        public GameOptions(VideoOptions videoOpt, AudioOptions audioOpt,KeyboardOptions keyboardOpt)
        {
            VideoOptions = videoOpt;
            AudioOptions = audioOpt;
            KeyboardOptions = keyboardOpt;

        }

        public GameOptions(){}
        public void SetVideoOptions(VideoOptions videoOpt)
        {
            VideoOptions = videoOpt;
        }
        public void SetAudioOptions(AudioOptions audioOpt)
        {
            AudioOptions = audioOpt;
        }
        public void SetKeyboardOptions(KeyboardOptions keyboardOpt)
        {
            KeyboardOptions = keyboardOpt;
        }

    }
}