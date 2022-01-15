using Arenbee.Framework.Actors;
using Arenbee.Framework.Actors.Stats;
using Arenbee.Framework.Enums;
using Godot;
using System;

namespace Arenbee.Assets.Enemies.PlantNS
{
    public partial class Plant : Enemy
    {
        public override void _Ready()
        {
            base._Ready();
            StateController.Init(new MoveStates.Idle(), new ActionStates.NotAttacking());
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
