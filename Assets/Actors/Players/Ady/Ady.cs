using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;

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
            Stats.SetAttribute(AttributeType.Level, 1);
            Stats.SetAttribute(AttributeType.MaxHP, 12);
            Stats.SetAttribute(AttributeType.HP, 12);
            Stats.SetAttribute(AttributeType.MaxMP, 1);
            Stats.SetAttribute(AttributeType.MP, 1);
            Stats.SetAttribute(AttributeType.Attack, 0);
            Stats.SetAttribute(AttributeType.Defense, 0);
            Stats.SetAttribute(AttributeType.MagicAttack, 1);
            Stats.SetAttribute(AttributeType.MagicDefense, 1);
            Stats.SetAttribute(AttributeType.Luck, 1);
            Stats.SetAttribute(AttributeType.Evade, 1);
            Stats.SetAttribute(AttributeType.Speed, 1);
        }
    }
}
