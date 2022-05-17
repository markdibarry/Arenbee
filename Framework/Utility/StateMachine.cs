using System;
using System.Collections.Generic;

namespace Arenbee.Framework.Utility
{
    public abstract class StateMachine<TState, TStateMachine>
        where TState : State<TState, TStateMachine>
        where TStateMachine : StateMachine<TState, TStateMachine>
    {
        /// <summary>
        /// Cache of the states
        /// </summary>
        /// <returns></returns>
        private readonly Dictionary<Type, TState> _stateCache = new();
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
        /// Clears the state cache.
        /// </summary>
        public void ClearCache()
        {
            _stateCache.Clear();
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
            if (_stateCache.TryGetValue(type, out TState newState))
                return newState;
            // If not in cache, init new one
            newState = new T() { StateMachine = (TStateMachine)this };
            newState.Init();
            if (_stateCache.Count == 0)
                FallbackState = newState;
            _stateCache[type] = newState;
            return newState;
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
        public void Update(float delta)
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