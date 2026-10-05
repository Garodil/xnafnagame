using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using model.Interfaces.Repository;
using model.Interfaces.Service;
namespace repository
{
    public class InputManagerRepository : IInputManagerRepository
    {
        
        public KeyboardState GetKeyboardState()
        {
            return Keyboard.GetState();
        }
        public MouseState GetMouseState()
        {
            return Mouse.GetState();
        }
        public GamePadState GetGamePadState()
        {
            return GamePad.GetState(PlayerIndex.One);
        }
    }
}