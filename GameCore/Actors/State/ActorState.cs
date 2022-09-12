using System;
using GameCore.Input;
using GameCore.Utility;

namespace GameCore.Actors;

public abstract class ActorState<TState, TStateMachine> : State<TState, TStateMachine>
    where TState : ActorState<TState, TStateMachine>
    where TStateMachine : ActorStateMachine<TState, TStateMachine>
{
    public string AnimationName { get; protected set; }
    public BlockedState BlockedStates { get; set; }
    public ActorInputHandler InputHandler => Actor.InputHandler;
    public StateControllerBase StateController => Actor.StateController;
    protected ActorBase Actor { get; private set; }

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
