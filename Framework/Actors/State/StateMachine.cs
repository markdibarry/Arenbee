using System;
using System.Collections.Generic;
using Godot;

namespace Arenbee.Framework.Actors
{
    public class StateMachine : IStateMachine
    {
        public StateMachine(Actor actor, StateController stateController)
        {
            Actor = actor;
            StateController = stateController;
            State = new None { StateMachine = this };
            State.Init();
        }

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
        public IState InitialState { get; set; }
        /// <summary>
        /// The current State.
        /// </summary>
        /// <value></value>
        public IState State { get; set; }

        /// <summary>
        /// To be called in the Actor's Process or PhysicsProcess method.
        /// </summary>
        public void Update()
        {
            State.Update();
        }

        /// <summary>
        /// Switches the current state.
        /// Calls Exit of previous State and enter of new State.
        /// </summary>
        /// <param name="newState"></param>
        public void TransitionTo(IState newState)
        {
            State.Exit();
            State = newState;
            State.StateMachine = this;
            State.Init();
            State.Enter();
        }

        /// <summary>
        /// Sets and starts InitialState.
        /// </summary>
        /// <param name="initialState"></param>
        public void Init(IState initialState)
        {
            InitialState = initialState;
            TransitionTo(initialState);
        }
    }
}