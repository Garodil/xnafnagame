using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Configs.Enum;


namespace model.Models.Configs
{
    public class AudioOptions
    {
        public Dictionary<SoundChannelsEnum, float> ChanelsVolume{get;private set;}
        public AudioOptions(Dictionary<SoundChannelsEnum, float> chanelsVol)
        {
            ChanelsVolume = chanelsVol;
        }
        public void ChangeOptions(Dictionary<SoundChannelsEnum, float> chanelsVol)
        {
            ChanelsVolume = chanelsVol;
        }

    }
}