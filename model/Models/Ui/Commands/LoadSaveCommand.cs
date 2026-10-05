using System.Diagnostics;
using model.Interfaces;
using model.Models.Input;
using Myra.Graphics2D.UI;

namespace model.Models.Ui.Commands
{
    public class LoadSaveCommand : MenuCommand
    {
        private readonly string _saveFile;
        public LoadSaveCommand(string saveFile) : base($"Load save {saveFile}")
        {
            _saveFile = saveFile;
        }
        public string SaveFile => _saveFile;
    }
}