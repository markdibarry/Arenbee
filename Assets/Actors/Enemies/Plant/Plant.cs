using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;
using Arenbee.Framework.Extensions;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Plant : Actor
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public override void Init()
        {
            base.Init();
            StateController.Init(
                new Idle(),
                new JumpStates.Grounded(),
                new ActionStates.NotAttacking());
        }

        protected override void SetDefaultStats()
        {
            Stats.ElementOffenses.Modifiers.Add(new ElementOffenseModifier(Element.Earth));
            Stats.AddElementDefenseMod(new ElementDefenseModifier()
            {
                Element = Element.Earth,
                Value = ElementDefense.Weak
            });
            Stats.SetAttribute(AttributeType.MaxHP, 4);
            Stats.SetAttribute(AttributeType.HP, 4);
            Stats.SetAttribute(AttributeType.Attack, 4);
            Stats.SetAttribute(AttributeType.Defense, 0);
        }
    }
}
