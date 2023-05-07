using Arenbee.ActionEffects;
using Arenbee.Actors;
using Arenbee.GUI;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace Arenbee;

[Tool]
public partial class LocatorAutoload : ALocatorAutoload
{
    protected override void Initialize()
    {
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideLoaderFactory(new LoaderFactory());
        ActorsLocator.ProvideActorBodyDB(new ActorBodyDB());
        ActorsLocator.ProvideActorDataDB(new ActorDataDB());
        ItemsLocator.ProvideItemCategoryDB(new ItemCategoryDB());
        ItemsLocator.ProvideItemDB(new ItemDB());
        ItemsLocator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
        StatsLocator.ProvideConditionLookup(new ConditionLookup());
        StatsLocator.ProvideStatusEffectModifierFactory(new StatusEffectModifierFactory());
        StatsLocator.ProvideStatTypeDB(new StatTypeDB());
        StatsLocator.ProvideStatusEffectDB(new StatusEffectDB());
    }
}
