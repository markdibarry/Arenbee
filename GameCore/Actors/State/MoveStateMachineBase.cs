﻿namespace GameCore.Actors;

public abstract class MoveStateMachineBase : ActorStateMachine<MoveState, MoveStateMachineBase>
{
    protected MoveStateMachineBase(Actor actor)
        : base(actor)
    { }
}