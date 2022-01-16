using Arenbee.Framework.Actors;

namespace Arenbee.Framework
{
    public interface IState
    {
        IStateMachine StateMachine { get; set; }
        StateController StateController { get; set; }
        bool IsInitialState { get; set; }
        string AnimationName { get; set; }
        void Init();
        void Update(float delta);
        void Enter();
        void Exit();
        void CheckForTransitions();
    }
}
