using Arenbee.ActionEffects;
using Arenbee.Actors;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Game;

[Tool]
public partial class LocatorAutoload : Node
{
    public override void _Ready()
    {
        Locator.ProvideActorDataDB(new ActorDataDB());
        Locator.ProvideConditionEventFilterFactory(new ConditionEventFilterFactory());
        Locator.ProvideStatusEffectModifierFactory(new StatusEffectModifierFactory());
        Locator.ProvideItemCategoryDB(new ItemCategoryDB());
        Locator.ProvideItemDB(new ItemDB(Locator.ItemCategoryDB));
        Locator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideStatTypeDB(new StatTypeDB());
        Locator.ProvideStatusEffectDB(new StatusEffectDB());
    }
}
