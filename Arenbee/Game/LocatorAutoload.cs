﻿using Arenbee.ActionEffects;
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
    public override void _Ready()
    {
        Locator.ProvideActionEffectDB(new ActionEffectDB());
        Locator.ProvideActorDataDB(new ActorDataDB());
        Locator.ProvideConditionLookup(new ConditionLookup());
        Locator.ProvideStatusEffectModifierFactory(new StatusEffectModifierFactory());
        Locator.ProvideLoaderFactory(new LoaderFactory());
        Locator.ProvideItemCategoryDB(new ItemCategoryDB());
        Locator.ProvideItemDB(new ItemDB());
        Locator.ProvideEquipmentSlotCategoryDB(new EquipmentSlotCategoryDB());
        Locator.ProvideStatTypeDB(new StatTypeDB());
        Locator.ProvideStatusEffectDB(new StatusEffectDB());
    }
}
