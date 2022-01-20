using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Actors.Players.AdyNS
{
    public partial class Ady : Actor
    {
        public override void Init()
        {
            base.Init();
            StateController.Init(
                new BaseStates.Idle(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
        }

        protected override void SetStats()
        {
            ActorStats.InitStat(StatType.Level, 1);
            ActorStats.InitStat(StatType.MaxHP, 12);
            ActorStats.InitStat(StatType.HP, 12);
            ActorStats.InitStat(StatType.MaxMP, 1);
            ActorStats.InitStat(StatType.MP, 1);
            ActorStats.InitStat(StatType.Attack, 0);
            ActorStats.InitStat(StatType.Defense, 0);
            ActorStats.InitStat(StatType.MagicAttack, 1);
            ActorStats.InitStat(StatType.MagicDefense, 1);
            ActorStats.InitStat(StatType.Luck, 1);
            ActorStats.InitStat(StatType.Evade, 1);
            ActorStats.InitStat(StatType.Speed, 1);
            base.SetStats();
        }
    }
}
