using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Extensions;
using Arenbee.Actors.Enemies.Default.State;

namespace Arenbee.Actors.Enemies;

public partial class Plant : Actor
{
    public Plant()
    {
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
        Stats.AddMod(new Modifier(StatType.ElementOff, (int)ElementType.Earth, ModOperator.Add, 1));
        Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Fire, ModOperator.Add, ElementDef.Weak));
        Stats.SetAttribute(AttributeType.MaxHP, 4);
        Stats.SetAttribute(AttributeType.HP, 4);
        Stats.SetAttribute(AttributeType.Attack, 4);
        Stats.SetAttribute(AttributeType.Defense, 0);
    }

    protected override void SetHitBoxes()
    {
        var headbox = HitBoxes.GetNode<HitBox>("HeadBox");
        headbox.SetBasicMeleeBox(this);
    }
}
