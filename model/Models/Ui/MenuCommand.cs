using Microsoft.Xna.Framework.Graphics;

namespace model.Models.Ui
{
    public abstract class MenuCommand
    {
        private readonly string _commandName;

        protected MenuCommand(string commandName)
        {
            _commandName = commandName;
        }

        public string CommandName => _commandName;
    }
}