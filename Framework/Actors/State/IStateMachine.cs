using Arenbee.Framework.Input;

namespace Arenbee.Framework.Actors
{
    public interface IStateMachine
    {
        Actor Actor { get; set; }
        StateController StateController { get; set; }
        IState InitialState { get; set; }
        IState State { get; set; }
        bool InputDisabled { get; set; }
        void Update(float delta);
        void TransitionTo(IState newState);
        void Init(IState initialState);
        bool IsActionJustPressed(string action);
        bool IsActionPressed(string action);
    }
}