using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model.Interfaces;
using model.Interfaces.Ui;
using model.Models.Ui;

namespace scenes.Menus.Commands
{
    public class SwitchMenuCommand : MenuCommand
    {
        private readonly string _menuName;

        public SwitchMenuCommand(string menuName) : base($"Switch to menu {menuName}")
        {
            _menuName = menuName;
        }

        public string MenuName => _menuName;
    }
}