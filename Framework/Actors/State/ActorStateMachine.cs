using Arenbee.Framework.Utility;

namespace Arenbee.Framework.Actors
{
    public class ActorStateMachine : StateMachine<ActorState, ActorStateMachine>
    {
        public ActorStateMachine(Actor actor, StateController stateController)
        {
            State = new None();
            Actor = actor;
            StateController = stateController;
        }

        /// <summary>
        /// The Actor using the StateMachine
        /// </summary>
        /// <value></value>
        public Actor Actor { get; set; }
        public StateController StateController { get; set; }
    }

    public class None : ActorState
    {
        public override ActorState CheckForTransitions() => null;

        public override void Enter() { }

        public override void Exit() { }

        public override ActorState Update(float delta) => null;

        protected override void PlayAnimation(string animationName) { }
    }
}