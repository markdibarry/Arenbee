using Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseGround;
using Arenbee.Actors.Enemies.Default.State;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.Statistics;

namespace Arenbee.Actors.Enemies;

public partial class Ball : Actor
{
    public Ball()
    {
        StateController = new StateController(
            this,
            new MoveStateMachine(this),
            new AirStateMachine(this),
            new HealthStateMachine(this),
            (actor) => new ActionStateMachine(actor),
            (actor) => new PatrolChaseGroundBT(actor));
    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void ApplyDefaultStats()
    {
        Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Weak));
        Stats.AddMod(new Modifier(StatType.StatusEffectOff, (int)StatusEffectType.Poison, ModOperator.Add, 1, 100));
        Stats.SetAttribute(AttributeType.MaxHP, 6);
        Stats.SetAttribute(AttributeType.HP, 6);
        Stats.SetAttribute(AttributeType.Attack, 4);
        Stats.SetAttribute(AttributeType.Defense, 0);
    }

    protected override void SetHitBoxes()
    {
        var bodybox = HitBoxes.GetNode<HitBox>("BodyBox");
        bodybox.SetBasicMeleeBox(this);
    }
}
