using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Configs;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra;

namespace model.Interfaces.Service
{
    public interface IConfigManager
    {
        void ConnectGDM(GraphicsDeviceManager GraphicsDeviceManager);
        void JsonSerialize(GameOptions options, string path = "../frontend//Config/core.json");
        GameOptions JsonDeserialize(string path = "../frontend//Config/core.json");
        void ChangeConfig(GameOptions oldOpt, GameOptions newOpt);
        GameOptions ConvertToGameOptions(TemporalOptions tmp);
        TemporalOptions ConvertToTemporalOptions(GameOptions options);
    }
}