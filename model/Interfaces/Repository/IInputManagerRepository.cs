using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
namespace model.Interfaces.Repository
{
    public interface IInputManagerRepository
    {
        // Should return an array of keys pressed
        
        KeyboardState GetKeyboardState();
        MouseState GetMouseState();
        GamePadState GetGamePadState();
    }
}