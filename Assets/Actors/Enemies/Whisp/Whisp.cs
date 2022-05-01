using Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseAir;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        public Whisp()
        {
            IsFloater = true;
            Friction = 50f;
            Acceleration = 1000f;
        }

        public static string GetScenePath() => GDEx.GetScenePath();

        public override void Init()
        {
            base.Init();
            StateController.Init(
                new Idle(),
                new JumpStates.Float(),
                new ActionStates.NotAttacking());
            BehaviorTree = new PatrolChaseAirBT(this);
        }

        protected override void ApplyDefaultStats()
        {
            Stats.AddMod(new Modifier(StatType.ElementOff, (int)ElementType.Fire, ModOperator.Add, 1));
            Stats.AddMod(new Modifier(StatType.StatusEffectOff, (int)StatusEffectType.Burn, ModOperator.Add, 1, 100));
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Water, ModOperator.Add, ElementDef.Weak));
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Resist));
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Fire, ModOperator.Add, ElementDef.Absorb));
            Stats.SetAttribute(AttributeType.MaxHP, 4);
            Stats.SetAttribute(AttributeType.HP, 4);
            Stats.SetAttribute(AttributeType.Attack, 2);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }
    }
}
