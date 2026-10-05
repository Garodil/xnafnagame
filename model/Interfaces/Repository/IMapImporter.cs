using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ldtk;
using Microsoft.Xna.Framework.Graphics;

namespace model.Interfaces.Repository
{
    public interface IMapImporter
    {
        LdtkJson LoadWorld(string mapPath);
    }
}