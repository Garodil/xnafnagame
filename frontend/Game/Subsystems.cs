using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

namespace frontend.Game 
{
    public static class Subsystems {
        public static Desktop InitMyra(Microsoft.Xna.Framework.Game game, Func<MouseInfo> mouseInfoGetter) {
            MyraEnvironment.Game = game;
            MyraEnvironment.MouseInfoGetter = mouseInfoGetter;
            game.IsMouseVisible = true;
            game.Services.AddService(typeof(Desktop), new Desktop());
            Desktop desktop = game.Services.GetService(typeof(Desktop)) as Desktop ?? throw new Exception();
            desktop.Root = new Panel();
            return desktop;
        }

        
    }
}