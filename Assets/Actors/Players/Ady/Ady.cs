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
            SetStat(StatType.Level, 1);
            SetStat(StatType.MaxHP, 12);
            SetStat(StatType.HP, 12);
            SetStat(StatType.MaxMP, 1);
            SetStat(StatType.MP, 1);
            SetStat(StatType.Attack, 0);
            SetStat(StatType.Defense, 0);
            SetStat(StatType.MagicAttack, 1);
            SetStat(StatType.MagicDefense, 1);
            SetStat(StatType.Luck, 1);
            SetStat(StatType.Evade, 1);
            SetStat(StatType.Speed, 1);
        }
    }
}
