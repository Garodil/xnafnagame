using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Models.Input;
using Microsoft.Xna.Framework.Input;


namespace model.Models.Configs
{
    public class KeyboardOptions
    {
        public Dictionary<InputActionsEnum, List<Keys>> KeyboardConfig{get;private set;} //TODO change from int to Keys
        public KeyboardOptions(
            Dictionary<InputActionsEnum, List<Keys>> keyboardConf // TODO 
        )
        {
            KeyboardConfig = keyboardConf;
        }
        public void ChangeOptions(
            Dictionary<InputActionsEnum, List<Keys>> keyboardConf // TODO 
        )
        {
            KeyboardConfig = keyboardConf;
        }
    }
}