using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Extensions;
using Arenbee.Actors.Enemies.Default.State;

namespace Arenbee.Actors.Enemies;

public partial class Plant : ActorBody
{
    public Plant()
    {
        StateController = new AStateController(
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
