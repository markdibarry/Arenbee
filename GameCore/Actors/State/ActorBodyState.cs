using System;
using GameCore.Input;
using GameCore.Statistics;
using GameCore.Utility;

namespace GameCore.Actors;

public abstract class ActorBodyState : State
{
    protected ActorBodyState(AActorBody actorBody)
    {
        ActorBody = actorBody;
    }

    public string AnimationName { get; protected set; } = string.Empty;
    public BlockedState BlockedStates { get; set; }
    public ActorInputHandler InputHandler => ActorBody.InputHandler;
    public AStateController StateController => ActorBody.StateController;
    public Stats Stats => ActorBody.Actor.Stats;
    protected AActorBody ActorBody { get; }

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
