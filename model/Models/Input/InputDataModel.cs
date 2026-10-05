using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;
using model.Interfaces.Service;
using model.Models.Input;

namespace model.Models.Input
{
    public class InputDataModel
    {
        
        public InputDataModel(
            Dictionary<Keys,InputStateEnum> keyboardState,
            Dictionary<MouseButtonEnum, InputStateEnum> mouseState,
            Dictionary<MouseEnum, int> mousePosition
            //GamePadState gamepadState
            )
        {
            KeyboardState = keyboardState;
            MouseState = mouseState;
            MousePosition = mousePosition;
        }

        public Dictionary<Keys,InputStateEnum> KeyboardState { private set; get; }
        public Dictionary<MouseButtonEnum, InputStateEnum> MouseState { private set; get; }
        public Dictionary<MouseEnum, int> MousePosition { private set; get; }
        public GamePadState GpState { private set; get; }


    }
}