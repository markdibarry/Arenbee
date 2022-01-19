using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Enemies.PlantNS
{
    public partial class Plant : Enemy
    {
        public override void Init()
        {
            base.Init();
            StateController.Init(
                new MoveStates.Idle(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
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
