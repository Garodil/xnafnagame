using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Structs;
using model.Models.Configs.Enum;

namespace model.Models.Configs
{
    public class VideoOptions
    {
        public Dictionary<ResolutionEnum, Size> ResolutionDictionary =  new Dictionary<ResolutionEnum, Size>()
        {
            {ResolutionEnum.r1920x1080,new Size(1080,1920) },
            {ResolutionEnum.r1280x720,new Size(720,1280) },
            {ResolutionEnum.r1366x768,new Size(768,1366) },
            {ResolutionEnum.r1024x768,new Size(768,1024) },
            {ResolutionEnum.r800x600,new Size(600,800) },
            {ResolutionEnum.r640x480,new Size(480,640) }
        };

        public ResolutionEnum CurrentResolutionIndex{get;private set;}
        public float RenderSubdivition{get;private set;}
        public VideoOptions(float sc, Size res)
        {
            var discoveredResolution = ResolutionDictionary.FirstOrDefault(
            x => x.Value.Equals(res), 
            ResolutionDictionary.First()
        );
            CurrentResolutionIndex = discoveredResolution.Key;
            RenderSubdivition = sc;
        }
        public void ChangeOptions(float sc, ResolutionEnum res)
        {
            CurrentResolutionIndex = res;
            RenderSubdivition = sc;
        }
    }
}