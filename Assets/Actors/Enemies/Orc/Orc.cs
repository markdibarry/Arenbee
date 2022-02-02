using Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseGround;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;

namespace Arenbee.Assets.Actors.Enemies.OrcNS
{
    public partial class Orc : Actor
    {
        public static readonly string ScenePath = $"res://Assets/Actors/Enemies/Orc/{nameof(Orc)}.tscn";
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
