using model.Models.Input;

namespace model.Interfaces.Service
{
    public interface IInputManagerService
    {
        /// Should return array of all user actions
        InputDataModel Actions { get; }
        void ProcessActions();
    }
}