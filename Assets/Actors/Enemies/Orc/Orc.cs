using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Extensions;
using Arenbee.Assets.Actors.Enemies.Default.State;
using Arenbee.Assets.Actors.Enemies.Default.Behavior.PatrolChaseGround;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Orc : Actor
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        public override void Init()
        {
            StateController.Init<Standing, Grounded, Normal>();
            BehaviorTree = new PatrolChaseGroundBT(this);
        }

        public override void InitActionState()
        {
            StateController.ActionStateMachine.TransitionTo<NotAttacking>();
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
