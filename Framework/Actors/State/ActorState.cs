using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Actors
{
    public abstract class ActorState<TState, TStateMachine> : State<TState, TStateMachine>
        where TState : ActorState<TState, TStateMachine>
        where TStateMachine : ActorStateMachine<TState, TStateMachine>
    {
        public string AnimationName { get; protected set; }
        public BlockableState[] BlockedStates { get; set; }
        public ActorInputHandler InputHandler => Actor.InputHandler;
        public StateController StateController => Actor.StateController;
        protected Actor Actor { get; private set; }

        public override void Init(TStateMachine stateMachine)
        {
            base.Init(stateMachine);
            Actor = stateMachine.Actor;
            BlockedStates ??= new BlockableState[0];
        }

        protected abstract void PlayAnimation(string animationName);
    }

    public enum BlockableState
    {
        Move,
        Jumping,
        Attack
    }
}