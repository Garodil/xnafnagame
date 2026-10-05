using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Models.Ui.Commands
{
    public class ShowMenuCommand : MenuCommand
    {
        public ShowMenuCommand() : base($"Show menu")
        {
        }
    }
}