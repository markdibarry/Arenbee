using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Extensions;
using Arenbee.Actors.Enemies.Default.Behavior.PatrolChaseAir;
using Arenbee.Actors.Enemies.Default.State;
using GameCore.Enums;

namespace Arenbee.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        public Whisp()
        {
            IsFloater = true;
            StateController = new StateController(
                this,
                new MoveStateMachine(this),
                new AirStateMachine(this),
                new HealthStateMachine(this),
                GetActionStateMachine());
        }

        public static string GetScenePath() => GDEx.GetScenePath();

        public override void Init()
        {
            BehaviorTree = new PatrolChaseAirBT(this);
        }

        public override ActionStateMachineBase GetActionStateMachine() => new ActionStateMachine(this);

        protected override void ApplyDefaultStats()
        {
            Stats.AddMod(new Modifier(StatType.ElementOff, (int)ElementType.Fire, ModOperator.Add, 1));
            Stats.AddMod(new Modifier(StatType.StatusEffectOff, (int)StatusEffectType.Burn, ModOperator.Add, value: 1, chance: 100));
            Stats.AddMod(new Modifier(StatType.StatusEffectDef, (int)StatusEffectType.Burn, ModOperator.Add, value: 100));
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Water, ModOperator.Add, ElementDef.Weak));
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Resist));
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Fire, ModOperator.Add, ElementDef.Absorb));
            Stats.SetAttribute(AttributeType.MaxHP, 4);
            Stats.SetAttribute(AttributeType.HP, 4);
            Stats.SetAttribute(AttributeType.Attack, 2);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }

        protected override void SetHitBoxes()
        {
            var bodybox = HitBoxes.GetNode<HitBox>("BodyBox");
            bodybox.SetBasicMeleeBox(this);
        }
    }
}
