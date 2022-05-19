using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Extensions;
using Arenbee.Assets.Actors.Enemies.Default.State;
using Arenbee.Assets.Actors.Enemies.Default.Behavior.PatrolChaseGround;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Orc : Actor
    {
        public Orc()
        {
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
            BehaviorTree = new PatrolChaseGroundBT(this);
        }

        public override ActionStateMachineBase GetActionStateMachine() => new ActionStateMachine(this);

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
