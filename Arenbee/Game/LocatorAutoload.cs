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
public partial class LocatorAutoload : BaseLocatorAutoload
{
    protected override void Initialize()
    {
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideLoaderFactory(new LoaderFactory());
        ActorsLocator.ProvideActorBodyDB(new ActorBodyPathDB());
        ActorsLocator.ProvideActorDataDB(new ActorDataDB());
        StatsLocator.ProvideConditionLookup(new ConditionTypeDB());
        StatsLocator.ProvideStatusEffectModifierFactory(new StatusEffectModifierFactory());
        StatsLocator.ProvideStatTypeDB(new StatTypeDB());
        StatsLocator.ProvideStatusEffectDB(new StatusEffectDB());
        ItemsLocator.ProvideItemCategoryDB(new ItemCategoryDB());
        ItemsLocator.ProvideItemDB(new ItemDB());
        ItemsLocator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
    }
}
