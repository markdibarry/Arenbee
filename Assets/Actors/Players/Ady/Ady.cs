using Arenbee.Assets.Actors.Default.State;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Players
{
    public partial class Ady : Actor
    {
        public Ady()
        {
            StateController = new StateController(
                this,
                new MoveStateMachine(this),
                new AirStateMachine(this),
                new HealthStateMachine(this),
                GetActionStateMachine());
        }

        public static string GetScenePath() => GDEx.GetScenePath();

        public override ActionStateMachineBase GetActionStateMachine() => new ActionStateMachine(this);

        protected override void ApplyDefaultStats()
        {
            Stats.AddMod(new Modifier(StatType.ElementDef, (int)ElementType.Dark, ModOperator.Add, ElementDef.Nullify));
            Stats.SetAttribute(AttributeType.MaxHP, 12);
            Stats.SetAttribute(AttributeType.HP, 12);
            Stats.SetAttribute(AttributeType.Attack, 0);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }
    }
}
