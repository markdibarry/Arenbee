using Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;
using Arenbee.Actors.Enemies.Default.State;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Statistics;

namespace Arenbee.Actors.Enemies;

public partial class Whisp : ActorBody
{
    public Whisp()
    {
        IsFloater = true;
        StateController = new AStateController(
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
