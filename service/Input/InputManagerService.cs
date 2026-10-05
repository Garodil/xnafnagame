using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using model.Interfaces.Service;
using model.Models.Input;
using model.Interfaces.Repository;
using logic.Component;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
namespace service
{
    public class InputManagerService : IInputManagerService
    {
        private readonly IInputManagerRepository _inputManagerRep;
        private KeyboardState _oldKeyboardState;
        private MouseState _oldMouseState;
        private GamePadState _oldGpState;
        public InputDataModel Actions { get; private set; }

        public InputManagerService(IInputManagerRepository inputManagerRepository)
        {
            _inputManagerRep = inputManagerRepository;
            _oldKeyboardState = _inputManagerRep.GetKeyboardState();
            _oldMouseState = _inputManagerRep.GetMouseState();
            _oldGpState = _inputManagerRep.GetGamePadState();

            Actions = new InputDataModel(
                new Dictionary<Keys, InputStateEnum>(),
                new Dictionary<MouseButtonEnum, InputStateEnum>(),
                new Dictionary<MouseEnum, int>()
            );
        }

        private static InputStateEnum KeyState(KeyboardState currentState,KeyboardState oldState,Keys key)
        {
            if (currentState.IsKeyDown(key) && oldState.IsKeyUp(key))
            {
                return InputStateEnum.Pressed;
            }
            else if (oldState.IsKeyUp(key))
            {
                return InputStateEnum.Up;
            }
            else if(currentState.IsKeyDown(key))
            {
                return InputStateEnum.Down;
            }
            else
            {
                return InputStateEnum.Unknown;
            }
        }

        private static InputStateEnum ButtonState(MouseState currentState, MouseState oldState, MouseButtonEnum button)
        {
            if (IsMouseDown(currentState, button) && !IsMouseDown(oldState, button)) 
            {
                return InputStateEnum.Pressed;
            }
            else if (!IsMouseDown(oldState, button)) 
            {
                return InputStateEnum.Up;
            }
            else if (IsMouseDown(currentState, button))
            {
                return InputStateEnum.Down;
            }
            else
            {
                return InputStateEnum.Unknown;
            }
        }

        private static bool IsMouseDown(MouseState currentState, MouseButtonEnum button) {
            return button switch
            {
                MouseButtonEnum.LeftButton => currentState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                MouseButtonEnum.RightButton => currentState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                MouseButtonEnum.MiddleButton => currentState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                MouseButtonEnum.xButton1 => currentState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                MouseButtonEnum.xButton2 => currentState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed,
                _ => false
            };
        }

        private static int MousePosition(MouseState currentState, MouseState oldState, MouseEnum value) { // TODO: отслеживать смещение
            return value switch
            {
                MouseEnum.X => currentState.X,
                MouseEnum.Y => currentState.Y,
                MouseEnum.ScrollWheelValue => currentState.ScrollWheelValue,
                _ => 0
            };
        }
        
        public void ProcessActions()
        {
            var mouseState = _inputManagerRep.GetMouseState();
            var gpState = _inputManagerRep.GetGamePadState();
            var keyboardState = _inputManagerRep.GetKeyboardState();
            var keysDict = new Dictionary<Keys, InputStateEnum>();
            foreach (var key in Keys.GetValues<Keys>())
            {
                var keyState = KeyState(keyboardState, _oldKeyboardState, key);
                keysDict[key] = keyState;
            }
            var mouseStateDict = new Dictionary<MouseButtonEnum, InputStateEnum>();
            foreach (var button in MouseButtonEnum.GetValues<MouseButtonEnum>())
            {
                var buttonState = ButtonState(mouseState, _oldMouseState, button);
                mouseStateDict[button] = buttonState;
            }
            var mousePositionDict = new Dictionary<MouseEnum, int>();
            foreach (var value in MouseEnum.GetValues<MouseEnum>())
            {
                var mousePosition = MousePosition(mouseState, _oldMouseState, value);
                mousePositionDict[value] = mousePosition;
            }

            var currentState = new InputDataModel(keysDict, mouseStateDict, mousePositionDict);
            _oldKeyboardState = keyboardState;
            _oldMouseState = mouseState;

            Actions = currentState;
        }
    }
}