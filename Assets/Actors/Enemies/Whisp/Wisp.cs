using Arenbee.Assets.Enemies.Behavior.PatrolChaseAir;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Enemies.WhispNS
{
    public partial class Wisp : Actor
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            _isFloater = true;
            Friction = 30f;
            Acceleration = 1000f;
        }

        public override void Init()
        {
            base.Init();
            StateController.Init(
                new MoveStates.Idle(),
                new JumpStates.Float(),
                new ActionStates.NotAttacking());
            BehaviorTree = new PatrolChaseAirBT(this);
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
