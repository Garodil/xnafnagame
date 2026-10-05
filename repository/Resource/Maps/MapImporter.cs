using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ldtk;
using Microsoft.Xna.Framework;
using model.Exceptions;
using model.Interfaces.Repository;
using model.Interfaces.Service;

namespace repository.Resource.Graphics
{
    public class MapImporter : IMapImporter
    {
        public LdtkJson LoadWorld(string mapPath)
        {
            var textTask = GetFileText(mapPath);
            var ldtkJson = LdtkJson.FromJson(textTask);
            if(ldtkJson.ExternalLevels == true)
            {
                var allLevels = LoadAllLevels(
                        ldtkJson.Levels
                            .Select(level => Path.Combine(Path.GetDirectoryName(mapPath), level.ExternalRelPath))
                    )
                    .ToArray();
                ldtkJson.Levels = allLevels;
            }
            return ldtkJson;
        }

        private static IEnumerable<Level> LoadAllLevels(IEnumerable<string> levelPaths)
        {
            return levelPaths.Select(levelPath => LoadLevel(levelPath));
        }

        private static Level LoadLevel(string levelPath)
        {
            var levelJson = GetFileText(levelPath);
            var levelParsed = Newtonsoft.Json.JsonConvert.DeserializeObject<ldtk.Level>(levelJson);
            return levelParsed;    
        }

        private static string GetFileText(string path)
        {
            if(File.Exists(path) == false)
            {
                throw new MapException($"No map file found at {path}");
            }
            return File.ReadAllText(path);
        }
    }
}