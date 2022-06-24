using System;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Actors
{
    public abstract class ActorState<TState, TStateMachine> : State<TState, TStateMachine>
        where TState : ActorState<TState, TStateMachine>
        where TStateMachine : ActorStateMachine<TState, TStateMachine>
    {
        public string AnimationName { get; protected set; }
        public BlockedState BlockedStates { get; set; }
        public ActorInputHandler InputHandler => Actor.InputHandler;
        public StateController StateController => Actor.StateController;
        protected Actor Actor { get; private set; }

        public override void Init(TStateMachine stateMachine)
        {
            base.Init(stateMachine);
            Actor = stateMachine.Actor;
        }

        protected abstract void PlayAnimation(string animationName);
    }

    [Flags]
    public enum BlockedState
    {
        None = 0,
        Move = 1,
        Jumping = 2,
        Attack = 4
    }
}