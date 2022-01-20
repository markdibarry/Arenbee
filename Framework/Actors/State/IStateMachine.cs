using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors
{
    public interface IStateMachine
    {
        StateMachineType StateMachineType { get; }
        Actor Actor { get; set; }
        StateController StateController { get; set; }
        IState InitialState { get; set; }
        IState State { get; set; }
        void Update(float delta);
        void TransitionTo(IState newState);
        void Init(IState initialState);
        void PlayAnimation(string animationName);
    }
}