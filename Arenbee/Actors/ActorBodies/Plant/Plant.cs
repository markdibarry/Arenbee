using Arenbee.Actors.State.DefaultEnemy;
using Arenbee.Statistics;
using GameCore.Extensions;

namespace Arenbee.Actors.ActorBodies;

public partial class Plant : ActorBody
{
    public Plant()
    {
        StateController = new StateController(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            new ActionStateMachine(this));
    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void SetHitBoxes()
    {
        var headbox = HitBoxes.GetNode<HitBox>("HeadBox");
        headbox.SetBasicMeleeBox(this);
    }
}
