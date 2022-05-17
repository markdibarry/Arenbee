using Arenbee.Assets.Actors.Default.State;
using Arenbee.Assets.Actors.Players.Default.State;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Players
{
    public partial class Ady : Actor
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        public override void Init()
        {
            StateController.Init<Standing, Grounded, Normal>();
        }

        public override void InitActionState()
        {
            StateController.ActionStateMachine.TransitionTo<NotAttacking>();
        }

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
