using Arenbee.Actors.Default.State;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Statistics;

namespace Arenbee.Actors.Players;

public partial class Ady : Actor
{
    public Ady()
    {
        ActorId = "Ady";
        WalkSpeed = 100;
        StateController = new StateControllerBase(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            new ActionStateMachine(this));
    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void ApplyDefaultStats()
    {
        Stats.SetAttribute(AttributeType.MaxHP, 12);
        Stats.SetAttribute(AttributeType.HP, 12);
        Stats.SetAttribute(AttributeType.Attack, 0);
        Stats.SetAttribute(AttributeType.Defense, 0);
    }
}
