using Arenbee.Assets.Actors.Enemies.Behavior.PatrolChaseAir;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Whisp : Actor
    {
        public Whisp()
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
                new Idle(),
                new JumpStates.Float(),
                new ActionStates.NotAttacking());
            BehaviorTree = new PatrolChaseAirBT(this);
        }

        protected override void SetDefaultStats()
        {
            Stats.ElementOffenses.Modifiers.Add(new ElementOffenseModifier(Element.Fire));
            Stats.AddElementDefenseMod(new ElementDefenseModifier()
            {
                Element = Element.Water,
                Value = ElementDefense.Weak
            });
            Stats.AddElementDefenseMod(new ElementDefenseModifier()
            {
                Element = Element.Earth,
                Value = ElementDefense.Resist
            });
            Stats.AddElementDefenseMod(new ElementDefenseModifier()
            {
                Element = Element.Fire,
                Value = ElementDefense.Absorb
            });
            Stats.SetAttribute(AttributeType.MaxHP, 4);
            Stats.SetAttribute(AttributeType.HP, 4);
            Stats.SetAttribute(AttributeType.Attack, 2);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }
    }
}
