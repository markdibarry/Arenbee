using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;

namespace Arenbee.Framework
{
    public abstract class ActorState : State<ActorState, ActorStateMachine>
    {
        public ActorStateType[] BlockedStates { get; set; }
        public bool IsActionBlocking { get; set; }
        public bool IsJumpBlocking { get; set; }
        public bool IsMoveBlocking { get; set; }
        public ActorInputHandler InputHandler => Actor.InputHandler;
        public string AnimationName { get; protected set; }
        public StateController StateController { get; set; }
        protected Actor Actor { get; private set; }

        public override void Init()
        {
            BlockedStates ??= new ActorStateType[0];
            Actor = StateMachine.Actor;
            StateController = StateMachine.StateController;
        }

        protected abstract void PlayAnimation(string animationName);
    }

    public enum ActorStateType
    {
        Move,
        Jump,
        Attack
    }
}