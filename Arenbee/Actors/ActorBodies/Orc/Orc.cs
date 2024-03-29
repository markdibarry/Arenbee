﻿using Arenbee.Actors.Behavior;
using Arenbee.Actors.State.DefaultEnemy;
using Arenbee.Statistics;
using GameCore.Utility;

namespace Arenbee.Actors.ActorBodies;

public partial class Orc : ActorBody
{
    public Orc() : base()
    {
        StateController = new StateController(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            new ActionStateMachine(this),
            (actor) => new PatrolChaseGroundBT(actor));
    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void SetHitBoxes()
    {
        var bodybox = HitBoxes.GetNode<HitBox>("BodyBox");
        bodybox.SetBasicMeleeBox(this);
    }
}
