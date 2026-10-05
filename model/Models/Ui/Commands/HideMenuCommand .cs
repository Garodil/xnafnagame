using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Models.Ui.Commands
{
    public class HideMenuCommand : MenuCommand
    {
        public HideMenuCommand() : base($"Hide menu")
        {
        }
    }
}