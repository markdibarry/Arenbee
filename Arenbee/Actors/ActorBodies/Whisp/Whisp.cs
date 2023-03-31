using Arenbee.Actors.Behavior.PatrolChaseAir;
using Arenbee.Actors.State.DefaultEnemy;
using Arenbee.Statistics;
using GameCore.Extensions;

namespace Arenbee.Actors.ActorBodies;

public partial class Whisp : ActorBody
{
    public Whisp()
    {
        IsFloater = true;
        StateController = new StateController(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            new ActionStateMachine(this),
            (actor) => new PatrolChaseAirBT(actor));
    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void SetHitBoxes()
    {
        var bodybox = HitBoxes.GetNode<HitBox>("BodyBox");
        bodybox.SetBasicMeleeBox(this);
    }
}
