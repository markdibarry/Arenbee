using Arenbee.ActionEffects;
using Arenbee.Actors;
using Arenbee.GUI;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Statistics;
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
        ActorsLocator.ProvideActorBodyDB(new ActorBodyDB());
        ActorsLocator.ProvideActorDataDB(new ActorDataDB());
        StatsLocator.ProvideConditionLookup(new ConditionLookup());
        StatsLocator.ProvideStatusEffectModifierFactory(new StatusEffectModifierFactory());
        Locator.ProvideLoaderFactory(new LoaderFactory());
        Locator.ProvideItemCategoryDB(new ItemCategoryDB());
        Locator.ProvideItemDB(new ItemDB());
        Locator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
        StatsLocator.ProvideStatTypeDB(new StatTypeDB());
        StatsLocator.ProvideStatusEffectDB(new StatusEffectDB());
        Locator.SetInitialized();
    }
}
