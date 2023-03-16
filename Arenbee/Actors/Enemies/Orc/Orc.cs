using Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseGround;
using Arenbee.Actors.Enemies.Default.State;
using Arenbee.Statistics;
using GameCore.Extensions;

namespace Arenbee.Actors.Enemies;

public partial class Orc : ActorBody
{
    public Orc()
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
