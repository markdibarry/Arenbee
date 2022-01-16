using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Enemies.OrcNS
{
    public partial class Orc : Enemy
    {
        public override void _Ready()
        {
            base._Ready();
            StateController.Init(
                new MoveStates.Walk(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            WalkSpeed = 50;
        }

        protected override void SetStats()
        {
            ActorStats.DefenseElementModifiers.Add(new ElementModifier()
            {
                Element = Element.Earth,
                Value = ElementModifier.Weak
            });
            ActorStats.InitStat(StatType.MaxHP, 4);
            ActorStats.InitStat(StatType.HP, 4);
            ActorStats.InitStat(StatType.Attack, 4);
            ActorStats.InitStat(StatType.Defense, 0);
            base.SetStats();
        }
    }
}
