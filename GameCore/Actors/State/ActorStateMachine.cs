using GameCore.Utility;

namespace GameCore.Actors
{
    public abstract class ActorStateMachine<TState, TStateMachine> : StateMachine<TState, TStateMachine>
        where TState : ActorState<TState, TStateMachine>
        where TStateMachine : ActorStateMachine<TState, TStateMachine>
    {
        protected ActorStateMachine(Actor actor)
        {
            Actor = actor;
        }

        /// <summary>
        /// The Actor using the StateMachine
        /// </summary>
        /// <value></value>
        public Actor Actor { get; set; }
        public StateController StateController => Actor.StateController;
    }
}