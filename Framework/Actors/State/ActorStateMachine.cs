using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Actors
{
    public class ActorStateMachine
    {
        public ActorStateMachine(Actor actor, StateController stateController, StateMachineType stateMachineType)
        {
            StateMachineType = stateMachineType;
            Actor = actor;
            StateController = stateController;
            State = new None { StateMachine = this };
            State.Init();
        }

        public StateMachineType StateMachineType { get; }
        /// <summary>
        /// The Actor using the StateMachine
        /// </summary>
        /// <value></value>
        public Actor Actor { get; set; }
        public StateController StateController { get; set; }
        /// <summary>
        /// State to return to as a starting point.
        /// Helpful for when switching weapons.
        /// </summary>
        /// <value></value>
        public ActorState InitialState { get; set; }
        /// <summary>
        /// The current State.
        /// </summary>
        /// <value></value>
        public ActorState State { get; set; }

        /// <summary>
        /// To be called in the Actor's Process or PhysicsProcess method.
        /// </summary>
        public void Update(float delta)
        {
            var state = State.Update(delta);
            if (state != null)
                TransitionTo(state);
        }

        /// <summary>
        /// Switches the current state.
        /// Calls Exit of previous State and enter of new State.
        /// </summary>
        /// <param name="newState"></param>
        public void TransitionTo(ActorState newState)
        {
            State.Exit();
            State = newState;
            State.StateMachine = this;
            State.Init();
            State.Enter();
        }

        public void Reset()
        {
            TransitionTo(InitialState);
        }

        /// <summary>
        /// Sets and starts InitialState.
        /// </summary>
        /// <param name="initialState"></param>
        public void Init(ActorState initialState)
        {
            InitialState = initialState;
            Reset();
        }

        public bool PlayAnimation(ActorState state, string animationName, bool force = false)
        {
            return StateController.PlayAnimation(this, state, animationName, force);
        }
    }
}