using System;
using Arenbee.Statistics;
using GameCore.Input;
using GameCore.Utility;

namespace Arenbee.Actors;

public abstract class ActorBodyState : State
{
    protected ActorBodyState(ActorBody actorBody)
    {
        ActorBody = actorBody;
    }

    public string AnimationName { get; protected set; } = string.Empty;
    public BlockedState BlockedStates { get; set; }
    public ActorInputHandler InputHandler => ActorBody.InputHandler;
    public StateController StateController => (StateController)ActorBody.StateController;
    public Stats Stats => (Stats)ActorBody.Actor.Stats;
    protected ActorBody ActorBody { get; }

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
