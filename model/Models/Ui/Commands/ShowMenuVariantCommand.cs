using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model.Models.Ui.Commands
{
    public class ShowMenuVariantCommand : MenuCommand
    {
        private readonly MenuTypeEnum _type;
        public MenuTypeEnum Type => _type;
        public ShowMenuVariantCommand(MenuTypeEnum type) : base($"Show menu variant")
        {
            _type = type;
        }
    }
}