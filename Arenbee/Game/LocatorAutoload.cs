using Arenbee.ActionEffects;
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
        Locator.ProvideItemDB(new ItemDB());
        Locator.ProvideItemCategoryDB(new ItemCategoryDB());
        Locator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideStatusEffectDB(new StatusEffectDB());
    }
}
