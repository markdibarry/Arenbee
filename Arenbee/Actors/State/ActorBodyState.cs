using System;
using Arenbee.Input;
using Arenbee.Statistics;
using GameCore.Actors;

namespace Arenbee.Actors;

public abstract class ActorBodyState : IState
{
    protected ActorBodyState(ActorBody actorBody)
    {
        ActorBody = actorBody;
    }

    public string AnimationName { get; protected set; } = string.Empty;
    public BlockedState BlockedStates { get; set; }
    public IActorInputHandler InputHandler => ActorBody.InputHandler;
    public StateController StateController => (StateController)ActorBody.StateController;
    public Stats? Stats => ActorBody.Actor?.Stats;
    protected ActorBody ActorBody { get; }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual bool TrySwitch(IStateMachine stateMachine) => false;

    public virtual void Update(double delta) { }

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
