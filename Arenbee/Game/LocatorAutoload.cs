using Arenbee.ActionEffects;
using Arenbee.Actors;
using Arenbee.GUI;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Utility;
using Godot;

namespace Arenbee.Game;

[Tool]
public partial class LocatorAutoload : Node
{
    public override void _Process(double delta)
    {
        if (!Locator.Initialized)
            Initialize();
    }

    public override void _Ready() => Initialize();

    private static void Initialize()
    {
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideActorBodyDB(new ActorBodyDB());
        Locator.ProvideActorDataDB(new ActorDataDB());
        Locator.ProvideConditionLookup(new ConditionLookup());
        Locator.ProvideStatusEffectModifierFactory(new StatusEffectModifierFactory());
        Locator.ProvideLoaderFactory(new LoaderFactory());
        Locator.ProvideItemCategoryDB(new ItemCategoryDB());
        Locator.ProvideItemDB(new ItemDB());
        Locator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
        Locator.ProvideStatTypeDB(new StatTypeDB());
        Locator.ProvideStatusEffectDB(new StatusEffectDB());
        Locator.SetInitialized();
    }
}
