using System.Windows.Input;
using Microsoft.Xna.Framework;
using model.Interfaces.Ui;
using model.Models.Input;
using model.Models.Ui;

namespace model.Interfaces.Service
{
    public interface IMenuManager
    {
        public void PushCommand(MenuCommand command);
        public void Update(GameTime gt, InputDataModel inputs);
        public bool IsMenuEnabled();
        public void SwapMenu(IMenu? menu);
        public void Render(GameTime gt);
    }
}