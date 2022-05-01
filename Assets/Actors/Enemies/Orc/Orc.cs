using Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseGround;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Orc : Actor
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public override void Init()
        {
            base.Init();
            StateController.Init(
                new BaseStates.Idle(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
            BehaviorTree = new PatrolChaseGroundBT(this);
        }

        protected override void ApplyDefaultStats()
        {
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Weak));
            Stats.AddMod(new Modifier(StatType.StatusEffectOff, (int)StatusEffectType.Poison, ModOperator.Add, 1, 100));
            Stats.SetAttribute(AttributeType.MaxHP, 6);
            Stats.SetAttribute(AttributeType.HP, 6);
            Stats.SetAttribute(AttributeType.Attack, 4);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }
    }
}
