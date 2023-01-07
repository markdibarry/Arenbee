﻿using GameCore.Actors;
using GameCore.Items;

namespace Arenbee.Actors.Enemies.Default.State;

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(ActorBase actor)
        : base(
            new ActionState[]
            {
                new NotAttacking(actor, null)
            },
            actor, null)
    {
    }

    public class NotAttacking : ActionState
    {
        public NotAttacking(ActorBase actor, HoldItem? holdItem)
            : base(actor, holdItem)
        {
        }

        public override void Enter() { }

        public override void Update(double delta)
        {
        }

        public override void Exit() { }
    }
}
