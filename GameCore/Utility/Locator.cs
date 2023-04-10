using System.Collections.Generic;
using GameCore.ActionEffects;
using GameCore.Actors;
using GameCore.Audio;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace GameCore.Utility;

public static class Locator
{
    private static AActorDataDB s_actorDataDB = null!;
    private static IConditionLookup s_conditionLookup = null!;
    private static AEquipmentSlotCategoryDB s_equipmentSlotCategoryDB = null!;
    private static AGameRoot s_gameRoot = null!;
    private static AItemDB s_itemDB = null!;
    private static AItemCategoryDB s_itemCategoryDB = null!;
    private static ILoaderFactory s_loaderFactory = null!;
    private static AStatTypeDB s_statTypeDB = null!;
    private static AStatusEffectDB s_statusEffectDB = null!;
    private static IStatusEffectModifierFactory s_statusEffectModifierFactory = null!;
    private static ActionEffectDBBase s_actionEffectDB = null!;

    public static ActionEffectDBBase ActionEffectDB => s_actionEffectDB;
    public static AActorDataDB ActorDataDB => s_actorDataDB;
    public static AAudioController Audio => s_gameRoot.AudioController;
    public static IConditionLookup ConditionLookup => s_conditionLookup;
    public static AEquipmentSlotCategoryDB EquipmentSlotCategoryDB => s_equipmentSlotCategoryDB;
    public static AGameRoot Root => s_gameRoot;
    public static AGameSession? Session => s_gameRoot.GameSession;
    public static AItemDB ItemDB => s_itemDB;
    public static AItemCategoryDB ItemCategoryDB => s_itemCategoryDB;
    public static ILoaderFactory LoaderFactory => s_loaderFactory;
    public static AStatTypeDB StatTypeDB => s_statTypeDB;
    public static AStatusEffectDB StatusEffectDB => s_statusEffectDB;
    public static IStatusEffectModifierFactory StatusEffectModifierFactory => s_statusEffectModifierFactory;
    public static ATransitionController TransitionController => s_gameRoot.TransitionController;

    public static void ProvideActionEffectDB(ActionEffectDBBase actionEffectDB)
    {
        s_actionEffectDB = actionEffectDB;
    }

    public static void ProvideActorDataDB(AActorDataDB actorDataDB) => s_actorDataDB = actorDataDB;

    public static void ProvideConditionLookup(IConditionLookup conditionLookup)
    {
        s_conditionLookup = conditionLookup;
    }

    public static void ProvideEquipmentSlotCategoryDB(AEquipmentSlotCategoryDB equipmentSlotCategoryDB)
    {
        s_equipmentSlotCategoryDB = equipmentSlotCategoryDB;
    }

    public static void ProvideGameRoot(AGameRoot gameRoot)
    {
        if (GodotObject.IsInstanceValid(s_gameRoot))
            s_gameRoot.Free();
        s_gameRoot = gameRoot;
    }

    public static void ProvideItemDB(AItemDB itemDB) => s_itemDB = itemDB;

    public static void ProvideItemCategoryDB(AItemCategoryDB itemCategoryDB) => s_itemCategoryDB = itemCategoryDB;

    public static void ProvideLoaderFactory(ILoaderFactory loaderFactory) => s_loaderFactory = loaderFactory;

    public static void ProvideStatTypeDB(AStatTypeDB statTypeDB) => s_statTypeDB = statTypeDB;

    public static void ProvideStatusEffectDB(AStatusEffectDB statusEffectDB) => s_statusEffectDB = statusEffectDB;

    public static void ProvideStatusEffectModifierFactory(IStatusEffectModifierFactory factory)
    {
        s_statusEffectModifierFactory = factory;
    }

    public static void CheckReferences()
    {
        List<string> unsetRefs = new();
        if (s_actionEffectDB == null)
            unsetRefs.Add("ActionEffect DB");
        if (s_actorDataDB == null)
            unsetRefs.Add("ActorData DB");
        if (s_conditionLookup == null)
            unsetRefs.Add("Condition Lookup");
        if (s_equipmentSlotCategoryDB == null)
            unsetRefs.Add("EquipmentSlotCategory DB");
        if (s_gameRoot == null)
            unsetRefs.Add("Game Root");
        if (s_itemCategoryDB == null)
            unsetRefs.Add("ItemCategory DB");
        if (s_itemDB == null)
            unsetRefs.Add("Item DB");
        if (s_loaderFactory == null)
            unsetRefs.Add("Loader Factory");
        if (s_statTypeDB == null)
            unsetRefs.Add("Stat Type DB");
        if (s_statusEffectDB == null)
            unsetRefs.Add("Status Effect DB");
        if (s_statusEffectModifierFactory == null)
            unsetRefs.Add("StatusEffectModifier Factory");

        if (unsetRefs.Count > 0)
        {
            string errMessage = "The following static Locator references have not been set: " + string.Join(", ", unsetRefs)
                + ". Please create an autoload and load them in the _Ready method.";
            throw new System.Exception(errMessage);
        }
    }
}
