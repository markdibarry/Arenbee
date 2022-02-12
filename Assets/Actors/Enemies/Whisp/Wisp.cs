using Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseAir;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Wisp : Actor
    {
        public Wisp() : base()
        {
            IsFloater = true;
            Friction = 30f;
            Acceleration = 1000f;
        }

        public static string GetScenePath() => GDEx.GetScenePath();

        public override void Init()
        {
            base.Init();
            StateController.Init(
                new BaseStates.Idle(),
                new JumpStates.Float(),
                new ActionStates.NotAttacking());
            BehaviorTree = new PatrolChaseAirBT(this);
        }

        protected override void SetDefaultStats()
        {
            DefenseElementModifiers.Add(new ElementModifier()
            {
                Element = Element.Water,
                Value = ElementModifier.Weak
            });
            SetStat(StatType.MaxHP, 4);
            SetStat(StatType.HP, 4);
            SetStat(StatType.Attack, 2);
            SetStat(StatType.Defense, 1);
        }
    }
}
