using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore.Utility;

public abstract class StateMachine<TState> : IStateMachine where TState : IState
{
    public StateMachine(TState[] states)
    {
        _states = StateMachine<TState>.ToStatesDictionary(states);
        FallbackState = states.First();
        State = FallbackState;
    }

    /// <summary>
    /// Cache of the states
    /// </summary>
    /// <returns></returns>
    private readonly Dictionary<Type, TState> _states = new();
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

    public void Reset() => SwitchTo(FallbackState);

    public void ExitState() => State.Exit();

    public bool TrySwitchTo<T>() where T : IState
    {
        if (!_states.TryGetValue(typeof(T), out TState? state))
            return false;
        SwitchTo(state);
        return true;
    }

    /// <summary>
    /// Updates the State.
    /// </summary>
    public void Update(double delta)
    {
        if (!State.TrySwitch(this))
            State.Update(delta);
    }

    /// <summary>
    /// Switches the current state.
    /// Calls Exit of previous State and enter of new State.
    /// </summary>
    /// <param name="newState"></param>
    private void SwitchTo(TState newState)
    {
        State.Exit();
        State = newState;
        State.Enter();
    }

    private static Dictionary<Type, TState> ToStatesDictionary(TState[] states)
    {
        if (states.Length == 0)
            throw new Exception();
        return states.ToDictionary(x => x.GetType(), x => x);
    }
}
