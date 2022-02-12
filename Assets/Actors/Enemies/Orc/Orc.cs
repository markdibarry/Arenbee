using Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseGround;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Orc : Actor
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public override void Init()
        {
            base.Init();
            StateController.Init(
                new BaseStates.Idle(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
            BehaviorTree = new PatrolChaseGroundBT(this);
        }

        protected override void SetDefaultStats()
        {
            DefenseElementModifiers.Add(new ElementModifier()
            {
                Element = Element.Earth,
                Value = ElementModifier.Weak
            });
            SetStat(StatType.MaxHP, 4);
            SetStat(StatType.HP, 4);
            SetStat(StatType.Attack, 4);
            SetStat(StatType.Defense, 0);
        }
    }
}
