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
            base.Init();
            StateController.Init(
                new BaseStates.Idle(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
        }

        protected override void SetDefaultStats()
        {
            Stats.SetAttribute(AttributeType.MaxHP, 12);
            Stats.SetAttribute(AttributeType.HP, 12);
            Stats.SetAttribute(AttributeType.Attack, 0);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }
    }
}
