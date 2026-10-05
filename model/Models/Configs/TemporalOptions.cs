using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Structs;
using Microsoft.Xna.Framework.Input;
using model.Models.Configs.Enum;
using model.Models.Input;
//using System.Drawing;

namespace model.Models.Configs
{
    public class TemporalOptions
    {
        
        public Dictionary<string, Size> ResolutionArray;

        public Size Resolution{get;set;}
        public float RenderSubdivition{get;set;}
        public Dictionary<InputActionsEnum, List<Keys>> KeyboardConfig{get; set;}
        public Dictionary<SoundChannelsEnum, float> ChanelsVolume{get;set;}
        public TemporalOptions()
        {
            ResolutionArray = new Dictionary<string, Size>()
            {
                { "640x480", new Size(480, 640) },
                { "800x600", new Size(600, 800) },
                { "1024x768", new Size(768, 1024) },
                { "1366x768", new Size(768, 1366) },
                { "1280x720", new Size(720, 1280) },
                { "1920x1080", new Size(1080, 1920) }
            };
        }

    }
}