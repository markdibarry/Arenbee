using System;
using System.Collections.Generic;

namespace GameCore.Utility
{
    public abstract class StateMachine<TState, TStateMachine>
        where TState : State<TState, TStateMachine>
        where TStateMachine : StateMachine<TState, TStateMachine>
    {
        /// <summary>
        /// The current State.
        /// </summary>
        /// <value></value>
        public TState State { get; set; }
        /// <summary>
        /// State to return to as fallback
        /// </summary>
        /// <value></value>
        public TState FallbackState { get; set; }
        /// <summary>
        /// Cache of the states
        /// </summary>
        /// <returns></returns>
        protected readonly Dictionary<Type, TState> States = new();

        public void AddState<T>() where T : TState, new()
        {
            // Find state in cache
            var type = typeof(T);
            var state = new T();
            States[type] = state;
            if (States.Count == 1)
            {
                State = state;
                FallbackState = state;
            }
        }

        public void ExitState()
        {
            State.Exit();
        }

        /// <summary>
        /// Gets state from cache or creates new one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TState GetState<T>() where T : TState, new()
        {
            // Find state in cache
            var type = typeof(T);
            if (States.TryGetValue(type, out TState newState))
                return newState;
            return null;
        }

        protected virtual void InitStates(TStateMachine stateMachine)
        {
            foreach (var statePair in States)
                statePair.Value.Init(stateMachine);
        }

        public void Init()
        {
            TransitionTo(FallbackState, null);
        }

        /// <summary>
        /// Switches the current state.
        /// Calls Exit of previous State and enter of new State.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void TransitionTo<T>(object[] args = null) where T : TState, new()
        {
            TransitionTo(GetState<T>(), args);
        }

        /// <summary>
        /// Updates the State.
        /// </summary>
        public void Update(double delta)
        {
            var state = State.Update(delta);
            if (state != null)
                TransitionTo(state, null);
        }

        /// <summary>
        /// Switches the current state.
        /// Calls Exit of previous State and enter of new State.
        /// </summary>
        /// <param name="newState"></param>
        private void TransitionTo(TState newState, object[] args)
        {
            State.Exit();
            State = newState;
            State.Enter(args);
        }
    }
}