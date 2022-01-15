using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Players.AdyNS
{
    public partial class Ady : Player
    {

        public override void _Ready()
        {
            base._Ready();
            StateController.Init(new MoveStates.Idle(), new ActionStates.NotAttacking());
        }

        public string TestAdyString { get; set; }

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
