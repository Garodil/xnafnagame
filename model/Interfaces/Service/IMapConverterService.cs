using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using model.Models.Game;
using model.Models.Game.Tilemap;

namespace model.Interfaces.Service
{
    public interface IMapConverterService
    {
        public LevelScheme GetLevelScheme(string worldRelPath, int levelIndex, ContentManager contentManager);

        int GetLevels(string worldRelPath);
    }
}