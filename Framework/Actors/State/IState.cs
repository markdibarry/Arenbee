using Arenbee.Framework.Actors;

namespace Arenbee.Framework
{
    public interface IState
    {
        IStateMachine StateMachine { get; set; }
        StateController StateController { get; set; }
        bool IsInitialState { get; set; }
        void Init();
        void Update();
        void Enter();
        void Exit();
        void CheckForTransitions();
    }
}
