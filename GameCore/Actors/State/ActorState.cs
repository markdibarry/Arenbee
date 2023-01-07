using System;
using GameCore.Input;
using GameCore.Utility;

namespace GameCore.Actors;

public abstract class ActorState : State
{
    protected ActorState(ActorBase actor)
    {
        Actor = actor;
    }

    public string AnimationName { get; protected set; } = string.Empty;
    public BlockedState BlockedStates { get; set; }
    public ActorInputHandler InputHandler => Actor.InputHandler;
    public StateControllerBase StateController => Actor.StateController;
    protected ActorBase Actor { get; private set; }

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
